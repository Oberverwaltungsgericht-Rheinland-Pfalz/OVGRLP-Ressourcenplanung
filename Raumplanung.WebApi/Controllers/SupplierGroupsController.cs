using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using AutoMapper;
using Serilog;
using Raumplanung.WebApi.Filter;
using Raumplanung.Data.DataAccess;
using Raumplanung.Infrastructure.Models;

namespace Raumplanung.WebApi.Controllers
{
  [Produces("application/json")]
  [Route("api/[controller]")]
  [ApiController]
  [AuthorizeAd("Reader")]
  public class SupplierGroupsController : BaseController
  {
    public SupplierGroupsController(RpDbContext context, IMapper mapper) : base(context, mapper)
    {
    }

    // GET: api/SupplierGroups
    [HttpGet]
    public async Task<ActionResult<IEnumerable<SupplierGroup>>> GetSupplierGroups()
    {
      return await _context.SupplierGroups.ToListAsync();
    }

    // GET: api/SupplierGroups/5
    [HttpGet("{id}")]
    public async Task<ActionResult<SupplierGroup>> GetSupplierGroup(long id)
    {
      var supplierGroup = await _context.SupplierGroups.FindAsync(id);

      if (supplierGroup == null)
      {
        return NotFound();
      }

      return supplierGroup;
    }

    // PUT: api/SupplierGroups/5
    [HttpPut("{id}")]
    [AuthorizeAd("Admin")]
    public async Task<IActionResult> PutSupplierGroup(long id, SupplierGroup supplierGroup)
    {
      if (id != supplierGroup.Id)
      {
        return BadRequest();
      }

      _context.Entry(supplierGroup).State = EntityState.Modified;

      try
      {
        await _context.SaveChangesAsync();
      }
      catch (DbUpdateConcurrencyException)
      {
        if (!SupplierGroupExists(id))
        {
          return NotFound();
        }
        else
        {
          throw;
        }
      }

      Log.Information("Supplier Group {@group.IdTitle} was updated by {@User.email}", supplierGroup.Id + supplierGroup.Title, base.RequestSender.Email);
      return NoContent();
    }

    // POST: api/SupplierGroups
    [HttpPost]
    [AuthorizeAd("Admin")]
    public async Task<ActionResult<SupplierGroup>> PostSupplierGroup(SupplierGroup supplierGroup)
    {
      _context.SupplierGroups.Add(supplierGroup);
      await _context.SaveChangesAsync();

      Log.Information("Supplier Group {@group.IdTitle} was inserted by {@User.email}", supplierGroup.Id + supplierGroup.Title, base.RequestSender.Email);
      return CreatedAtAction("GetSupplierGroup", new { id = supplierGroup.Id }, supplierGroup);
    }

    // DELETE: api/SupplierGroups/5
    [HttpDelete("{id}")]
    [AuthorizeAd("Admin")]
    public async Task<ActionResult<SupplierGroup>> DeleteSupplierGroup(long id)
    {
      var supplierGroup = await _context.SupplierGroups.FindAsync(id);
      if (supplierGroup == null)
      {
        return NotFound();
      }

      _context.SupplierGroups.Remove(supplierGroup);
      await _context.SaveChangesAsync();

      Log.Information("Supplier Group {@group.IdTitle} was deleted by {@User.email}", supplierGroup.Id + supplierGroup.Title, base.RequestSender.Email);
      return supplierGroup;
    }

    private bool SupplierGroupExists(long id)
    {
      return _context.SupplierGroups.Any(e => e.Id == id);
    }
  }
}