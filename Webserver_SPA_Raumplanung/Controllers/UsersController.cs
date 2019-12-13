using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using DbRaumplanung.DataAccess;
using DbRaumplanung.Models;
using AspNetCoreVueStarter.Filter;
using AutoMapper;
using AspNetCoreVueStarter.ViewModels;

namespace AspNetCoreVueStarter.Controllers
{
    [Produces("application/json")]
    [Route("api/[controller]")]
    [ApiController]
    [AuthorizeAd("Reader")]
    public class UsersController : BaseController
    {
        public UsersController(RpDbContext context, IMapper mapper) : base(context, mapper)
        {
        }

        // GET: api/Users
        [HttpGet]
        public async Task<ActionResult<IEnumerable<User>>> GetUsers()
        {
            return await _context.Users.ToListAsync();
        }

        // GET: api/Users/5
        [HttpGet("{id:long}")]
        public async Task<ActionResult<User>> GetUser(long id)
        {
            var user = await _context.Users.FindAsync(id);

            if (user == null)
            {
                return NotFound();
            }

            return user;
        }

        // GET: api/Users/Name/5
        [HttpGet("Name/{id:long}")]
        public async Task<ActionResult<ContactUser>> GetUserName(long id)
        {
            var user = await _context.Users.FindAsync(id);

            if (user == null)
            {
                return NotFound();
            }

            var userVM = _mapper.Map<User, ContactUser>(user);
            return userVM;
        }

        // GET: api/Users/me
        [HttpGet("me")]
        public ActionResult<UserViewModel> GetCurrentUser()
        {          
            return RequestSenderVM;
        }

        // - PUT: api/Users/5
        //[HttpPut("{id}")]
        [NonAction]
        public async Task<IActionResult> PutUser(long id, User user)
        {
            if (id != user.Id)
            {
                return BadRequest();
            }

            _context.Entry(user).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!UserExists(id))
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

        // - POST: api/Users
        // [HttpPost]
        [NonAction]
        public async Task<ActionResult<User>> PostUser(User user)
        {
            _context.Users.Add(user);
            await _context.SaveChangesAsync();

            return CreatedAtAction("GetUser", new { id = user.Id }, user);
        }

        // - DELETE: api/Users/5
        // [HttpDelete("{id}")]
        [NonAction]
        public async Task<ActionResult<User>> DeleteUser(long id)
        {
            var user = await _context.Users.FindAsync(id);
            if (user == null)
            {
                return NotFound();
            }

            _context.Users.Remove(user);
            await _context.SaveChangesAsync();

            return user;
        }

        private bool UserExists(long id)
        {
            return _context.Users.Any(e => e.Id == id);
        }
    }
}
