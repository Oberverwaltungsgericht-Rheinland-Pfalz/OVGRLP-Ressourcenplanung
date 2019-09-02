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
    public class AllocationsController : ControllerBase
    {
        private readonly RpDbContext _context;

        public AllocationsController(RpDbContext context)
        {
            _context = context;
        }

        // GET: api/Allocations
        [HttpGet]
        public async Task<ActionResult<IEnumerable<object>>> GetAllocations()
        {
            //return await _context.Allocations.Include(g => g.Ressource).Include(g => g.Purpose).ToListAsync();
            var p = (from a in _context.Allocations
                         //   where purpose.Allocations.Count > 0
                     select new
                     {
                         Id = a.Id,
                         From = a.From,
                         To = a.To,
                         IsAllDay = a.IsAllDay,
                         Status = a.Status,
                         Ressource_id = a.Ressource.Id,
                         Purpose_id = a.Purpose.Id,
                         CreatedBy = a.CreatedBy,
                         CreatedAt = a.CreatedAt,
                         LastModified = a.LastModified,
                         LastModifiedBy = a.LastModifiedBy.Id,
                         ApprovedBy = a.ApprovedBy.Id,
                         ApprovedAt = a.ApprovedAt,
                         ReferencePerson = a.ReferencePerson.Id
                     }).ToList();
            return p;
        }

        // GET: api/Allocations/5
        [HttpGet("{id}")]
        public async Task<ActionResult<Allocation>> GetAllocation(long id)
        {
            var allocation = await _context.Allocations.FindAsync(id);

            if (allocation == null)
            {
                return NotFound();
            }

            return allocation;
        }

        // PUT: api/Allocations/5
        [HttpPut("{id}")]
        public async Task<IActionResult> PutAllocation(long id, Allocation allocation)
        {
            if (id != allocation.Id)
            {
                return BadRequest();
            }

            _context.Entry(allocation).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!AllocationExists(id))
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

        // PUT: api/Allocations/Status/5
        [HttpPut("status/{id}")]
        public async Task<IActionResult> PutAllocation(long id, int status)
        {
            var allocation = await _context.Allocations.FindAsync(id);

            if (allocation == null)
            {
                return BadRequest();
            }

            allocation.Status = (MeetingStatus) status;
            _context.Entry(allocation).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!AllocationExists(id))
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

        // POST: api/Allocations
        [HttpPost]
        public async Task<ActionResult<Allocation>> PostAllocation(Allocation allocation)
        {
            _context.Allocations.Add(allocation);
            await _context.SaveChangesAsync();

            return CreatedAtAction("GetAllocation", new { id = allocation.Id }, allocation);
        }

        // DELETE: api/Allocations/5
        [HttpDelete("{id}")]
        public async Task<ActionResult<Allocation>> DeleteAllocation(long id)
        {
            var allocation = await _context.Allocations.FindAsync(id);
            if (allocation == null)
            {
                return NotFound();
            }

            _context.Allocations.Remove(allocation);
            await _context.SaveChangesAsync();

            return allocation;
        }

        private bool AllocationExists(long id)
        {
            return _context.Allocations.Any(e => e.Id == id);
        }
    }
}
