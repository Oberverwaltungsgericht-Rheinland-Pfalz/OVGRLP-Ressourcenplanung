using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.DirectoryServices;
using System.Runtime.Serialization;
using System.Security.Principal;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;

namespace AspNetCoreVueStarter.Filter
{
    public class AuthorizeAdActionFilter : IAuthorizationFilter
    {
        private readonly string _permission;
        readonly IDictionary<string, string> RightRoles = new Dictionary<string, string>() { { "Reader", "OVGVG\\NJZ Alle" }, { "Editor", "OVGVG\\NJZ IT" }, { "Admin", "OVGVG\\Dashboard_Admins" } };
        readonly IDictionary<string, int> Roles = new Dictionary<string, int>() { { "OVGVG\\NJZ Alle", 0 }, { "OVGVG\\NJZ IT", 10 }, { "OVGVG\\Dashboard_Admins", 100 } };

        public AuthorizeAdActionFilter(string permission)
        {
            _permission = permission;
        }

        public void OnAuthorization(AuthorizationFilterContext context)
        {
            var requiredRole = RightRoles[_permission];
            var requiredLevel = Roles[requiredRole];
            var user = context.HttpContext.User;
            var identity = (WindowsIdentity)user.Identity;
            var groups = new List<string>();

            foreach (var group in identity.Groups)
            {
                string groupTitle = group.Translate(typeof(NTAccount)).ToString();
                if (Roles.ContainsKey(groupTitle))
                {
                    groups.Add(groupTitle);
                    if (Roles[groupTitle] >= requiredLevel)
                        return;
                }
            }
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
