using System.Security.Principal;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.Extensions.Configuration;
using Rema.DbAccess;
using Rema.WebApi.Controllers;
using Rema.WebApi.LogicServices;

namespace Rema.WebApi.Filter
{
  public class AuthorizeAdActionFilter : ActionFilterAttribute
  {
    private readonly IConfiguration _configuration;
    private readonly string _permission;
    protected UserManagementService _userManagementService;
    protected RpDbContext _context;

   
    public AuthorizeAdActionFilter(RpDbContext context, string permission, IConfiguration configuration)
    {
      _configuration = configuration;
      _permission = permission;
      _userManagementService = new UserManagementService(context);
    }

    public override void OnActionExecuting(Microsoft.AspNetCore.Mvc.Filters.ActionExecutingContext context)
    {
      var user = context.HttpContext.User;
      var identity = (WindowsIdentity)user.Identity;
      var requiredRole = Startup.Roles.Find(e => e.Name.Equals(_permission));

      var roles = _userManagementService.SearchUserGroups(identity);
      
      var baseController = (BaseController)context.Controller;
      baseController.RequestSenderVM.Roles = roles;

      if (baseController.RequestSenderVM.Roles.Exists(e => e.HasRole(requiredRole)))
        return;
      else if(_permission != "Reader")  // Schränkt nur ein auf bekannte Nutzer
        context.Result = new UnauthorizedResult();
    }
  }
}
