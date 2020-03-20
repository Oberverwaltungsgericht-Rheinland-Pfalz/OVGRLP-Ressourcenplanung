using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Rema.DbAccess;
using Rema.Infrastructure.Models;
using Rema.Infrastructure.LDAP;
using Rema.WebApi.Filter;
using Rema.WebApi.ViewModels;
using Serilog;
using Rema.ServiceLayer.Services;
using Microsoft.Extensions.Primitives;

namespace Rema.WebApi.Controllers
{
  [Produces("application/json")]
  [Route("[controller]")]
  [ApiController]
  [AuthorizeAd("Reader")]
  public class UsersController : BaseController
  {
    public UsersController(RpDbContext context, IMapper mapper) : base(context, mapper)
    {
    }

    // GET: users
    [HttpGet]
    public async Task<ActionResult<IEnumerable<User>>> GetUsers()
    {
      Log.Information("GET users");
      try
      {
        return await _context.Users.ToListAsync();
      }
      catch (Exception ex)
      {
        Log.Error(ex, "error while getting users");
        return NotFound();
      }
    }

    // GET: users/5
    [HttpGet("{id}")]
    public async Task<ActionResult<AdUserViewModel>> GetUser(long id)
    {
      Log.Information("GET users/{id}", id);

      try
      {
        var user = await _context.Users.FindAsync(id);
        if (user == null)
        {
          return NotFound();
        }
        var returnUser = new AdUserViewModel() { ActiveDirectoryID = user.ActiveDirectoryID, Email = user.Email, Name = user.Name};
        return returnUser;
      }
      catch (Exception ex)
      {
        Log.Error(ex, "error while getting user");
        return NotFound();
      }
    }

    // GET: adUser/name/5
    [HttpGet("adUser/{namePart}")]
    public ActionResult<List<AdUserViewModel>> GetAdUsers(string namePart)
    {
      Log.Information("GET users/adUser/{namePart}", namePart);

      if (namePart.Length < 3) return new List<AdUserViewModel>();

      var adService = new AdService(Startup.DomainsToSearch);
      List<AdUserViewModel> adUsers = adService.SearchAdUsers<AdUserViewModel>(namePart);

      Request.Headers.TryGetValue("timestamp", out StringValues timestampValue);
      Response.Headers.Add("timestamp", timestampValue.ToString());
      return adUsers;
    }

    // GET: users/name/5
    [HttpGet("name/{id}")]
    public async Task<ActionResult<ContactUser>> GetUserName(long id)
    {
      Log.Information("GET users/name/{id}", id);

      User user;
      try
      {
        user = await _context.Users.FindAsync(id);
        if (user == null)
        {
          return NotFound();
        }
      }
      catch (Exception ex)
      {
        Log.Error(ex, "error while getting user");
        return NotFound();
      }

      try
      {
        var userVM = _mapper.Map<User, ContactUser>(user);
        return userVM;
      }
      catch (Exception ex)
      {
        Log.Error(ex, "error while mapping user");
        return Conflict();
      }
    }
    // GET: users/names/[]
    [HttpGet("names/{id}")]
    public async Task<ActionResult<List<ContactUser>>> GetUserNames(long[] ids)
    {
      Log.Information("GET users/names/{id}", ids);

      IList<User> users;
      try
      {
        users = await _context.Users.Where(e => ids.Contains(e.Id)).ToListAsync();
        if (users == null)
        {
          return NotFound();
        }
      }
      catch (Exception ex)
      {
        Log.Error(ex, "error while getting user");
        return NotFound();
      }

      try
      {
        var userVM = users.Select(_mapper. Map<User, ContactUser>).ToList();
        return userVM;
      }
      catch (Exception ex)
      {
        Log.Error(ex, "error while mapping user");
        return Conflict();
      }
    }

    // GET: users/me
    [HttpGet("me")]
    public ActionResult<UserViewModel> GetCurrentUser()
    {
      Log.Information("GET users/me");
      try
      {
        return RequestSenderVM;
      }
      catch (Exception ex)
      {
        Log.Error(ex, "error while getting current user");
        return NotFound();
      }
    }

    // GET: users/contact/?namepart=NAMEPART
    [HttpGet("contact")]
    public ActionResult<ContactUser> GetContactUser(string namepart)
    {
      Log.Information("GET users/contact/?namepart=NAMEPART: {namepart}", namepart);
      // lookup ldap for user
      // return list of users as contactUser
      return null;
    }

    // PUT: users/5
    [HttpPut("{id}")]
    public async Task<IActionResult> PutUser(long id, User user)
    {
      Log.Information("PUT users/{id}: {user}", id, user);

      if (id != user.Id)
      {
        return BadRequest();
      }

      try
      {
        _context.Entry(user).State = EntityState.Modified;
        await _context.SaveChangesAsync();
        return NoContent();
      }
      catch (Exception ex)
      {
        Log.Error(ex, "error while saving changed user");
        return Conflict();
      }
    }

    // POST: users
    [HttpPost]
    public async Task<ActionResult<User>> PostUser(User user)
    {
      Log.Information("POST users: {user}", user);

      try
      {
        _context.Users.Add(user);
        await _context.SaveChangesAsync();
        return CreatedAtAction("GetUser", new { id = user.Id }, user);
      }
      catch (Exception ex)
      {
        Log.Error(ex, "error while saving new user");
        return Conflict();
      }
    }

    // DELETE: users/5
    [HttpDelete("{id}")]
    public async Task<ActionResult<User>> DeleteUser(long id)
    {
      Log.Information("DELETE users/{id}", id);

      User user;

      try
      {
        user = await _context.Users.FindAsync(id);
        if (user == null)
        {
          return NotFound();
        }
      }
      catch (Exception ex)
      {
        Log.Error(ex, "error while getting user");
        return NotFound();
      }

      try
      {
        _context.Users.Remove(user);
        await _context.SaveChangesAsync();
        return NoContent();
      }
      catch (Exception ex)
      {
        Log.Error(ex, "error while removing user");
        return Conflict();
      }
    }
  }
}
