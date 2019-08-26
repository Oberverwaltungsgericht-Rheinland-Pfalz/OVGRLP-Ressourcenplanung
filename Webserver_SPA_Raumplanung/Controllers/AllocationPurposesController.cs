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
    [Produces("application/json")]
    [Route("api/[controller]")]
    [ApiController]
    public class AllocationPurposesController : ControllerBase
    {
        private readonly RpDbContext _context;

        public AllocationPurposesController(RpDbContext context)
        {
            _context = context;
        }

        // GET: api/AllocationPurposes
        [HttpGet]
        public async Task<ActionResult<IEnumerable<AllocationPurpose>>> GetAllocationPurposes()
        {
            return await _context.AllocationPurposes.ToListAsync();
        }

        // GET: api/AllocationPurposes/5
        [HttpGet("{id}")]
        public async Task<ActionResult<AllocationPurpose>> GetAllocationPurpose(long id)
        {
            var allocationPurpose = await _context.AllocationPurposes.FindAsync(id);

            if (allocationPurpose == null)
            {
                return NotFound();
            }

            return allocationPurpose;
        }

        // PUT: api/AllocationPurposes/5
        [HttpPut("{id}")]
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

            return NoContent();
        }

        // POST: api/AllocationPurposes
        [HttpPost]
        public async Task<ActionResult<AllocationPurpose>> PostAllocationPurpose(AllocationPurpose allocationPurpose)
        {
            _context.AllocationPurposes.Add(allocationPurpose);
            await _context.SaveChangesAsync();

            return CreatedAtAction("GetAllocationPurpose", new { id = allocationPurpose.Id }, allocationPurpose);
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

            return allocationPurpose;
        }

        private bool AllocationPurposeExists(long id)
        {
            return _context.AllocationPurposes.Any(e => e.Id == id);
        }
    }
}
