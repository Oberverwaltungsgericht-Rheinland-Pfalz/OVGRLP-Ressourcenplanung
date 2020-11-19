using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using PdfSharpCore.Drawing;
using PdfSharpCore.Drawing.Layout;
using PdfSharpCore.Pdf;
using Rema.DbAccess;
using Rema.Infrastructure.Email;
using Rema.Infrastructure.Email.Templates;
using Rema.Infrastructure.Models;
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

    public AllocationsController(RpDbContext context, IMapper mapper, IEmailTrigger emailTrigger) : base(context, mapper)
    {
      this._emailTrigger = emailTrigger;
    }

    [Route("print")]
    [HttpGet("print/{id}")]
    public async Task<FileStreamResult> GetPrint(long id)
    {
      Log.Information("GET print allocation id:" + id);
      Allocation allocation;

      try
      {
        allocation = await _context.Allocations
          .Include(o => o.Ressources)
          .Include(r => r.ReferencePerson)
          .Include(r => r.LastModifiedBy)
          .Include(r => r.CreatedBy)
          .Include(g => g.AllocationGadgets).ThenInclude(ag => ag.Gadget).ThenInclude(g => g.SuppliedBy)
          .FirstOrDefaultAsync(i => i.Id == id);

        if (allocation == null)
        {
          return null;
        }
      }
      catch (Exception ex)
      {
        Log.Error(ex, "error while getting allocation");
        return null;
      }

      string filename = DateTime.UtcNow.Ticks + "print.pdf";
      try
      {
        var template = new PrintTemplate(allocation);

        PdfDocument document = new PdfDocument();

        PdfPage page = document.AddPage();
        XGraphics gfx = XGraphics.FromPdfPage(page);
        XFont font = new XFont("Calibri", 12, XFontStyle.Bold);
        XTextFormatter tf = new XTextFormatter(gfx);

        XRect rect = new XRect(40, 50, page.Width * 0.8, page.Height * 7.5);
        gfx.DrawRectangle(XBrushes.White, rect);
        tf.DrawString(template.ToString(), font, XBrushes.Black, rect, XStringFormats.TopLeft);

        //var document = new PdfDocument();
        //var page = document.AddPage();
        //var gfx = XGraphics.FromPdfPage(page);
        //var font = new XFont("OpenSans", 12, XFontStyle.Bold);

        //gfx.DrawString(template.ToString(), font, XBrushes.Black, new XRect(20, 20, page.Width/10, /*page.Height*/ 25), XStringFormats.Center);

        document.Save(filename);

        var fs = new FileStream(filename, FileMode.Open, FileAccess.Read, FileShare.None, 4096, FileOptions.DeleteOnClose);
        return File(fs, "application/pdf", "FileDownloadName.pdf");
      }
      catch (Exception ex)
      {
        Log.Error(ex, "printing pdf for id: " + id);
        return null;
      }
      finally
      {
        // System.IO.File.Delete(filename);
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
          .Include(g => g.AllocationGadgets)
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

    /*
        // PUT: allocations/5
        [HttpPut("{id}")]
        public async Task<IActionResult> PutAllocation(long id, AllocationViewModel allocationVM)
        {
          Log.Information("PUT allocations/{id}: {allocation}", id, allocationVM);

          if (id != allocationVM.Id)
          {
            Log.Error("allocation not mached the id");
            return BadRequest();
          }
          Allocation allocation;
          try
          {
            allocation = _mapper.Map<AllocationViewModel, Allocation>(allocationVM);
          }
          catch (Exception ex)
          {
            Log.Error(ex, "error while mapping allocation");
            return BadRequest();
          }

          bool hasRight = base.RequestSenderVM.Roles.Exists(e => e.HasRole(Startup.Editor)) || allocation.CreatedBy.Id == base.RequestSenderVM.Id;
          if (!hasRight)
          {
            Log.Warning("User {user} was restricted to change allocation {allocation}", base.RequestSenderVM, allocation);
            return new UnauthorizedResult();
          }

          try
          {
            allocation.LastModified = DateTime.Now;
            if (allocation.LastModifiedBy.Id != base.RequestSender.Id)
              allocation.LastModifiedBy = base.RequestSender;
            _context.Entry(allocation).State = EntityState.Modified;
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

          return Ok();
        } */

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
        var gadgets = await _context.Gadgets.Include(e => e.SuppliedBy).Where(g => allocationVM.GadgetsIds.Contains(g.Id)).ToListAsync();

        allocation.AllocationGadgets = new List<AllocationGagdet>();
        foreach (var g in gadgets)
        {
          allocation.AllocationGadgets.Add(new AllocationGagdet
          {
            AllocationId = allocation.Id,
            Allocation = allocation,
            GadgetId = g.Id,
            Gadget = g
          });
        }
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
        allocation = await _context.Allocations
          .Include(r => r.ReferencePerson)
          .Include(g => g.AllocationGadgets).ThenInclude(ag => ag.Gadget).ThenInclude(g => g.SuppliedBy)
          .FirstOrDefaultAsync(i => i.Id == allocation.Id);
        allocation = await _context.Allocations.FindAsync(allocation.Id);
        var template = new NewAllocationTemplate(allocation, yourRequest);
        this._emailTrigger.SendEmail(template, allocation?.ReferencePerson?.Email, template.GetGroupEmails());
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
        var gadgets = _context.Gadgets.Include(e => e.SuppliedBy).Where(g => allocationsVM.GadgetsIds.Contains(g.Id));

        foreach (var a in allocations)
        {
          a.AllocationGadgets = new List<AllocationGagdet>();
          foreach (var g in gadgets)
          {
            a.AllocationGadgets.Add(new AllocationGagdet
            {
              AllocationId = a.Id,
              Allocation = a,
              GadgetId = g.Id,
              Gadget = g
            });
          }
        }
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
        this._emailTrigger.SendEmail(template, recipient, template.GetGroupEmails());
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
        allocation = await _context.Allocations
          .Include(o => o.Ressources)
          .Include(r => r.ReferencePerson)
          .Include(r => r.LastModifiedBy)
          .Include(g => g.AllocationGadgets).ThenInclude(ag => ag.Gadget).ThenInclude(g => g.SuppliedBy)
          .FirstOrDefaultAsync(i => i.Id == id);
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
      this._emailTrigger.SendEmail(template, allocation?.ReferencePerson?.Email, template.GetGroupEmails());
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
      string ressourceName;

      try
      {
        allocation = await _context.Allocations.Include(o => o.Ressources).Include(o => o.AllocationGadgets)
          .Include(o => o.LastModifiedBy).Include(o => o.ReferencePerson).AsNoTracking().FirstOrDefaultAsync(i => i.Id == editedRequest.Id);
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

        _context.Entry(allocation).State = EntityState.Modified;
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
          this._emailTrigger.SendEmail(template, allocation?.ReferencePerson?.Email, new List<string>());
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
        oldAllocation = await _context.Allocations
          .Include(o => o.Ressources)
          .Include(o => o.ReferencePerson)
          .Include(o => o.LastModifiedBy)
          .Include(o => o.AllocationGadgets).ThenInclude(ag => ag.Gadget).ThenInclude(g => g.SuppliedBy)
          .FirstOrDefaultAsync(i => i.Id == allocationVM.Id);
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

      oldAllocation.AllocationGadgets = oldAllocation.AllocationGadgets ?? new List<AllocationGagdet>();
      // Entfallene Hilfsmittel müssen aus allocationGadgets vom Allocation-Objekt entfernt werden
      // Neue Hilfsmittel müssen hinzugefügt werden
      var currentGadgetIds = oldAllocation.AllocationGadgets.Select(x => x.GadgetId);
      var newGadgets = allocationVM.GadgetsIds.Where(x => !currentGadgetIds.Contains(x));
      var droppedGadgets = currentGadgetIds.Where(x => !allocationVM.GadgetsIds.Contains(x)).ToArray();
      IQueryable<Gadget> newGadgetsObjects = new List<Gadget>().AsQueryable();
      var deletedGadgets = new List<AllocationGagdet>();
      var createdGadgets = new List<AllocationGagdet>();

      if (newGadgets.Any())
      {
        try
        {
          newGadgetsObjects = _context.Gadgets
            .Include(g => g.SuppliedBy)
            .Where(g => newGadgets.Contains(g.Id));

          foreach (var g in newGadgetsObjects)
          {
            var ag = new AllocationGagdet
            {
              AllocationId = oldAllocation.Id,
              Allocation = oldAllocation,
              GadgetId = g.Id,
              Gadget = g
            };
            createdGadgets.Add(ag);
            oldAllocation.AllocationGadgets.Add(ag);
          }
        }
        catch (Exception ex)
        {
          Log.Error(ex, "error while creating new gadgets to allocation");
        }
      }

      if (droppedGadgets.Any())
      {
        try
        {
          oldAllocation.AllocationGadgets.Where(x => droppedGadgets.Contains(x.GadgetId));

          foreach (var i in droppedGadgets)
          {
            var fuu = oldAllocation.AllocationGadgets.SingleOrDefault(x => x.GadgetId == i);
            deletedGadgets.Add(fuu);
            oldAllocation.AllocationGadgets.Remove(fuu);
          }
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
      var allGadgetGroups = oldAllocation.AllocationGadgets.Select(x => x.Gadget.SuppliedBy.GroupEmail).ToList();
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
      this._emailTrigger.SendEmail(template, oldAllocation?.ReferencePerson?.Email, template.GetGroupEmails());
      return Ok();
    }

    private void MapChangedOrNewRessources(Allocation oldAllocation, AllocationViewModel allocationVM)
    {
      var newRessources = _context.Ressources.Where(r => allocationVM.RessourceIds.Contains(r.Id));
      oldAllocation.Ressources.Clear();
      foreach (var ressource in newRessources)
        oldAllocation.Ressources.Add(ressource);
    }
  }
}
