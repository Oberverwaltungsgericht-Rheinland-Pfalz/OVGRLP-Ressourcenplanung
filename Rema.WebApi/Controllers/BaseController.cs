using System;
using System.DirectoryServices;
using System.Linq;
using System.Security.Principal;
using AutoMapper;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Rema.DbAccess;
using Rema.Infrastructure.Models;
using Rema.WebApi.ViewModels;
using Serilog;

// For more information on enabling MVC for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace Rema.WebApi.Controllers
{
  public abstract class BaseController : ControllerBase
  {
    protected RpDbContext _context;
    protected IMapper _mapper;

    //public int highestRole = -1;
    protected BaseController(RpDbContext context, IMapper mapper)
    {
      _context = context;
      _mapper = mapper;
    }

    private User _user = null;
    private UserViewModel _userViewModel = null;

    public UserViewModel RequestSenderVM
    {
      get
      {
        if (_userViewModel == null)
          _userViewModel = _mapper.Map<User, UserViewModel>(RequestSender);
        return _userViewModel;
      }
    }

    protected User RequestSender
    {
      get
      {
        if (_user != null) return _user;

        _user = GetUserFromHttpContext(this.HttpContext);

        SaveUser(_user);

        return _user;
      }
    }

    private void SaveUser(User user)
    {
      try
      {
        var dbUser = _context.Users.AsNoTracking().FirstOrDefault(p => p.ActiveDirectoryID == user.ActiveDirectoryID);

        if(dbUser == null)
        {
          _context.Users.Add(user);
        }
        else
        {
          user.Id = dbUser.Id;
          _context.Entry(dbUser).CurrentValues.SetValues(user);
        }
        _context.SaveChanges();
      }
      catch (Exception ex)
      {
        Log.Error(ex, "An error occured while storing the user the database.");
      }
    }

    private User GetUserFromHttpContext(HttpContext context)
    {
      var requester = this.HttpContext.User;
      var identity = (WindowsIdentity)requester.Identity;

      return GetUserByActiveDirectoryId(identity.User.Value);
    }

    public User GetUserByActiveDirectoryId(string activeDirectoryID)
    {
      var user = new DirectoryEntry($"LDAP://<SID={activeDirectoryID}>");

      //Ask for only the attributes you want to read.
      //If you omit this, it will end up getting every attribute with a value,
      //which is unnecessary.
      user.RefreshCache(new[] { "givenName", "sn", "mail", "displayName", "company", "name" });
      /*
      foreach(System.DirectoryServices.PropertyValueCollection p in user.Properties) {
        Debug.WriteLine(p.PropertyName);
        Debug.WriteLine(p.Value);
      }
      */

      var firstName = user.Properties["givenName"].Value;
      var lastName = user.Properties["sn"].Value;
      var mail = (string)user.Properties["mail"].Value;
      var displayName = user.Properties["displayName"].Value;
      var company = (string)user.Properties["company"].Value;
      var name = (string)user.Properties["name"].Value;

      return new User()
      {
        Email = mail,
        Name = name,
        Organisation = company,
        ActiveDirectoryID = activeDirectoryID
      };
    }
  }
}
