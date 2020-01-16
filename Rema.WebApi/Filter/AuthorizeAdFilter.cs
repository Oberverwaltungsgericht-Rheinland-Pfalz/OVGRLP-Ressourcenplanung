using System.Collections.Generic;
using System.Security.Principal;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.Extensions.Configuration;
using Rema.WebApi.Controllers;
using Rema.WebApi.ViewModels;

namespace Rema.WebApi.Filter
{
  public class AuthorizeAdActionFilter : ActionFilterAttribute
  {
    private readonly IConfiguration _configuration;
    private readonly string _permission;

    public static List<Role> Roles
    {
      get =>
          new List<Role>() { Startup.Reader, Startup.Editor, Startup.Admin };
    }
    
    public AuthorizeAdActionFilter(string permission, IConfiguration configuration)
    {
      _configuration = configuration;
      _permission = permission;
    }

    public override void OnActionExecuting(ActionExecutingContext context)
    {
      var requiredRole = Roles.Find(e => e.Name.Equals(_permission));
      var user = context.HttpContext.User;
      var identity = (WindowsIdentity)user.Identity;
      var baseController = (BaseController)context.Controller;

      foreach (var group in identity.Groups)
      {
        string groupTitle = group.Translate(typeof(NTAccount)).ToString();
        var role = Roles.Find(e => e.AdDescription.Equals(groupTitle));
        if (role != null)
        {
          baseController.RequestSenderVM.Roles.Add(role);
        }
      }

      if (baseController.RequestSenderVM.Roles.Exists(e => e.HasRole(requiredRole)))
        return;
      else
        context.Result = new UnauthorizedResult();
    }
  }
}
