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
using Rema.ServiceLayer.Interfaces;
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
    public ISupplierGroupsService _groupsService;
    public SupplierGroupsController(RpDbContext context, IMapper mapper, ISupplierGroupsService groupsService) : base(context, mapper)
    {
      this._groupsService = groupsService;
    }

    // GET: suppliergroups
    [HttpGet]
    public async Task<ActionResult<IEnumerable<SupplierGroup>>> GetSupplierGroups()
    {
      Log.Information("GET suppliergroups");
      var groups = await _groupsService.GetSupplierGroups();
      if (groups == null) return NotFound();
      return groups.ToList();
    }

    // GET: suppliergroups/5
    [HttpGet("{id}")]
    public async Task<ActionResult<SupplierGroup>> GetSupplierGroup(long id)
    {
      Log.Information("GET suppliergroups/{id}", id);

      var group = await _groupsService.GetSupplierGroup(id);
      if (group == null) return NotFound();
      return group;
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
      var success = await _groupsService.PutSupplierGroup(supplierGroup);
      if (success) return NoContent();
      else return Conflict();
    }

    // POST: suppliergroups
    [HttpPost]
    [AuthorizeAd("Admin")]
    public async Task<ActionResult<SupplierGroup>> PostSupplierGroup(SupplierGroup supplierGroup)
    {
      Log.Information("POST suppliergroups: {supplierGroup}", supplierGroup);
      var success = await _groupsService.PostSupplierGroup(supplierGroup);
      if (!success) return Conflict();
      return CreatedAtAction("GetSupplierGroup", new { id = supplierGroup.Id }, supplierGroup);
    }

    // DELETE: suppliergroups/5
    [HttpDelete("{id}")]
    [AuthorizeAd("Admin")]
    public async Task<ActionResult<SupplierGroup>> DeleteSupplierGroup(long id)
    {
      Log.Information("DELETE suppliergroup/{id}", id);

      var success = await _groupsService.DeleteSupplierGroup(id);
      if (!success) return NotFound();
      return NoContent();
    }
  }
}
