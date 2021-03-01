using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Rema.DbAccess;
using Rema.Infrastructure.Email;
using Rema.Infrastructure.Email.Templates;
using Rema.Infrastructure.Models;
using Rema.ServiceLayer.ControllerLogic;
using Rema.WebApi.Filter;
using Rema.WebApi.ViewModels;
using Serilog;

namespace Rema.WebApi.Controllers
{
  [Produces("application/json")]
  [Route("[controller]")]
  [ApiController]
  [AuthorizeAd("Reader")]
  public class AllocationsController : BaseController
  {
    private readonly IEmailTrigger _emailTrigger;
    private readonly IAllocationService _allocationService;

    public AllocationsController(RpDbContext context, IMapper mapper, IEmailTrigger emailTrigger, IAllocationService allocationService) : base(context, mapper)
    {
      this._emailTrigger = emailTrigger;
      this._allocationService = allocationService;
    }

    [Route("print")]
    [HttpGet("print/{id}")]
    public async Task GetPrint(long id)
    {
      Log.Information("GET print allocation id:" + id);
      Allocation allocation;

      try
      {
        allocation = await _allocationService.FindAllocationAll(id);

        if (allocation == null)
        {
          Log.Warning("GET print allocation " + id + " not found id");
          return;
        }
      }
      catch (Exception ex)
      {
        Log.Error(ex, "error while getting allocation");
        return;
      }

      try
      {
        MemoryStream stream = this._allocationService.GenerateAllocationPrintPdf(allocation);

        Response.ContentType = "application/pdf";
        Response.StatusCode = 200;
        await stream.CopyToAsync(Response.Body);
        await Response.CompleteAsync();
        await stream.DisposeAsync();
      }
      catch (Exception ex)
      {
        Log.Error(ex, "creating pdf and sending for id: " + id);
      }
    }

    // GET: allocations
    [HttpGet]
    public async Task<ActionResult<IEnumerable<AllocationViewModel>>> GetAllocations()
    {
      Log.Information("GET allocations");
      var today = DateTime.Now;
      today = today.AddMonths(-1);
      var firstDayOfLastMonth = today.AddDays(-today.Day + 1);  // first day of last month

      IQueryable<Allocation> query;

      try
      {
        query = _context.Allocations
          .Where(g => g.To > firstDayOfLastMonth);

        query.Include(g => g.Ressources)
          .Include(g => g.ApprovedBy)
          .Load();

        query.Include(g => g.CreatedBy)
          .Include(g => g.Gadgets)
          .Load();

        query.Include(g => g.LastModifiedBy)
          .Include(g => g.ReferencePerson)
          .Load();
      }
      catch (Exception ex)
      {
        Log.Error(ex, "error while getting allocations");
        return NotFound();
      }

      try
      {
        var allMapped = await query.Select(e => _mapper.Map<Allocation, AllocationViewModel>(e)).ToListAsync();
        var json = Ok(allMapped);
        return json;
      }
      catch (Exception ex)
      {
        Log.Error(ex, "error while mapping allocations");
        return NotFound();
      }
    }

    private async Task<Allocation> RetrieveAllocation(long id)
    {
      try
      {
        var allocation = await _context.Allocations.FindAsync(id);
        if (allocation == null)
        {
          return null;
        }
        return allocation;
      }
      catch (Exception ex)
      {
        Log.Error(ex, "error while getting allocation");
        return null;
      }
    }

    // GET: allocations/5
    [HttpGet("{id}")]
    public async Task<ActionResult<Allocation>> GetAllocation(long id)
    {
      Log.Information("GET allocation/{id}", id);

      var allocation = await RetrieveAllocation(id);
      if (allocation != null) return Ok(allocation);
      return NotFound();
    }

    // POST: allocation
    [HttpPost]
    public async Task<ActionResult<AllocationViewModel>> PostAllocation(AllocationViewModel allocationVM)
    {
      Log.Information("POST allocation: {allocation}", allocationVM);
      Allocation allocation;
      User requestedUser;

      try
      {
        requestedUser = base.RequestSender;
      }
      catch (Exception ex)
      {
        Log.Error(ex, "error while getting request sender");
        return Conflict();
      }

      try
      {
        allocation = _mapper.Map<AllocationViewModel, Allocation>(allocationVM);
      }
      catch (Exception ex)
      {
        Log.Error(ex, "error while mapping allocation");
        return BadRequest();
      }

      if (allocationVM.HintsForSuppliers.Any())
        try
        {
          var searchedGroupIds = allocationVM.HintsForSuppliers.Select(g => g.GroupId);
          var groups = await _context.SupplierGroups.Where(g => searchedGroupIds.Contains(g.Id)).ToListAsync();
          foreach (SimpleSupplierHint hint in allocationVM.HintsForSuppliers)
          {
            var group = groups.Find(e => e.Id == hint.GroupId);
            var newHintsList = allocation.HintsForSuppliers;  // list existiert nur virtuell als deserialisierung!
            newHintsList.Add(new SupplierHint() { Group = group, Message = hint.Message });
            allocation.HintsForSuppliers = newHintsList;
          }
        }
        catch (Exception ex)
        {
          Log.Error(ex, "error while mapping supplier hints");
          return BadRequest();
        }

      try
      {
        MapChangedOrNewRessources(allocation, allocationVM);
      }
      catch (Exception ex)
      {
        Log.Error(ex, "error while getting ressource");
      }

      try
      {
        MapChangedOrNewGadget(allocation, allocationVM);
      }
      catch (Exception ex)
      {
        Log.Error(ex, "error while mapping gadgets to allocation");
      }


      try
      {
        allocation.LastModified = DateTime.Now;
        allocation.LastModifiedBy = requestedUser;
      }
      catch (Exception ex)
      {
        Log.Error(ex, "error while setting modified informations");
      }

      try
      {
        allocation.CreatedAt = DateTime.Now;
        allocation.CreatedBy = requestedUser;
      }
      catch (Exception ex)
      {
        Log.Error(ex, "error while setting created informations");
      }

      try
      {
        allocation.ApprovedBy = requestedUser;
      }
      catch (Exception ex)
      {
        Log.Error(ex, "error while setting approved information");
      }

      try
      {
        if (allocation.IsAllDay)
        {
          allocation.From = allocation.From.Date + new TimeSpan(0, 0, 0);
          allocation.To = allocation.To.Date + new TimeSpan(23, 59, 00);
        }
      }
      catch (Exception ex)
      {
        Log.Error(ex, "error while set correct times for all day");
      }

      try
      {
        if (!string.IsNullOrEmpty(allocationVM.ReferencePersonId))
        {
          allocation.ReferencePerson = _userManagementService.GetAndUpdateOrInsertUserFromDB(allocationVM.ReferencePersonId);
        }
        else if (allocationVM.Status == MeetingStatus.Pending)
        {
          allocation.ReferencePerson = requestedUser;
        }
      }
      catch (Exception ex)
      {
        Log.Error(ex, "error while setting referencePerson");
      }

      try
      {
        _context.Allocations.Add(allocation);
        await _context.SaveChangesAsync();
      }
      catch (Exception ex)
      {
        Log.Error(ex, "error while saving the new allocation");
        return Conflict();
      }

      var isBooking = allocation.Status >= MeetingStatus.Approved && base.RequestSenderVM.Roles.Exists(e => e.HasRole(Startup.Editor));
      allocation.Status = isBooking ? allocationVM.Status : MeetingStatus.Pending;
      var yourRequest = isBooking ? "Buchung" : "Anfrage";

      try
      {
        allocation = await _context.Allocations.FindAsync(allocation.Id);
        var template = new NewAllocationTemplate(allocation, yourRequest);
        var emailSuccess = this._emailTrigger.SendEmail(template, allocation?.ReferencePerson?.Email, template.GetGroupEmails());
        AddMailErrorHeaderCondidtional(emailSuccess);
      }
      catch (Exception ex)
      {
        Log.Error(ex, "error while processing mails");
      }

      try
      {
        var returnAllocation = _mapper.Map<Allocation, AllocationViewModel>(allocation);
        return CreatedAtAction("GetAllocation", new { id = allocation.Id }, returnAllocation);
      }
      catch (Exception ex)
      {
        Log.Error(ex, "error while mapping save allocation to result");
      }

      return Ok();
    }

    // POST: allocations
    [Route("PostAllocations")]
    [HttpPost]
    public async Task<ActionResult<AllocationViewModel>> PostAllocations(MultipleAllocationsViewModel allocationsVM)
    {
      Log.Information("POST allocations: {allocation}", allocationsVM);

      List<Allocation> allocations = new List<Allocation>();
      User requestedUser;
      Guid scheduleSeries = Guid.NewGuid();

      try
      {
        requestedUser = await _context.Users.FindAsync(this.RequestSender.Id);
      }
      catch (Exception ex)
      {
        Log.Error(ex, "error while getting request sender");
        return Conflict();
      }

      try
      {
        foreach (var date in allocationsVM.Dates)
        {
          var singleDate = _mapper.Map<AllocationViewModel, Allocation>(allocationsVM);
          var newFrom = DateTime.Parse(date);
          singleDate.From = newFrom.Date + allocationsVM.From.TimeOfDay;

          var newTo = DateTime.Parse(date);
          singleDate.To = newFrom.Date + allocationsVM.To.TimeOfDay;

          singleDate.ScheduleSeriesGuid = scheduleSeries;
          allocations.Add(singleDate);
        }
      }
      catch (Exception ex)
      {
        Log.Error(ex, "error while mapping allocations");
        return BadRequest();
      }

      try
      {
        allocations.ForEach(
          e => MapChangedOrNewRessources(e, allocationsVM));
      }
      catch (Exception ex)
      {
        Log.Error(ex, "error while getting ressource");
      }

      try
      {
        var gadgets = await _context.Gadgets
          .Include(e => e.SuppliedBy)
          .Where(g => allocationsVM.GadgetsIds.Contains(g.Id))
          .ToListAsync();

        allocations.ForEach(a =>
        {
          a.Gadgets = new List<Gadget>();
          gadgets.ForEach(g => a.Gadgets.Add(g));
        });
      }
      catch (Exception ex)
      {
        Log.Error(ex, "error while mapping gadgets to allocation");
      }

      try
      {
        allocations.ForEach(e => e.LastModified = DateTime.Now);
        allocations.ForEach(e => e.LastModifiedBy = requestedUser);
      }
      catch (Exception ex)
      {
        Log.Error(ex, "error while setting modified informations");
      }

      try
      {
        allocations.ForEach(e => e.CreatedAt = DateTime.Now);
        allocations.ForEach(e => e.CreatedBy = requestedUser);
      }
      catch (Exception ex)
      {
        Log.Error(ex, "error while setting created informations");
      }

      try
      {
        allocations.ForEach(e => e.ApprovedBy = requestedUser);
      }
      catch (Exception ex)
      {
        Log.Error(ex, "error while setting approved information");
      }

      try
      {
        if (allocationsVM.IsAllDay)
        {
          allocations.ForEach((e) =>
          {
            e.From = e.From.Date + new TimeSpan(0, 0, 0);
            e.To = e.To.Date + new TimeSpan(23, 59, 00);
          });
        }
      }
      catch (Exception ex)
      {
        Log.Error(ex, "error while set correct times for all day");
      }

      if (allocationsVM.HintsForSuppliers.Any())
        try
        {
          var searchedGroupIds = allocationsVM.HintsForSuppliers.Select(g => g.GroupId);
          var groups = await _context.SupplierGroups.Where(g => searchedGroupIds.Contains(g.Id)).ToListAsync();

          var hintsList = new List<SupplierHint>();
          foreach (SimpleSupplierHint hint in allocationsVM.HintsForSuppliers)
          {
            var group = groups.Find(e => e.Id == hint.GroupId);
            hintsList.Add(new SupplierHint() { Group = group, Message = hint.Message });
          }
          allocations.ForEach(e => e.HintsForSuppliers = hintsList);
        }
        catch (Exception ex)
        {
          Log.Error(ex, "error while mapping supplier hints");
          return BadRequest();
        }

      try
      {
        if (!string.IsNullOrEmpty(allocationsVM.ReferencePersonId))
        {
          var referencePerson = await _context.Users.SingleOrDefaultAsync(e => e.ActiveDirectoryID == allocationsVM.ReferencePersonId);
          allocations.ForEach(e => e.ReferencePerson = referencePerson);
        }
        else if (allocationsVM.Status == MeetingStatus.Pending)
        {
          allocations.ForEach(e => e.ReferencePerson = requestedUser);
        }
      }
      catch (Exception ex)
      {
        Log.Error(ex, "error while setting referencePerson");
      }

      var status = allocations[0].Status;
      Boolean hasApproveRight = base.RequestSenderVM.Roles.Exists(e => e.HasRole(Startup.Editor));
      if (status >= MeetingStatus.Approved && !hasApproveRight)
        return new UnauthorizedResult();

      try
      {
        allocations.ForEach(e => _context.Allocations.Add(e));
        await _context.SaveChangesAsync();
      }
      catch (Exception ex)
      {
        Log.Error(ex, "error while saving the new allocation");
        return Conflict();
      }

      try
      {
        var recipient = allocations[0]?.ReferencePerson?.Email;

        var isBooking = allocations[0].Status >= MeetingStatus.Approved && base.RequestSenderVM.Roles.Exists(e => e.HasRole(Startup.Editor));
        var yourRequest = isBooking ? "Buchung" : "Buchungsanfrage";

        var template = new NewAllocationTemplate(allocations, yourRequest);
        var emailSuccess = this._emailTrigger.SendEmail(template, recipient, template.GetGroupEmails());
        AddMailErrorHeaderCondidtional(emailSuccess);
      }
      catch (Exception ex)
      {
        Log.Error(ex, "error while processing mails");
        return Conflict();
      }

      return Ok();
    }

    // DELETE: allocations/5
    [HttpDelete("{id}")]
    [AuthorizeAd("Editor")]
    public async Task<ActionResult<Allocation>> DeleteAllocation(long id)
    {
      Allocation allocation;

      Log.Information("DELETE allocations/{id}", id);

      try
      {
        allocation = await _allocationService.FindAllocationAll(id);
      }
      catch (Exception ex)
      {
        Log.Error(ex, "error while getting allocation");
        return NotFound();
      }

      if (allocation == null)
      {
        return NotFound();
      }

      bool hasRight = base.RequestSenderVM.Roles.Exists(e => e.HasRole(Startup.Editor)) ||
          (allocation.CreatedBy.Id == base.RequestSenderVM.Id && allocation.Status == MeetingStatus.Pending);
      if (!hasRight)
      {
        Log.Warning("User {user} was restricted to change allocation {allocation}", base.RequestSenderVM, allocation);
        return new UnauthorizedResult();
      }

      try
      {
        _context.Allocations.Remove(allocation);
        await _context.SaveChangesAsync();
      }
      catch (Exception ex)
      {
        Log.Error(ex, "error while deleting allocation");
      }

      var template = new DeletedAllocationTemplate(allocation);
      var emailSuccess = this._emailTrigger.SendEmail(template, allocation?.ReferencePerson?.Email, template.GetGroupEmails());
      AddMailErrorHeaderCondidtional(emailSuccess);

      return Ok();
    }

    // PUT: allocations/editRequest
    [HttpPut("editRequest/")]
    [AuthorizeAd("Editor")]
    public async Task<ActionResult<Boolean>> EditRequest(AllocationRequestEdition editedRequest)
    {
      // Todo: klären ob eine Absage gleichbedeutend mit einer Löschung ist
      Log.Information("PUT allocations/editrequest: {editRequest}", editedRequest);

      Allocation allocation;
      
      try
      {
        allocation = await _allocationService.FindAllocationAll(editedRequest.Id);
      }
      catch (Exception ex)
      {
        Log.Error(ex, "error while getting allocation");
        return NotFound();
      }

      if (allocation == null)
      {
        return NotFound();
      }
      var ed = editedRequest;
      try
      {
        allocation.Status = (MeetingStatus)editedRequest.status;
        allocation.LastModified = DateTime.Now;
        if (allocation.LastModifiedBy.Id != base.RequestSender.Id)
          allocation.LastModifiedBy = base.RequestSender;
        allocation.ApprovedAt = DateTime.Now;
        allocation.ApprovedBy = base.RequestSender;

        if ((MeetingStatus)editedRequest.status == MeetingStatus.Moved)
        {
          allocation.From = editedRequest.From.GetValueOrDefault();
          allocation.To = editedRequest.To.GetValueOrDefault();
          allocation.IsAllDay = editedRequest.IsAllDay;

          if (editedRequest.IsAllDay)
          {
            try
            {
              allocation.From = editedRequest.From.GetValueOrDefault().Date + new TimeSpan(0, 0, 0);
              allocation.To = editedRequest.To.GetValueOrDefault().Date + new TimeSpan(23, 59, 00);
            }
            catch (Exception ex)
            {
              Log.Error(ex, "error while set correct times for all day");
            }
          }
        }

        await _context.SaveChangesAsync();
      }
      catch (Exception ex)
      {
        Log.Error(ex, "error while editing and saving allocation");
        return Conflict();
      }

      try
      {
        EmailTemplate template = null;
        if ((MeetingStatus)editedRequest.status == MeetingStatus.Moved)
        {
          template = new StatusChangedTemplate(allocation, "verschoben");
        }
        else if ((MeetingStatus)editedRequest.status == MeetingStatus.Approved)
        {
          template = new StatusChangedTemplate(allocation, "genehmigt");
        }
        else if ((MeetingStatus)editedRequest.status == MeetingStatus.Clarification)
        {
          template = new StatusChangedTemplate(allocation, "abgelehnt");
        }
        if (template != null)
        {
          var emailSuccess = this._emailTrigger.SendEmail(template, allocation?.ReferencePerson?.Email, new List<string>());
          AddMailErrorHeaderCondidtional(emailSuccess);
        }
      }
      catch (Exception ex)
      {
        Log.Error(ex, "error while processing mails");
      }

      return Ok();
    }

    // PUT: allocations/{editAllocation}
    [HttpPut("edit/{editAllocation}")]
    [AuthorizeAd("Editor")]
    public async Task<ActionResult<Boolean>> EditAllocation(AllocationViewModel allocationVM)
    {
      Log.Information("PUT allocations/editAllocation: {allocationModel}", allocationVM);

      if (allocationVM.Id == 0)
      {
        Log.Error("allocation id was not filled");
        return BadRequest();
      }

      Allocation oldAllocation;
      try
      {
        oldAllocation = await _allocationService.FindAllocationAll(allocationVM.Id);
        if (oldAllocation == null) return BadRequest("ID does not exist");
      }
      catch (Exception ex)
      {
        Log.Error(ex, "error while getting allocation");
        return NotFound();
      }

      // bool ressourceChanged = false;
      Allocation changedAllocation;
      try
      {
        changedAllocation = _mapper.Map<AllocationViewModel, Allocation>(allocationVM);
      }
      catch (Exception ex)
      {
        Log.Error(ex, "error while mapping allocation");
        return BadRequest();
      }

      if (!string.IsNullOrEmpty(allocationVM.ScheduleSeries) && oldAllocation.ScheduleSeriesGuid == null)
      {
        return BadRequest(); // nachträgliches hinzufügen zu serienterminen ist nicht möglich
      }

      if (string.IsNullOrEmpty(allocationVM.ScheduleSeries) && oldAllocation.ScheduleSeriesGuid != null)
      {
        oldAllocation.ScheduleSeriesGuid = null;
      }

      try
      {
        if (!string.IsNullOrEmpty(allocationVM.ReferencePersonId) && allocationVM.ReferencePersonId.Length > 12)
        {
          oldAllocation.ReferencePerson = _userManagementService.GetAndUpdateOrInsertUserFromDB(allocationVM.ReferencePersonId); ;
        }
        else if (string.IsNullOrEmpty(allocationVM.ReferencePersonId))
        {
          oldAllocation.ReferencePerson = null;
        }
      }
      catch (Exception ex)
      {
        Log.Error(ex, "error while changing referencePerson");
      }

      MapChangedOrNewRessources(oldAllocation, allocationVM);

      var oldHintsGroupIds = oldAllocation.HintsForSuppliers.Select(e => e.Group.Id);
      oldAllocation.HintsForSuppliers = new List<SupplierHint>();
      if (allocationVM.HintsForSuppliers.Any())
        try
        {
          var searchedGroupIds = allocationVM.HintsForSuppliers.Select(g => g.GroupId);
          var groups = await _context.SupplierGroups.Where(g => searchedGroupIds.Contains(g.Id)).ToListAsync();
          foreach (SimpleSupplierHint hint in allocationVM.HintsForSuppliers)
          {
            var group = groups.Find(e => e.Id == hint.GroupId);
            var newHintsList = oldAllocation.HintsForSuppliers;  // list existiert nur virtuell als deserialisierung!
            newHintsList.Add(new SupplierHint() { Group = group, Message = hint.Message });
            oldAllocation.HintsForSuppliers = newHintsList;
          }
        }
        catch (Exception ex)
        {
          Log.Error(ex, "error while mapping supplier hints");
          return BadRequest();
        }

      oldAllocation.Gadgets = oldAllocation.Gadgets ?? new List<Gadget>();
      // Entfallene Hilfsmittel müssen aus allocationGadgets vom Allocation-Objekt entfernt werden
      // Neue Hilfsmittel müssen hinzugefügt werden
      var currentGadgetIds = oldAllocation.Gadgets.Select(x => x.Id);
      var newGadgets = allocationVM.GadgetsIds.Where(x => !currentGadgetIds.Contains(x));
      var droppedGadgetIds = currentGadgetIds.Where(x => !allocationVM.GadgetsIds.Contains(x)).ToArray();
      
      var createdGadgets = new List<Gadget>();
      var deletedGadgets = new List<Gadget>();

      if (newGadgets.Any())
      {
        try
        {
          createdGadgets = await _context.Gadgets
            .Include(g => g.SuppliedBy)
            .Where(g => newGadgets.Contains(g.Id))
            .ToListAsync();

          createdGadgets.ForEach(ng => oldAllocation.Gadgets.Add(ng));
        }
        catch (Exception ex)
        {
          Log.Error(ex, "error while creating new gadgets to allocation");
        }
      }

      if (droppedGadgetIds.Any())
      {
        try
        {
          deletedGadgets = await _context.Gadgets
            .Include(g => g.SuppliedBy)
            .Where(g => droppedGadgetIds.Contains(g.Id))
            .ToListAsync();

          deletedGadgets.ForEach(rg => oldAllocation.Gadgets.Remove(rg));
        }
        catch (Exception ex)
        {
          Log.Error(ex, "error while creating deleted gadgets of allocation");
        }
      }

      // Prüfung der Nachrichten an Unterstützergruppen und auswahl der betroffenen Gruppen
      var newHintsGroupIds = allocationVM.HintsForSuppliers.Select(e => e.GroupId);
      var hintGroupMails = await _context.SupplierGroups
        .Where(e => oldHintsGroupIds.Contains(e.Id) || newHintsGroupIds.Contains(e.Id))
        .Select(e => e.GroupEmail)
        .ToListAsync();

      // Alle Unterstützergruppen müssen benachrichtigt werden wenn sich etwas relevantes ändert, daher:
      // if (ressourceChanged || (allocationVM.IsAllDay != oldAllocation.IsAllDay) || (oldAllocation.From != changedAllocation.From) || (oldAllocation.To != changedAllocation.To)) {
      // Änderung: einfach alle benachrichtigen (ggf. das granularere konfigurierbar machen
      var allHintsGroups = oldAllocation.HintsForSuppliers.Select(x => x.Group.GroupEmail).ToList();
      var allGadgetGroups = oldAllocation.Gadgets.Select(x => x.SuppliedBy.GroupEmail).ToList();
      hintGroupMails.AddRange(allHintsGroups);
      hintGroupMails.AddRange(allGadgetGroups);
      // }

      try
      {
        if (allocationVM.IsAllDay)
        {
          oldAllocation.From = oldAllocation.From.Date + new TimeSpan(0, 0, 0);
          oldAllocation.To = oldAllocation.To.Date + new TimeSpan(23, 59, 00);
        }
      }
      catch (Exception ex)
      {
        Log.Error(ex, "error while set correct times for all day");
      }

      // Änderungen sind nur für Bearbeiter und darüber hinaus erlaubt, außer wenn Anfrage noch nicht genehmigt
      bool hasRight = base.RequestSenderVM.Roles.Exists(e => e.HasRole(Startup.Editor)) ||
                      (oldAllocation.CreatedBy.Id == base.RequestSenderVM.Id && oldAllocation.Status == MeetingStatus.Pending);
      if (!hasRight)
      {
        Log.Warning("User {user} was restricted to change allocation {allocation}", base.RequestSenderVM, changedAllocation);
        return new UnauthorizedResult();
      }

      try
      {
        oldAllocation.Title = changedAllocation.Title;
        oldAllocation.IsAllDay = changedAllocation.IsAllDay;
        oldAllocation.Notes = changedAllocation.Notes;
        oldAllocation.From = changedAllocation.From;
        oldAllocation.To = changedAllocation.To;
        oldAllocation.ContactPhone = changedAllocation.ContactPhone;
        // ressources are already changed
        // gadgets are added above

        oldAllocation.LastModified = DateTime.Now;
        if (oldAllocation.LastModifiedBy.Id != base.RequestSender.Id)
          oldAllocation.LastModifiedBy = base.RequestSender;
      }
      catch (Exception ex)
      {
        Log.Error(ex, "error while set modified values for allocation");
        return Conflict();
      }

      try
      {
        await _context.SaveChangesAsync();
      }
      catch (Exception ex)
      {
        Log.Error(ex, "error while save allocation");
        return Conflict();
      }

      var template = new GadgetUpdateTemplate(oldAllocation, createdGadgets, deletedGadgets, hintGroupMails);
      var emailSuccess = this._emailTrigger.SendEmail(template, oldAllocation?.ReferencePerson?.Email, template.GetGroupEmails());
      AddMailErrorHeaderCondidtional(emailSuccess);
      return Ok();
    }

    private void MapChangedOrNewRessources(Allocation oldAllocation, AllocationViewModel allocationVM)
    {
      var newRessources = _context.Ressources.Where(r => allocationVM.RessourceIds.Contains(r.Id));
      oldAllocation.Ressources = new List<Ressource>();
      foreach (var ressource in newRessources)
        oldAllocation.Ressources.Add(ressource);
    }

    private void MapChangedOrNewGadget(Allocation oldAllocation, AllocationViewModel allocationVM)
    {
      var newGadgets = _context.Gadgets.Where(r => allocationVM.GadgetsIds.Contains(r.Id))
        .Include(r => r.SuppliedBy)
        .ToList();

      oldAllocation.Gadgets = new List<Gadget>();
      foreach (var gadget in newGadgets)
        oldAllocation.Gadgets.Add(gadget);
    }

    private void AddMailErrorHeaderCondidtional(bool hasNoError)
    {
      if (!hasNoError)
        Response.Headers.Add("mailerror", "Fehler bei Versand der Benachrichtigungs-E-Mails");
    }

    // könnte mal benutzt werden
    /*
[HttpGet]
[Route("timespan/m/{year}/{month}")]
public async Task<ActionResult<IEnumerable<AllocationViewModel>>> GetAllocationsMonth(long year, long month)
{
  Log.Information("GET allocations timespan month");

  // get the first day of the month
  DateTime first = new DateTime((int)year, (int)month, 1);

  // subtract 6 days so the last few allocations are displayed
  first = first.AddDays(-6);

  // Get the last day of the month
  DateTime last = new DateTime((int)year, (int)month, DateTime.DaysInMonth((int)year, (int)month));

  // add 6 days so the last few allocations are displayed
  last = last.AddDays(6);

  Console.WriteLine("First Date: {0}", first);
  Console.WriteLine("Last Date: {0}", last);

  IQueryable<Allocation> query;

  try
  {
    query = _context.Allocations
      .Where(g => (g.To >= first && g.To <= last) || (g.From <= last && g.From >= first) || (g.From <= first && g.To >= last));

    query.Include(g => g.Ressources)
      .Include(g => g.ApprovedBy)
      .Load();

    query.Include(g => g.CreatedBy)
      .Include(g => g.AllocationGadgets)
      .Load();

    query.Include(g => g.LastModifiedBy)
      .Include(g => g.ReferencePerson)
      .Load();

    var allMapped = await query.Select(e => _mapper.Map<Allocation, AllocationViewModel>(e)).ToListAsync();
    var json = Ok(allMapped);
    return json;
  }
  catch (Exception ex)
  {
    Log.Error(ex, "error while getting allocations");
    return NotFound();
  }
}

[HttpGet]
[Route("timespan/w/{year}/{week}")]
public async Task<ActionResult<IEnumerable<AllocationViewModel>>> GetAllocationsWeek(long year, long week)
{
  Log.Information("GET allocations timespan week");

  IQueryable<Allocation> query;

  DateTime first = System.Globalization.ISOWeek.ToDateTime((int)year, (int)week, DayOfWeek.Monday);
  DateTime last = first.AddDays(6);

  Console.WriteLine("First Date: {0}", first);
  Console.WriteLine("Last Date: {0}", last);

  try
  {
    query = _context.Allocations
      .Where(g => (g.To >= first && g.To <= last) || (g.From <= last && g.From >= first) || (g.From <= first && g.To >= last));

    query.Include(g => g.Ressources)
      .Include(g => g.ApprovedBy)
      .Load();

    query.Include(g => g.CreatedBy)
      .Include(g => g.AllocationGadgets)
      .Load();

    query.Include(g => g.LastModifiedBy)
      .Include(g => g.ReferencePerson)
      .Load();

    var allMapped = await query.Select(e => _mapper.Map<Allocation, AllocationViewModel>(e)).ToListAsync();
    var json = Ok(allMapped);
    return json;
  }
  catch (Exception ex)
  {
    Log.Error(ex, "error while getting allocations");
    return NotFound();
  }
}
*/
  }
}
