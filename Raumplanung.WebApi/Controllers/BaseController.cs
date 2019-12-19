using System;
using System.Collections.Generic;
using System.DirectoryServices;
using System.Linq;
using System.Security.Principal;
using System.Threading.Tasks;
using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using Raumplanung.Data.DataAccess;
using Raumplanung.Infrastructure.Models;
using Raumplanung.WebApi.ViewModels;

// For more information on enabling MVC for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace Raumplanung.WebApi.Controllers
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

    private User _userInst = null;
    private UserViewModel _userVMinst = null;

    public UserViewModel RequestSenderVM
    {
      get
      {
        if (_userVMinst == null)
          _userVMinst = _mapper.Map<User, UserViewModel>(RequestSender);
        return _userVMinst;
      }
    }

    protected User RequestSender
    {
      get
      {
        if (_userInst != null) return _userInst;

        var requester = this.HttpContext.User;
        var identity = (WindowsIdentity)requester.Identity;
        string domainId = identity.User.Value;

        var user = _context.Users.FirstOrDefault(p => p.ActiveDirectoryID == domainId);
        if (user == null)
        {
          var newUser = GetADUser(identity);
          _context.Users.Add(newUser);
          _context.SaveChanges();
          _userInst = newUser;
        }
        else
        {
          _userInst = user;
        }

        return _userInst;
      }
    }

    private User GetADUser(WindowsIdentity identity)
    {
      return GetADUserById(identity.User.Value);
    }

    public User GetADUserById(string activeDirectoryID)
    {
      var user = new DirectoryEntry($"LDAP://<SID={activeDirectoryID}>");

      //Ask for only the attributes you want to read.
      //If you omit this, it will end up getting every attribute with a value,
      //which is unnecessary.
      user.RefreshCache(new[] { "givenName", "sn", "mail", "displayName", "company", "name" });

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