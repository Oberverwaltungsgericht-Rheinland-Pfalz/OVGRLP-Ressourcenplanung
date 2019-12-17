using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using AutoMapper;
using Serilog;
using Raumplanung.WebApp.Filter;
using Raumplanung.Data.DataAccess;
using Raumplanung.Infrastructure.Models;

namespace Raumplanung.WebApp.Controllers
{
  [Produces("application/json")]
  [Route("api/[controller]")]
  [ApiController]
  [AuthorizeAd("Reader")]
  public class RessourcesController : BaseController
  {
    public RessourcesController(RpDbContext context, IMapper mapper) : base(context, mapper)
    {
    }

    // GET: api/Ressources
    [HttpGet]
    public async Task<ActionResult<IEnumerable<Ressource>>> GetRessources()
    {
      return await _context.Ressources.ToListAsync();
    }

    // GET: api/Ressources/5
    [HttpGet("{id}")]
    public async Task<ActionResult<Ressource>> GetRessource(long id)
    {
      var ressource = await _context.Ressources.FindAsync(id);

      if (ressource == null)
      {
        return NotFound();
      }

      return ressource;
    }

    // PUT: api/Ressources/5
    [HttpPut("{id}")]
    [AuthorizeAd("Admin")]
    public async Task<IActionResult> PutRessource(long id, Ressource ressource)
    {
      if (id != ressource.Id)
      {
        return BadRequest();
      }

      _context.Entry(ressource).State = EntityState.Modified;

      try
      {
        await _context.SaveChangesAsync();
      }
      catch (DbUpdateConcurrencyException)
      {
        if (!RessourceExists(id))
        {
          return NotFound();
        }
        else
        {
          throw;
        }
      }

      Log.Information("Ressource {@ressource.IdTitle} was updated by {@User.email}", ressource.Id + ressource.Name, base.RequestSender.Email);
      return NoContent();
    }

    // POST: api/Ressources
    [HttpPost]
    [AuthorizeAd("Admin")]
    public async Task<ActionResult<Ressource>> PostRessource(Ressource ressource)
    {
      _context.Ressources.Add(ressource);
      await _context.SaveChangesAsync();

      Log.Information("Ressource {@ressource.IdTitle} was inserted by {@User.email}", ressource.Id + ressource.Name, base.RequestSender.Email);
      return CreatedAtAction("GetRessource", new { id = ressource.Id }, ressource);
    }

    // DELETE: api/Ressources/5
    [HttpDelete("{id}")]
    [AuthorizeAd("Admin")]
    public async Task<ActionResult<Ressource>> DeleteRessource(long id)
    {
      var ressource = await _context.Ressources.FindAsync(id);
      if (ressource == null)
      {
        return NotFound();
      }

      _context.Ressources.Remove(ressource);
      await _context.SaveChangesAsync();

      Log.Information("Ressource {@ressource.IdTitle} was deleted by {@User.email}", ressource.Id + ressource.Name, base.RequestSender.Email);
      return ressource;
    }

    private bool RessourceExists(long id)
    {
      return _context.Ressources.Any(e => e.Id == id);
    }
  }
}