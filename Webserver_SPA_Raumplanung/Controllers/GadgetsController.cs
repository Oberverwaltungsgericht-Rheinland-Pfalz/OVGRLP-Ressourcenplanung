using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using DbRaumplanung.DataAccess;
using DbRaumplanung.Models;
using Serilog;
using AutoMapper;
using AspNetCoreVueStarter.ViewModels;
using System.Net;
using System.Net.Mail;
using Infrastructure.Email;
using AspNetCoreVueStarter.Filter;

namespace AspNetCoreVueStarter.Controllers
{
    [Produces("application/json")]
    [Route("api/[controller]")]
    [ApiController]
    [AuthorizeAd("Reader")]
    public class GadgetsController : BaseController
    {
        public GadgetsController(RpDbContext context, IMapper mapper) : base(context, mapper)
        {
        }

        // GET: api/Gadgets
        [HttpGet]
        public async Task<ActionResult<IEnumerable<GadgetViewModel>>> GetGadgets()
        {
            var gadgets = await _context.Gadgets.Include(g => g.SuppliedBy).ToListAsync();
            var gadgetVMS = gadgets.Select<Gadget, GadgetViewModel>((e) => _mapper.Map<Gadget, GadgetViewModel>(e));
            
            Log.Information("GetGadgets was executed. {@gadets.Count} were send", gadgets.Count);
            return gadgetVMS.ToList();
        }

        // GET: api/Gadgets/5
        [HttpGet("{id}")]
        public async Task<ActionResult<GadgetViewModel>> GetGadget(long id)
        {
            var gadget = await _context.Gadgets.FindAsync(id);

            if (gadget == null)
            {
                return NotFound();
            }

            return _mapper.Map<Gadget, GadgetViewModel>(gadget);
        }

        // PUT: api/Gadgets/5
        [HttpPut("{id}")]
        [AuthorizeAd("Admin")]
        public async Task<IActionResult> PutGadget(long id, GadgetViewModel gadget)
        {
            Gadget gad = await AddGroup(gadget);
            if (id != gad.Id)
            {
                return BadRequest();
            }
            
            _context.Entry(gad).State = EntityState.Modified;

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

            Log.Information("Gadget {@gadets.IdTitle} was updated by {@User.email}", gadget.Id + gadget.Title, base.RequestSender.Email);
            return NoContent();
        }

        // POST: api/Gadgets
        [HttpPost]
        [AuthorizeAd("Admin")]
        public async Task<ActionResult<GadgetViewModel>> PostGadget(GadgetViewModel gadget)
        {
            Gadget gad = await AddGroup(gadget);
            _context.Gadgets.Add(gad);
            await _context.SaveChangesAsync();

            EmailTrigger.SendEmail("Hilfsmittel erzeugt", $"{gadget.Title} wurde erstellt", recipient: base.RequestSender.Email);
            Log.Information("Gadget {@gadets.IdTitle} was inserted by {@User.email}", gadget.Id + gadget.Title, base.RequestSender.Email);
            return CreatedAtAction("GetGadget", new { id = gad.Id }, gadget);
        }

        private async Task<Gadget> AddGroup(GadgetViewModel gadget) {
            var supplier = await _context.SupplierGroups.FindAsync(gadget.SuppliedBy);
            Gadget gad = new Gadget() { Title=gadget.Title, SuppliedBy = supplier};
            if (gadget.Id != 0) gad.Id = gadget.Id;
            return gad;
        }

        // DELETE: api/Gadgets/5
        [HttpDelete("{id}")]
        [AuthorizeAd("Admin")]
        public async Task<ActionResult<Gadget>> DeleteGadget(long id)
        {
            var gadget = await _context.Gadgets.FindAsync(id);
            if (gadget == null)
            {
                return NotFound();
            }

            _context.Gadgets.Remove(gadget);
            await _context.SaveChangesAsync();

            Log.Information("Gadget {@gadets.IdTitle} was deleted by {@User.email}", gadget.Id + gadget.Title, base.RequestSender.Email);
            return gadget;
        }

        private bool GadgetExists(long id)
        {
            return _context.Gadgets.Any(e => e.Id == id);
        }
    }
}
