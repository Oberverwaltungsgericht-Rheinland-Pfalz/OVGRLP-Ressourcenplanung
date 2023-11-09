// SPDX-FileCopyrightText: © 2019 Oberverwaltungsgericht Rheinland-Pfalz <poststelle@ovg.jm.rlp.de>
// SPDX-License-Identifier: EUPL-1.2
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
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
  [Produces("application/json")]
  [Route("[controller]")]
  [ApiController]
  [AuthorizeAd("Reader")]
  public class GadgetsController : BaseController
  {
    public GadgetsController(RpDbContext context, IMapper mapper) : base(context, mapper)
    {
    }

    // GET: gadgets
    [HttpGet]
    public async Task<ActionResult<IEnumerable<GadgetViewModel>>> GetGadgets()
    {
      Log.Information("GET gadgets");

      try
      {
        var gadgets = await _context.Gadgets.Include(g => g.SuppliedBy).ToListAsync();
        var gadgetVMs = gadgets.Select<Gadget, GadgetViewModel>((e) => _mapper.Map<Gadget, GadgetViewModel>(e)).ToList();
        return Ok(gadgetVMs);
      }
      catch(Exception ex)
      {
        Log.Error(ex, "error while getting gadgets");
        return NotFound();
      }
    }

    // GET: gadgets/5
    [HttpGet("{id}")]
    public async Task<ActionResult<GadgetViewModel>> GetGadget(long id)
    {
      Log.Information("GET gadgets/{id}", id);

      try
      {
        var gadget = await _context.Gadgets.FindAsync(id);

        if (gadget == null)
          return NotFound();

        return Ok(_mapper.Map<Gadget, GadgetViewModel>(gadget));
      }
      catch(Exception ex)
      {
        Log.Error(ex, "error while getting gedget");
        return NotFound();
      }
    }

    // POST: gadgets
    [HttpPost]
    [AuthorizeAd("Admin")]
    public async Task<ActionResult<GadgetViewModel>> PostGadget(GadgetViewModel gadgetVM)
    {
      Log.Information("POST gadgets: {gadget}", gadgetVM);

      Gadget gadget;

      try
      { 
        gadget = new Gadget()
        {
          Id = gadgetVM.Id,
          Title = gadgetVM.Title,
          SuppliedBy = await _context.SupplierGroups.FindAsync(gadgetVM.SuppliedBy)
        };
      }
      catch(Exception ex)
      {
        Log.Error(ex, "error while mapping gadget");
        return Conflict();
      }

      try
      {
        _context.Gadgets.Add(gadget);
        await _context.SaveChangesAsync();
      }
      catch(Exception ex)
      {
        Log.Error(ex, "wrror while saving gadget");
        return Conflict();
      }

      try
      {
        var returnGadget = _mapper.Map<Gadget, GadgetViewModel>(gadget);
        return CreatedAtAction("GetGadget", new { id = gadget.Id }, returnGadget);
      }
      catch(Exception ex)
      {
        Log.Error(ex, "error while mapping return gadget");
        return Conflict();
      }
    }

    // PUT: gadgets/5
    [HttpPut("{id}")]
    [AuthorizeAd("Admin")]
    public async Task<IActionResult> PutGadget(long id, GadgetViewModel gadgetVM)
    {
      Log.Information("PUT gadgets/{id}: {gadget}", id, gadgetVM);

      if(id != gadgetVM.Id)
      {
        return BadRequest();
      }

      Gadget gadget;

      try
      {
        gadget = new Gadget()
        {
          Id = gadgetVM.Id,
          Title = gadgetVM.Title,
          IsDeactivated = gadgetVM.IsDeactivated,
          SuppliedBy = await _context.SupplierGroups.FindAsync(gadgetVM.SuppliedBy)
        };
      }
      catch (Exception ex)
      {
        Log.Error(ex, "error while mapping gadget");
        return Conflict();
      }

      try
      {
        _context.Entry(gadget).State = EntityState.Modified;
        await _context.SaveChangesAsync();
      }
      catch (Exception ex)
      {
        Log.Error(ex, "error while saving gadget");
        return Conflict();
      }

      return Ok();
    }

    // DELETE: gadgets/5
    [HttpDelete("{id}")]
    [AuthorizeAd("Admin")]
    public async Task<ActionResult<Gadget>> DeleteGadget(long id)
    {
      Log.Information("DELETE gadgets/{id}", id);

      Gadget gadget;

      try
      {
        gadget = await _context.Gadgets.FindAsync(id);
        if (gadget == null)
        {
          return NotFound();
        }
      }
      catch(Exception ex)
      {
        Log.Error(ex, "error while getting gadget");
        return NotFound();
      }
      
      try
      {
        _context.Gadgets.Remove(gadget);
        await _context.SaveChangesAsync();
        return NoContent();
      }
      catch(Exception ex)
      {
        Log.Error(ex, "error while removing gadget");
        return Conflict();
      }
    }
  }
}
