using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Rema.DbAccess;
using Rema.Infrastructure.Email;
using Rema.Infrastructure.Models;
using Rema.WebApi.Filter;
using Rema.WebApi.ViewModels;
using Serilog;

namespace Rema.WebApi.Controllers
{
  [Authorize]
  [Produces("application/json")]
  [Route("[controller]")]
  [ApiController]
  [AuthorizeAd("Reader")]
  public class AllocationsController : BaseController
  {
    public AllocationsController(RpDbContext context, IMapper mapper) : base(context, mapper)
    {
    }

    // GET: allocations
    [HttpGet]
    public async Task<ActionResult<IEnumerable<AllocationViewModel>>> GetAllocations()
    {
      Log.Information("GET allocations");

      List<Allocation> allocations;

      try
      {
        allocations = await _context.Allocations
          .Include(g => g.Ressource)
          .Include(g => g.ApprovedBy)
          .Include(g => g.CreatedBy)
          .Include(g => g.AllocationGadgets)
          .Include(g => g.LastModifiedBy)
          .Include(g => g.ReferencePerson)
          .ToListAsync();
      }
      catch (Exception ex)
      {
        Log.Error(ex, "error while getting allocations");
        return NotFound();
      }

      try
      {
        var allMapped = allocations.Select(e => _mapper.Map<Allocation, AllocationViewModel>(e));
        //var allMapped = all.Select(e => _mapper.Map<Allocation, AllocationViewModel>(e));
        /*
        var p = (from a in _context.Allocations
                 select new
                 {
                   Id = a.Id,
                   From = a.From,
                   To = a.To,
                   IsAllDay = a.IsAllDay,
                   Status = a.Status,
                   Ressource_id = a.Ressource.Id,
                   CreatedBy = a.CreatedBy.Id,
                   CreatedAt = a.CreatedAt,
                   LastModified = a.LastModified,
                   LastModifiedBy = a.LastModifiedBy.Id,
                   ApprovedBy = a.ApprovedBy.Id,
                   ApprovedAt = a.ApprovedAt,
                   ReferencePerson = a.ReferencePerson.Id
                 }).ToList();
        */
        return Ok(allMapped);
      }
      catch (Exception ex)
      {
        Log.Error(ex, "error while mapping allocations");
        return NotFound();
      }
    }

    // GET: allocations/5
    [HttpGet("{id}")]
    public async Task<ActionResult<Allocation>> GetAllocation(long id)
    {
      Log.Information("GET allocation/{id}", id);

      try
      {
        var allocation = await _context.Allocations.FindAsync(id);
        if (allocation == null)
        {
          return NotFound();
        }
        return Ok(allocation);
      }
      catch (Exception ex)
      {
        Log.Error(ex, "error while getting allocation");
        return NotFound();
      }
    }

    // PUT: allocations/5
    [HttpPut("{id}")]
    public async Task<IActionResult> PutAllocation(long id, Allocation allocation)
    {
      Log.Information("PUT allocations/{id}: {allocation}", id, allocation);

      if (id != allocation.Id)
      {
        Log.Error("allocation not mached the id");
        return BadRequest();
      }

      try
      {
        allocation.LastModified = DateTime.Now;
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
    }

    // POST: allocations
    [HttpPost]
    public async Task<ActionResult<AllocationViewModel>> PostAllocation(AllocationViewModel allocationVM)
    {
      Log.Information("POST allocation: {allocation}", allocationVM);

      Allocation allocation;
      User requestedUser;

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
        allocation = _mapper.Map<AllocationViewModel, Allocation>(allocationVM);
      }
      catch (Exception ex)
      {
        Log.Error(ex, "error while mapping allocation");
        return BadRequest();
      }

      try
      {
        allocation.Ressource = await _context.Ressources.FindAsync(allocationVM.Ressource_id);
      }
      catch (Exception ex)
      {
        Log.Error(ex, "error while getting ressource");
      }

      try
      {
        var gadgets = _context.Gadgets.Where(g => allocationVM.GadgetsIds.Contains(g.Id));

        allocation.AllocationGadgets = new List<AllocationGagdet>();
        foreach(var g in gadgets)
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
      catch(Exception ex)
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
        if (allocationVM.ReferencePerson_id == 0)
        {
          allocation.ReferencePerson = requestedUser;
        }
        else
        {
          var referencePerson = await _context.Users.FindAsync(allocationVM.ReferencePerson_id);
          allocation.ReferencePerson = referencePerson;
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

      try
      {
        if (allocation.Status >= MeetingStatus.Approved && base.RequestSenderVM.Roles.Exists(e => e.HasRole(Startup.Editor)))
        {
          allocation.Status = allocationVM.Status;
          EmailTrigger.SendEmail(
            "Buchung wurde erstellt",
            $"Ihre Buchungsanfrage {allocation.Title} der Ressource {allocation.Ressource.Name} " +
            $"vom {allocation.From} bis {allocation.To} wurde vorgenommen",
            recipient: base.RequestSender.Email);
        }
        else
        {
          allocation.Status = MeetingStatus.Pending;
          EmailTrigger.SendEmail(
            "Anfrage wurde erstellt",
            $"Ihre Buchungsanfrage {allocation.Title} der Ressource {allocation.Ressource.Name} " +
            $"vom {allocation.From} bis {allocation.To} wurde gestellt", recipient: base.RequestSender.Email);
        }
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

    // DELETE: allocations/5
    [HttpDelete("{id}")]
    public async Task<ActionResult<Allocation>> DeleteAllocation(long id)
    {
      Allocation allocation;

      Log.Information("DELETE allocations/{id}", id);

      try
      {
        allocation = await _context.Allocations
          .Include(o => o.Ressource)
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

      try
      {
        _context.Allocations.Remove(allocation);
        await _context.SaveChangesAsync();
      }
      catch (Exception ex)
      {
        Log.Error(ex, "error while deleting allocation");
      }

      return Ok();
    }

    [HttpGet("filter/{filter}")]
    public async Task<ActionResult<IEnumerable<object>>> GetAllocations(AllocationFilter filter)
    {
      //TODO: Aktuell wird hier garnicht gefiltert

      Log.Information("GET allocations/filter/{filter}", filter);

      try
      {
        var all = await _context.Allocations
          .Include(g => g.Ressource)
          .Include(g => g.ApprovedBy)
          .Include(g => g.CreatedBy)
          .Include(g => g.LastModifiedBy)
          .Include(g => g.ReferencePerson)
          .ToListAsync();

        // return all.Select(e => _mapper.Map<Allocation, AllocationViewModel>(e));
        var p = (from a in _context.Allocations
                 select new
                 {
                   Id = a.Id,
                   From = a.From,
                   To = a.To,
                   IsAllDay = a.IsAllDay,
                   Status = a.Status,
                   Ressource_id = a.Ressource.Id,
                   CreatedBy = a.CreatedBy,
                   CreatedAt = a.CreatedAt,
                   LastModified = a.LastModified,
                   LastModifiedBy = a.LastModifiedBy.Id,
                   ApprovedBy = a.ApprovedBy.Id,
                   ApprovedAt = a.ApprovedAt,
                   ReferencePerson = a.ReferencePerson.Id
                 }).ToList();
        return Ok(p);
      }
      catch (Exception ex)
      {
        Log.Error(ex, "error while getting filtered allocation");
        return NotFound();
      }
    }

    // PUT: allocations/status/5
    [HttpPut("status/{id}")]
    public async Task<IActionResult> PutAllocation(long id, int status)
    {
      Log.Information("PUT allocations/status/{id}: {status}", id, status);

      Allocation allocation;

      try
      {
        allocation = await _context.Allocations.FindAsync(id);
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

      try
      {
        allocation.Status = (MeetingStatus)status;
        allocation.LastModified = DateTime.Now;
        allocation.LastModifiedBy = base.RequestSender;
        _context.Entry(allocation).State = EntityState.Modified;

        await _context.SaveChangesAsync();
      }
      catch (Exception ex)
      {
        Log.Error(ex, "error while changing status and save allocation");
        return Conflict();
      }

      return Ok();
    }

    // PUT: allocations/editrequest
    [HttpPut("{editedRequest}")]
    public async Task<ActionResult<Boolean>> EditRequest(AllocationRequestEdition editedRequest)
    {
      Log.Information("PUT allocations/editrequest: {editRequest}", editedRequest);

      Allocation allocation;

      try
      {
        allocation = await _context.Allocations
          .Include(o => o.Ressource)
          .FirstOrDefaultAsync(i => i.Id == editedRequest.Id);
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

      try
      {
        allocation.Status = (MeetingStatus)editedRequest.status;
        allocation.LastModified = DateTime.Now;
        allocation.LastModifiedBy = base.RequestSender;
        allocation.ApprovedAt = DateTime.Now;
        allocation.ApprovedBy = base.RequestSender;

        if ((MeetingStatus)editedRequest.status == MeetingStatus.Moved)
        {
          allocation.From = editedRequest.From.GetValueOrDefault();
          allocation.To = editedRequest.To.GetValueOrDefault();
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
        if ((MeetingStatus)editedRequest.status == MeetingStatus.Moved)
        {
          EmailTrigger.SendEmail("Buchung wurde verschoben", $"Ihre Buchung {allocation.Title} der Ressource {allocation.Ressource.Name} vom {allocation.From} bis {allocation.To} wurde verschoben von {base.RequestSender.Name}", recipient: base.RequestSender.Email);
        }
        else if ((MeetingStatus)editedRequest.status == MeetingStatus.Approved)
        {
          EmailTrigger.SendEmail("Buchung wurde genehmigt", $"Ihre Buchungsanfrage {allocation.Title} der Ressource {allocation.Ressource.Name} vom {allocation.From} bis {allocation.To} wurde genehmigt von {base.RequestSender.Name}", recipient: base.RequestSender.Email);
        }
        else if ((MeetingStatus)editedRequest.status == MeetingStatus.Clarification)
        {
          EmailTrigger.SendEmail("Buchung wurde abgelehnt", $"Ihre Buchungsanfrage {allocation.Title} der Ressource {allocation.Ressource.Name} vom {allocation.From} bis {allocation.To} wurde abgelehnt", recipient: base.RequestSender.Email);
        }
      }

      catch (Exception ex)
      {
        Log.Error(ex, "error while processing mails");
      }

      return Ok();
    }
  }
}
