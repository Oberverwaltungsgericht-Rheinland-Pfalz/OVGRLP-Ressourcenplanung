using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using DbRaumplanung.DataAccess;
using DbRaumplanung.Models;
using AspNetCoreVueStarter.ViewModels;
using AutoMapper;
using Infrastructure.Email;

namespace AspNetCoreVueStarter.Controllers
{
    [Produces("application/json")]
    [Route("api/[controller]")]
    [ApiController]
    public class AllocationsController : ControllerBase
    {
        private readonly RpDbContext _context;
        private readonly IMapper _mapper;

        public AllocationsController(RpDbContext context, IMapper mapper)
        {
            _context = context;
            _mapper = mapper;
        }

        // GET: api/Allocations
        [HttpGet]
        public async Task<ActionResult<IEnumerable<object>>> GetAllocations()
        {
            var all =  await _context.Allocations.Include(g => g.Ressource).Include(g => g.Purpose).Include(g => g.ApprovedBy).Include(g => g.CreatedBy).Include(g => g.LastModifiedBy).Include(g => g.ReferencePerson).ToListAsync();
           // return all.Select(e => _mapper.Map<Allocation, AllocationViewModel>(e));
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

        [HttpGet("filter/{filter}")]
        public async Task<ActionResult<IEnumerable<object>>> GetAllocations(AllocationFilter filter)
        {
            var all = await _context.Allocations.Include(g => g.Ressource).Include(g => g.Purpose).Include(g => g.ApprovedBy).Include(g => g.CreatedBy).Include(g => g.LastModifiedBy).Include(g => g.ReferencePerson).ToListAsync();
            // return all.Select(e => _mapper.Map<Allocation, AllocationViewModel>(e));
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
            allocation.LastModified = DateTime.Now;
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

        // PUT: api/Allocations/EditRequest
        [HttpPut("{editedRequest}")]
        public async Task<ActionResult<Boolean>> EditRequest(AllocationRequestEdition editedRequest)
        {
            var allocation = await _context.Allocations.FindAsync(editedRequest.Id);

            if (allocation == null)
            {
                return BadRequest();
            }

            allocation.Status = (MeetingStatus)editedRequest.status;
            allocation.LastModified = DateTime.Now;
            allocation.ApprovedAt = DateTime.Now;
            if((MeetingStatus) editedRequest.status == MeetingStatus.Moved)
            {
                EmailTrigger.SendEmail("Buchung wurde verschoben", $"Ihre Buchung {allocation.Purpose.Title} der Ressource {allocation.Ressource.Name} vom {allocation.From} bis {allocation.To} wurde gelöscht");

                allocation.From = editedRequest.From.GetValueOrDefault();
                allocation.To = editedRequest.To.GetValueOrDefault();
            }
              else if((MeetingStatus)editedRequest.status == MeetingStatus.Approved)
            {
                EmailTrigger.SendEmail("Buchung wurde genehmigt", $"Ihre Buchungsanfrage {allocation.Purpose.Title} der Ressource {allocation.Ressource.Name} vom {allocation.From} bis {allocation.To} wurde genehmigt");
            }
              else if ((MeetingStatus)editedRequest.status == MeetingStatus.Clarification)
            {
                EmailTrigger.SendEmail("Buchung wurde abgelehnt", $"Ihre Buchungsanfrage {allocation.Purpose.Title} der Ressource {allocation.Ressource.Name} vom {allocation.From} bis {allocation.To} wurde abgelehnt");
            }

            _context.Entry(allocation).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!AllocationExists(editedRequest.Id))
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
        public async Task<ActionResult<AllocationViewModel>> PostAllocation(AllocationViewModel allocation)
        {
            var purpose = await _context.AllocationPurposes.FindAsync(allocation.Purpose_id);
            var ressource = await _context.Ressources.FindAsync(allocation.Ressource_id);
            var user = await _context.Users.FindAsync(1L);
            
            Allocation all = _mapper.Map<AllocationViewModel, Allocation>(allocation);
            all.Purpose = purpose;
            all.Ressource = ressource;
            all.LastModified = DateTime.Now;
            all.CreatedAt = DateTime.Now;
            all.ApprovedBy = all.CreatedBy = all.LastModifiedBy = all.ReferencePerson = all.ApprovedBy = user;


            _context.Allocations.Add(all);
            await _context.SaveChangesAsync();

            return CreatedAtAction("GetAllocation", new { id = allocation.Id }, allocation);
        }

        // DELETE: api/Allocations/5
        [HttpDelete("{id}")]
        public async Task<ActionResult<Allocation>> DeleteAllocation(long id)
        {
            var allocation = await _context.Allocations.Include(o => o.Purpose).Include(o => o.Ressource).FirstOrDefaultAsync(i => i.Id == id);
            if (allocation == null)
            {
                return NotFound();
            }

            _context.Allocations.Remove(allocation);
            await _context.SaveChangesAsync();

            EmailTrigger.SendEmail("Ihr Termin wurde gelöscht", $"Ihre Buchung {allocation.Purpose.Title} der Ressource {allocation.Ressource.Name} vom {allocation.From} bis {allocation.To} wurde gelöscht");
            return allocation;
        }

        private bool AllocationExists(long id)
        {
            return _context.Allocations.Any(e => e.Id == id);
        }
    }
}
