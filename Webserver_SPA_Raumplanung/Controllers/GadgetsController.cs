using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using DbRaumplanung.Contracts;
using DbRaumplanung.DataAccess;
using DbRaumplanung.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace AspNetCoreVueStarter.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class GadgetsController : ControllerBase
    {
        private readonly IGadgetStore _gadgetStore;
        public GadgetsController(IGadgetStore store)
        {
            _gadgetStore = store;
        }

        // GET: api/Gadgets
        [HttpGet]
        public IEnumerable<Gadget> Get()
        {
            return _gadgetStore.GetGadgets();
            // return new string[] { "value1", "value2" };
        }

        // GET: api/Gadgets/5
        [HttpGet("{id}", Name = "Get")]
        public Gadget Get(int id)
        {
            return _gadgetStore.GetGadgetById(id);
            // return "value";
        }

        // POST: api/Gadgets
        [HttpPost]
        public void Post([FromBody] string value)
        {
        }

        // PUT: api/Gadgets/5
        [HttpPut("{id}")]
        public void Put(int id, [FromBody] string value)
        {
        }

        // DELETE: api/ApiWithActions/5
        [HttpDelete("{id}")]
        public void Delete(int id)
        {
            _gadgetStore.DeleteGadget
            var user = await _context.Users.FindAsync(id);
            if (user == null)
            {
                return NotFound();
            }

            _context.Users.Remove(user);
            await _context.SaveChangesAsync();

            return user;
        }
    }
}
