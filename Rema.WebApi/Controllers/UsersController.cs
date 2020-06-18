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
using Rema.ServiceLayer;
using Rema.ServiceLayer.Interfaces;

namespace Rema.WebApi.Controllers
{
  [Produces("application/json")]
  [Route("[controller]")]
  [ApiController]
  [AuthorizeAd("Reader")]
  public class UsersController : BaseController
  {
    private readonly IAdService _adService;
    private readonly IUserService _userService;
    public UsersController(RpDbContext context, IMapper mapper, IAdService adService, IUserService userService) : base(context, mapper)
    {
      this._adService = adService;
      this._userService = userService;
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

      var returnUser = await _userService.GetUser(_context.Users, id);
      if (returnUser == null)
      {
        return NotFound();
      }
      return returnUser;
    }

    // GET: adUser/name/5
    [HttpGet("adUser/{namePart}")]
    public ActionResult<List<AdUserViewModel>> GetAdUsers(string namePart)
    {
      Log.Information("GET users/adUser/{namePart}", namePart);

      if (namePart.Length < 3) return new List<AdUserViewModel>();

      List<AdUserViewModel> adUsers = _adService.SearchAdUsers<AdUserViewModel>(namePart);

      Request.Headers.TryGetValue("timestamp", out StringValues timestampValue);
      Response.Headers.Add("timestamp", timestampValue.ToString());
      return adUsers;
    }

    // GET: users/name/5
    [HttpGet("name/{id}")]
    public async Task<ActionResult<ContactUser>> GetUserName(long id)
    {
      Log.Information("GET users/name/{id}", id);

      var userObject = await _userService.GetUserName(_context.Users, _mapper , id);
      if (userObject == null) return NotFound();
      return userObject;
    }

    // GET: users/names/[] array as underline separated numbers
    [HttpGet("names/{ids}")]
    public async Task<ActionResult<List<ContactUser>>> GetUserNames(string ids)
    {
      Log.Information("GET users/names/{id}", ids);
      var idArray = ids.Split('_').Select(e => long.Parse(e));
      var userVM = await _userService.GetUserNames(_context.Users, _mapper, idArray);
      if (userVM == null) return NotFound();
      return userVM;
    }

    // GET: users/me
    [HttpGet("me")]
    public ActionResult<UserViewModel> GetCurrentUser()
    {
      Log.Information("GET users/me");

      // todo: if-bedingungen; Sende nur IF config-parameter = true
      Response.Headers.Add("Requests-Allowed", "true");
      try
      {
        return RequestSenderVMInitial();
      }
      catch (Exception ex)
      {
        Log.Error(ex, "error while getting current user");
        return NotFound();
      }
    }

    /* Benutzer werden nicht manuell bearbeitet. Sie werden vom Webserver bei Bedarf angelegt und geändert. Eine Löschung ist nicht vorgesehen
    // PUT: users/5
    [HttpPut("{id}")]
    [AuthorizeAd("Admin")]
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
    [AuthorizeAd("Admin")]
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
    [AuthorizeAd("Admin")]
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
    */
  }
}
