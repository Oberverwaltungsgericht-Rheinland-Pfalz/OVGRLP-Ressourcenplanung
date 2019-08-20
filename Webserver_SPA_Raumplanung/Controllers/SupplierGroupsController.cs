using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using DbRaumplanung.DataAccess;
using DbRaumplanung.Models;

namespace AspNetCoreVueStarter.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class SupplierGroupsController : ControllerBase
    {
        private readonly RpDbContext _context;

        public SupplierGroupsController(RpDbContext context)
        {
            _context = context;
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

            return NoContent();
        }

        // POST: api/SupplierGroups
        [HttpPost]
        public async Task<ActionResult<SupplierGroup>> PostSupplierGroup(SupplierGroup supplierGroup)
        {
            _context.SupplierGroups.Add(supplierGroup);
            await _context.SaveChangesAsync();

            return CreatedAtAction("GetSupplierGroup", new { id = supplierGroup.Id }, supplierGroup);
        }

        // DELETE: api/SupplierGroups/5
        [HttpDelete("{id}")]
        public async Task<ActionResult<SupplierGroup>> DeleteSupplierGroup(long id)
        {
            var supplierGroup = await _context.SupplierGroups.FindAsync(id);
            if (supplierGroup == null)
            {
                return NotFound();
            }

            _context.SupplierGroups.Remove(supplierGroup);
            await _context.SaveChangesAsync();

            return supplierGroup;
        }

        private bool SupplierGroupExists(long id)
        {
            return _context.SupplierGroups.Any(e => e.Id == id);
        }
    }
}
