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
using Serilog;

namespace Rema.WebApi.Controllers
{
  [Produces("application/json")]
  [Route("[controller]")]
  [ApiController]
  [AuthorizeAd("Reader")]
  public class SupplierGroupsController : BaseController
  {
    public SupplierGroupsController(RpDbContext context, IMapper mapper) : base(context, mapper)
    {
    }

    // GET: suppliergroups
    [HttpGet]
    public async Task<ActionResult<IEnumerable<SupplierGroup>>> GetSupplierGroups()
    {
      Log.Information("GET suppliergroups");

      try
      {
        return await _context.SupplierGroups.ToListAsync();
      }
      catch(Exception ex)
      {
        Log.Error(ex, "error while getting suppliergroups");
        return NotFound();
      }
    }

    // GET: suppliergroups/5
    [HttpGet("{id}")]
    public async Task<ActionResult<SupplierGroup>> GetSupplierGroup(long id)
    {
      Log.Information("GET suppliergroups/{id}", id);

      try
      {
        var supplierGroup = await _context.SupplierGroups.FindAsync(id);
        if (supplierGroup == null)
        {
          return NotFound();
        }
        return supplierGroup;
      }
      catch(Exception ex)
      {
        Log.Error(ex, "error while getting suppliergroup");
        return NotFound();
      }
    }

    // PUT: suppliergroups/5
    [HttpPut("{id}")]
    [AuthorizeAd("Admin")]
    public async Task<IActionResult> PutSupplierGroup(long id, SupplierGroup supplierGroup)
    {
      Log.Information("PUT suppliergroups/{id}: {supplierGroup}", id, supplierGroup);

      if (id != supplierGroup.Id)
      {
        return BadRequest();
      }

      try
      { 
        _context.Entry(supplierGroup).State = EntityState.Modified;
        await _context.SaveChangesAsync();
        return NoContent();
      }
      catch (Exception ex)
      {
        Log.Error(ex, "error while saving changed suppliergroup");
        return Conflict();
      }
    }

    // POST: suppliergroups
    [HttpPost]
    [AuthorizeAd("Admin")]
    public async Task<ActionResult<SupplierGroup>> PostSupplierGroup(SupplierGroup supplierGroup)
    {
      Log.Information("POST suppliergroups: {supplierGroup}", supplierGroup);

      try
      {
        _context.SupplierGroups.Add(supplierGroup);
        await _context.SaveChangesAsync();
        return CreatedAtAction("GetSupplierGroup", new { id = supplierGroup.Id }, supplierGroup);
      }
      catch(Exception ex)
      {
        Log.Error(ex, "error while saving new suppliergroup");
        return Conflict();
      }
    }

    // DELETE: suppliergroups/5
    [HttpDelete("{id}")]
    [AuthorizeAd("Admin")]
    public async Task<ActionResult<SupplierGroup>> DeleteSupplierGroup(long id)
    {
      Log.Information("DELETE suppliergroup/{id}", id);

      SupplierGroup supplierGroup;

      try
      {
        supplierGroup = await _context.SupplierGroups.FindAsync(id);
        if (supplierGroup == null)
        {
          return NotFound();
        }
      }
      catch(Exception ex)
      {
        Log.Error(ex, "error while getting suppliergroup");
        return NotFound();
      }

      try
      {
        _context.SupplierGroups.Remove(supplierGroup);
        await _context.SaveChangesAsync();
        return NoContent();
      }
      catch(Exception ex)
      {
        Log.Error(ex, "error while remoning suppliergrup");
        return Conflict();
      }
    }
  }
}
