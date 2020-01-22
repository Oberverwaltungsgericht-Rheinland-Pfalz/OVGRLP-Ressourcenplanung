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
  public class RessourcesController : BaseController
  {
    public RessourcesController(RpDbContext context, IMapper mapper) : base(context, mapper)
    {
    }

    // GET: ressources
    [HttpGet]
    public async Task<ActionResult<IEnumerable<Ressource>>> GetRessources()
    {
      Log.Information("GET ressources");
      try
      {
        return await _context.Ressources.ToListAsync();
      }
      catch(Exception ex)
      {
        Log.Error(ex, "error while getting ressources");
        return Conflict();
      }
    }

    // GET: ressources/5
    [HttpGet("{id}")]
    public async Task<ActionResult<Ressource>> GetRessource(long id)
    {
      try
      {
        var ressource = await _context.Ressources.FindAsync(id);
        if (ressource == null)
        {
          return NotFound();
        }
        return Ok(ressource);
      }
      catch(Exception ex)
      {
        Log.Error(ex, "error while getting ressource");
        return NotFound();
      }
    }

    // PUT: ressources/5
    [HttpPut("{id}")]
    [AuthorizeAd("Admin")]
    public async Task<IActionResult> PutRessource(long id, Ressource ressource)
    {
      Log.Information("PUT ressource/{id}: {ressource}", id, ressource);
      if (id != ressource.Id)
      {
        return BadRequest();
      }

      try
      {
        _context.Entry(ressource).State = EntityState.Modified;
        await _context.SaveChangesAsync();
        return NoContent();
      }
      catch (Exception ex)
      {
        Log.Error(ex, "error while saving changed ressource");
        return Conflict();
      }
    }

    // POST: ressources
    [HttpPost]
    [AuthorizeAd("Admin")]
    public async Task<ActionResult<Ressource>> PostRessource(Ressource ressource)
    {
      Log.Information("POST ressources: {ressource}", ressource);

      try
      {
        _context.Ressources.Add(ressource);
        await _context.SaveChangesAsync();
        return CreatedAtAction("GetRessource", new { id = ressource.Id }, ressource);
      }
      catch(Exception ex)
      {
        Log.Error(ex, "error while saving new ressource");
        return Conflict();
      }
    }

    // DELETE: ressources/5
    [HttpDelete("{id}")]
    [AuthorizeAd("Admin")]
    public async Task<ActionResult<Ressource>> DeleteRessource(long id)
    {
      Log.Information("DELETE ressources/{id}", id);

      Ressource ressource;

      try
      {
        ressource = await _context.Ressources.FindAsync(id);
        if (ressource == null)
        {
          return NotFound();
        }
      }
      catch(Exception ex)
      {
        Log.Error(ex, "error while getting ressource");
        return NotFound();
      }

      try
      {
        _context.Ressources.Remove(ressource);
        await _context.SaveChangesAsync();
        return NoContent();
      }
      catch(Exception ex)
      {
        Log.Error(ex, "error while removing ressource");
        return Conflict();
      }
    }
  }
}
