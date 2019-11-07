using AspNetCoreVueStarter.Controllers;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using System.Collections.Generic;
using System.Security.Principal;

namespace AspNetCoreVueStarter.Filter
{
    public class AuthorizeAdActionFilter : ActionFilterAttribute
    {
        private readonly string _permission;
        readonly IDictionary<string, string> RightRoles = new Dictionary<string, string>() { { "Reader", "OVGVG\\NJZ Alle" }, { "Editor", "OVGVG\\NJZ IT" }, { "Admin", "OVGVG\\Dashboard_Admins" } };
        readonly IDictionary<string, int> Roles = new Dictionary<string, int>() { { "OVGVG\\NJZ Alle", 0 }, { "OVGVG\\NJZ IT", 10 }, { "OVGVG\\Dashboard_Admins", 100 } };

        public AuthorizeAdActionFilter(string permission)
        {
            _permission = permission;
        }

        public override void OnActionExecuting(ActionExecutingContext context)
        {
            var requiredRole = RightRoles[_permission];
            var requiredLevel = Roles[requiredRole];
            var user = context.HttpContext.User;
            var identity = (WindowsIdentity)user.Identity;
            var groups = new List<string>();
            var baseController = (BaseController)context.Controller;

            foreach (var group in identity.Groups)
            {
                string groupTitle = group.Translate(typeof(NTAccount)).ToString();
                if (Roles.ContainsKey(groupTitle))
                {
                    groups.Add(groupTitle);
                    var groupLevel = Roles[groupTitle];

                    if (groupLevel > baseController.highestRole) 
                        baseController.highestRole = groupLevel;  
                }
            }

            if (baseController.highestRole >= requiredLevel)
                return;
            else
                context.Result = new UnauthorizedResult();
        }
    }

    public class AuthorizeAdAttribute : TypeFilterAttribute
    {
        public AuthorizeAdAttribute(string permission)
            : base(typeof(AuthorizeAdActionFilter))
        {
            Arguments = new object[] { permission };
        }
    }

}
