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
using Rema.Infrastructure.Models;
using Rema.WebApi.Filter;
using Rema.WebApi.MappingProfiles;
using Rema.WebApi.ViewModels;
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
    public async Task<ActionResult<IEnumerable<RessourceViewModel>>> GetRessources()
    {
      Log.Information("GET ressources");
      try
      {
        var ressources = await _context.Ressources.ToListAsync();
        var ressourceVMs = ressources.Select<Ressource, RessourceViewModel>((e) => 
          _mapper.Map<Ressource, RessourceViewModel>(e)).ToList();
        return Ok(ressourceVMs);
      }
      catch(Exception ex)
      {
        Log.Error(ex, "error while getting ressources");
        return Conflict();
      }
    }

    // GET: ressources/5
    [HttpGet("{id}")]
    public async Task<ActionResult<RessourceViewModel>> GetRessource(long id)
    {
      try
      {
        var ressource = await _context.Ressources.FindAsync(id);
        if (ressource == null)
        {
          return NotFound();
        }
        var ressourceView = _mapper.Map<RessourceViewModel>(ressource);
        return Ok(ressourceView);
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
    public async Task<IActionResult> PutRessource(long id, RessourceViewModel ressource)
    {
      Log.Information("PUT ressource/{id}: {ressource}", id, ressource);
      if (id != ressource.Id)
      {
        return BadRequest();
      }

      var ressourceObj = _mapper.Map<RessourceViewModel, Ressource>(ressource);
      try
      {
        _context.Entry(ressourceObj).State = EntityState.Modified;
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
    public async Task<ActionResult<RessourceViewModel>> PostRessource(RessourceViewModel ressource)
    {
      Log.Information("POST ressources: {ressource}", ressource);

      var ressourceObj = _mapper.Map<RessourceViewModel, Ressource>(ressource);
      try
      {
        _context.Ressources.Add(ressourceObj);
        await _context.SaveChangesAsync();
        return CreatedAtAction("GetRessource", new { id = ressource.Id }, ressourceObj);
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
    public async Task<ActionResult> DeleteRessource(long id)
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
