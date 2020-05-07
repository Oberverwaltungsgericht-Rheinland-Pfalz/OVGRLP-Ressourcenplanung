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
using Rema.WebApi.LogicServices;
using Rema.WebApi.ViewModels;
using Serilog;

// For more information on enabling MVC for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace Rema.WebApi.Controllers
{
  public abstract class BaseController : ControllerBase
  {
    protected RpDbContext _context;
    protected IMapper _mapper;
    protected UserManagementService _userManagementService;

    protected BaseController(RpDbContext context, IMapper mapper)
    {
      _context = context;
      _mapper = mapper;
      _userManagementService = new UserManagementService(context);
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
        if (_user == null)
        {
          var requester = this.HttpContext.User;
          var identity = (WindowsIdentity)requester.Identity;
          var adID = identity.User.Value;

          _user = _userManagementService.GetAndUpdateUserFromDB(adID);
          //        _user = GetUserFromHttpContext(this.HttpContext);  
          //        SaveUser(_user);
          // todo: check who to add new user informations
        }
        return _user;
      }
    }

    public UserViewModel RequestSenderVMInitial()
    {
      if (_userViewModel == null)
      {
        if (_user == null)
        {
          var requester = this.HttpContext.User;
          var identity = (WindowsIdentity)requester.Identity;
          var adID = identity.User.Value;

          _user = _userManagementService.GetAndUpdateOrInsertUserFromDB(adID);
        }
        _userViewModel = _mapper.Map<User, UserViewModel>(_user);
      }
      return _userViewModel;
    }
  }
}
