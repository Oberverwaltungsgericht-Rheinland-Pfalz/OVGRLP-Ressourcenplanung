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
    public class GadgetsController : ControllerBase
    {
        private readonly RpDbContext _context;

        public GadgetsController(RpDbContext context)
        {
            _context = context;
        }

        // GET: api/Gadgets
        [HttpGet]
        public async Task<ActionResult<IEnumerable<Gadget>>> GetGadgets()
        {
            return await _context.Gadgets.ToListAsync();
        }

        // GET: api/Gadgets/5
        [HttpGet("{id}")]
        public async Task<ActionResult<Gadget>> GetGadget(long id)
        {
            var gadget = await _context.Gadgets.FindAsync(id);

            if (gadget == null)
            {
                return NotFound();
            }

            return gadget;
        }

        // PUT: api/Gadgets/5
        [HttpPut("{id}")]
        public async Task<IActionResult> PutGadget(long id, Gadget gadget)
        {
            if (id != gadget.Id)
            {
                return BadRequest();
            }

            _context.Entry(gadget).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!GadgetExists(id))
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

        // POST: api/Gadgets
        [HttpPost]
        public async Task<ActionResult<Gadget>> PostGadget(Gadget gadget)
        {
            _context.Gadgets.Add(gadget);
            await _context.SaveChangesAsync();

            return CreatedAtAction("GetGadget", new { id = gadget.Id }, gadget);
        }

        // DELETE: api/Gadgets/5
        [HttpDelete("{id}")]
        public async Task<ActionResult<Gadget>> DeleteGadget(long id)
        {
            var gadget = await _context.Gadgets.FindAsync(id);
            if (gadget == null)
            {
                return NotFound();
            }

            _context.Gadgets.Remove(gadget);
            await _context.SaveChangesAsync();

            return gadget;
        }

        private bool GadgetExists(long id)
        {
            return _context.Gadgets.Any(e => e.Id == id);
        }
    }
}
