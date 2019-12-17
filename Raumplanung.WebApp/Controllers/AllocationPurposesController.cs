﻿using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Raumplanung.Data.DataAccess;
using Raumplanung.Infrastructure.Models;
using Raumplanung.WebApp.Filter;
using Raumplanung.WebApp.ViewModels;
using Serilog;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Raumplanung.WebApp.Controllers
{
  [Produces("application/json")]
  [Route("api/[controller]")]
  [ApiController]
  [AuthorizeAd("Reader")]
  public class AllocationPurposesController : BaseController
  {
    public AllocationPurposesController(RpDbContext context, IMapper mapper) : base(context, mapper)
    {
    }

    // GET: api/AllocationPurposes
    [HttpGet]
    public async Task<ActionResult<IEnumerable<object>>> GetAllocationPurposes()
    {
      //var purposes = await _context.AllocationPurposes.Include(g => g.Allocations).ToListAsync();
      var p = await (from purpose in _context.AllocationPurposes
                       //   where purpose.Allocations.Count > 0
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
      return p;
    }

    // GET: api/AllocationPurposes/5
    [HttpGet("{id}")]
    public async Task<ActionResult<AllocationPurposeViewModel>> GetAllocationPurpose(long id)
    {
      var allocationPurpose = await _context.AllocationPurposes.FindAsync(id);

      if (allocationPurpose == null)
      {
        return NotFound();
      }
      var purposeVM = _mapper.Map<AllocationPurpose, AllocationPurposeViewModel>(allocationPurpose);
      return purposeVM;
    }

    // PUT: api/AllocationPurposes/5
    [HttpPut("{id}")]
    [AuthorizeAd("Reader")]
    public async Task<IActionResult> PutAllocationPurpose(long id, AllocationPurpose allocationPurpose)
    {
      if (id != allocationPurpose.Id)
      {
        return BadRequest();
      }

      _context.Entry(allocationPurpose).State = EntityState.Modified;

      try
      {
        await _context.SaveChangesAsync();
      }
      catch (DbUpdateConcurrencyException)
      {
        if (!AllocationPurposeExists(id))
        {
          return NotFound();
        }
        else
        {
          throw;
        }
      }

      Log.Information("Allocation Purpose {@purpose.IdTitle} was updated by {@User.email}", allocationPurpose.Id + allocationPurpose.Title, base.RequestSender.Email);
      return NoContent();
    }

    // POST: api/AllocationPurposes
    [HttpPost]
    public async Task<ActionResult<AllocationPurpose>> PostAllocationPurpose(AllocationPurposeViewModel allocationPurpose)
    {
      var purpose = _mapper.Map<AllocationPurposeViewModel, AllocationPurpose>(allocationPurpose);

      _context.AllocationPurposes.Add(purpose);

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

      await _context.SaveChangesAsync();
      var returnPurpose = _mapper.Map<AllocationPurpose, AllocationPurposeViewModel>(purpose);

      Log.Information("Allocation Purpose {@purpose.IdTitle} was created by {@User.email}", allocationPurpose.Id + allocationPurpose.Title, base.RequestSender.Email);
      return CreatedAtAction("GetAllocationPurpose", new { id = returnPurpose.Id }, returnPurpose);
    }

    // DELETE: api/AllocationPurposes/5
    [HttpDelete("{id}")]
    public async Task<ActionResult<AllocationPurpose>> DeleteAllocationPurpose(long id)
    {
      var allocationPurpose = await _context.AllocationPurposes.FindAsync(id);
      if (allocationPurpose == null)
      {
        return NotFound();
      }

      _context.AllocationPurposes.Remove(allocationPurpose);
      await _context.SaveChangesAsync();

      Log.Information("Allocation Purpose {@purpose.IdTitle} was deleted by {@User.email}", allocationPurpose.Id + allocationPurpose.Title, base.RequestSender.Email);
      return allocationPurpose;
    }

    private bool AllocationPurposeExists(long id)
    {
      return _context.AllocationPurposes.Any(e => e.Id == id);
    }
  }
}