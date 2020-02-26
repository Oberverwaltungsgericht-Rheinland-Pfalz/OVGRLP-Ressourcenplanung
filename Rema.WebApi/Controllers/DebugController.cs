using System;
using System.Collections.Generic;
using System.DirectoryServices.AccountManagement;
using System.DirectoryServices.ActiveDirectory;
using System.Linq;
using System.Security.Principal;
using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using Rema.DbAccess;
using Rema.WebApi.Filter;

namespace Rema.WebApi.Controllers
{
  [Produces("application/json")]
  [Route("[controller]")]
  [ApiController]
  public class DebugController : BaseController
  {
    public DebugController(RpDbContext context, IMapper mapper) : base(context, mapper)
    {
    }

    [Route("CurrentUserMembers")]
    [HttpGet]
    public IEnumerable<string> GetCurrentUserMembers()
    {
      var securityMembers = new List<string>();
      var user = this.HttpContext.User;
      var identity = (WindowsIdentity)user.Identity;

      securityMembers.Add(identity.Name);
      foreach (var group in identity.Groups)
      {
        string groupTitle = group.Translate(typeof(NTAccount)).ToString();
        securityMembers.Add(groupTitle);
      }

      return securityMembers;
    }

    [HttpGet("UserMembers/{AdName}")]
    [AuthorizeAd("Admin")]
    public IEnumerable<string> GetUserMembers(string AdName)
    {
      var securityMembers = new List<string>();

      if (!AdName.Contains("@"))
      {
        securityMembers.Add("Bitte geben Sie den Active-Directory Name im Format 'name@domäne' an!");
        return securityMembers;
      }
      string username = AdName.Split('@')[0].ToString();
      string domainName = AdName.Split('@')[1].ToString();

      Domain domain = FindDomainByName(domainName);
      if (null == domain)
      {
        securityMembers.Add(string.Format("Es konnte keine Domäne zu '{0}' ermittelt werden!", domainName));
        return securityMembers;
      }

      securityMembers = GetMemberListByUserName(domain.Name, username);

      return securityMembers;
    }

    private Domain FindDomainByName(string name)
    {
      DomainCollection domains = Forest.GetCurrentForest().Domains;
      Domain domain = null;
      foreach (Domain dom in domains)
      {
        if (dom.Name.Contains(name, StringComparison.OrdinalIgnoreCase))
          domain = dom;
      }
      return domain;
    }

    private List<string> GetMemberListByUserName(string domainName, string userName)
    {
      var securityMembers = new List<string>();
      using (var context = new PrincipalContext(ContextType.Domain, domainName))
      {
        var user = UserPrincipal.FindByIdentity(context, userName);
        if (null != user)
        {
          var userIsMemberOf = user.GetAuthorizationGroups().Where(o => o.Guid != null).Select(o => o.Sid.Translate(typeof(NTAccount)).ToString());
          var groups = new HashSet<string>(userIsMemberOf, StringComparer.OrdinalIgnoreCase);

          securityMembers.Add(user.DisplayName);
          securityMembers.AddRange(groups);
        }
      }
      return securityMembers;
    }
  }
}
