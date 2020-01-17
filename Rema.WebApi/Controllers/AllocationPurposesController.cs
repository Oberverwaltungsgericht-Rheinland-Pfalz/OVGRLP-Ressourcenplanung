using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Rema.DbAccess;
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
  public class AllocationPurposesController : BaseController
  {
    public AllocationPurposesController(RpDbContext context, IMapper mapper) : base(context, mapper)
    {
    }

    // GET: allocationpurposes
    [HttpGet]
    public async Task<ActionResult<IEnumerable<object>>> GetAllocationPurposes()
    {
      Log.Information("GET: allocationpurposes");

      try
      {
        return await (from purpose in _context.AllocationPurposes
                      select new
                      {
                        Id = purpose.Id,
                        Title = purpose.Title,
                        Description = purpose.Description,
                        Notes = purpose.Notes,
                        ContactPhone = purpose.ContactPhone,
                        Gadgets = purpose.Gadgets.Select(x => x.GadgetId),
                        Allocations = purpose.Allocations.Select(x => x.Id)
                      }).ToListAsync();
      }
      catch (Exception ex)
      {
        Log.Error(ex, "error while getting allocationpurposes");
        return NotFound();
      }
    }

    // GET: allocationpurposes/5
    [HttpGet("{id}")]
    public async Task<ActionResult<AllocationPurposeViewModel>> GetAllocationPurpose(long id)
    {
      Log.Information("GET: allocationpurposes/{id}", id);

      try
      {
        var allocationPurpose = await _context.AllocationPurposes.FindAsync(id);

        if (allocationPurpose == null)
        {
          return NotFound();
        }

        var purposeVM = _mapper.Map<AllocationPurpose, AllocationPurposeViewModel>(allocationPurpose);
        return purposeVM;
      }
      catch (Exception ex)
      {
        Log.Error(ex, "error while getting allocationpurpose");
        return NotFound();
      }
    }

    // PUT: allocationpurposes/5
    [HttpPut("{id}")]
    [AuthorizeAd("Reader")]
    public async Task<IActionResult> PutAllocationPurpose(long id, AllocationPurpose allocationPurpose)
    {
      Log.Information("PUT allocationpurposes({id}: {allocationPurpose}", id, allocationPurpose);

      if (id != allocationPurpose.Id)
      {
        return BadRequest();
      }

      try
      {
        _context.Entry(allocationPurpose).State = EntityState.Modified;
        await _context.SaveChangesAsync();
      }
      catch (Exception ex)
      {
        Log.Error(ex, "error while save allocationpurpose");
        return NotFound();
      }

      return Ok();
    }

    // POST: allocationpurposes
    [HttpPost]
    public async Task<ActionResult<AllocationPurpose>> PostAllocationPurpose(AllocationPurposeViewModel allocationPurpose)
    {
      Log.Information("POST allocationpurposes: {allocationPurpose}", allocationPurpose);
      AllocationPurpose purpose;

      try
      {
        purpose = _mapper.Map<AllocationPurposeViewModel, AllocationPurpose>(allocationPurpose);
      }
      catch (Exception mapEx)
      {
        Log.Error(mapEx, "error while map allocationpurpose");
        return BadRequest();
      }

      try
      {
        _context.AllocationPurposes.Add(purpose);
      }
      catch (Exception saveEx)
      {
        Log.Error(saveEx, "error while save allocationpurpose");
        return NotFound();
      }

      try
      {
        if (allocationPurpose.GadgetIds.Any())
        {
          var tasksGadgetIds = allocationPurpose.GadgetIds.Select(e => _context.Gadgets.FindAsync(e).AsTask());
          var gadgets = await Task.WhenAll(tasksGadgetIds);

          foreach (var gadget in gadgets)
          {
            var gadgetPurpose = new GadgetPurpose(gadget, purpose);
            _context.GadgetPurposes.Add(gadgetPurpose);
            purpose.Gadgets.Add(gadgetPurpose);
            gadget.AllocationPurposes.Add(gadgetPurpose);
          }
        }
      }
      catch (Exception ex)
      {
        Log.Error(ex, "error while getting gadgets for allocationpurpose");
        return NotFound();
      }

      try
      {
        await _context.SaveChangesAsync();
      }
      catch (Exception ex)
      {
        Log.Error(ex, "error while save allocationpurpose");
        return NotFound();
      }

      try
      {
        var returnPurpose = _mapper.Map<AllocationPurpose, AllocationPurposeViewModel>(purpose);
        return CreatedAtAction("GetAllocationPurpose", new { id = returnPurpose.Id }, returnPurpose);
      }
      catch (Exception ex)
      {
        Log.Error(ex, "error while map save allocationpurpose");
        return Conflict();
      }
    }

    // DELETE: allocationpurposes/5
    [HttpDelete("{id}")]
    public async Task<IActionResult> DeleteAllocationPurpose(long id)
    {
      Log.Information("DELETE allocationpurposes/{id}", id);

      var allocationPurpose = await _context.AllocationPurposes.FindAsync(id);
      if (allocationPurpose == null)
      {
        return NotFound();
      }

      try
      {
        _context.AllocationPurposes.Remove(allocationPurpose);
        await _context.SaveChangesAsync();
      }
      catch(Exception ex)
      {
        Log.Error(ex, "error while delete allocationpurpose");
        return Conflict();
      }

      return Ok();
    }
  }
}
