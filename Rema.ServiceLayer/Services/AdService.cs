using System;
using System.Collections.Generic;
using System.DirectoryServices;
using System.DirectoryServices.AccountManagement;
using System.DirectoryServices.ActiveDirectory;
using System.Linq;
using System.Security.Principal;
using System.Text;

namespace Rema.ServiceLayer.Services
{
  public class AdService
  {
    public List<string> DomainsToSearch;

    public AdService()
    {
    }

    public AdService(List<string> domainsToSearch)
    {
      this.DomainsToSearch = domainsToSearch;
    }

    public Domain FindDomainByName(string name)
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

    public List<string> GetMemberListByUserName(string domainName, string userName)
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

    public List<string> SearchAdUsers(string userName)
    {
      var foundUsers = new List<string>();
      List<string> searchDomainList = this.DomainsToSearch;

      DomainCollection domains = Forest.GetCurrentForest().Domains;
      foreach (Domain dom in domains)
      {
        if (null == searchDomainList || searchDomainList.Count == 0 || searchDomainList.Contains(dom.Name))
        {
          using (var context = new PrincipalContext(ContextType.Domain, dom.Name))
          {
            UserPrincipal userPrin = new UserPrincipal(context);
            userPrin.Enabled = true;   // nur aktive Nutzer
            if (!string.IsNullOrEmpty(userName))
              userPrin.DisplayName = "*" + userName + "*";

            using (var searcher = new PrincipalSearcher(userPrin))
            {
              var ds = searcher.GetUnderlyingSearcher() as DirectorySearcher;
              //ds.PropertiesToLoad.Clear();
              //ds.PropertiesToLoad.Add("mail");
              foreach (var result in searcher.FindAll())
              {
                DirectoryEntry de = result.GetUnderlyingObject() as DirectoryEntry;

                foundUsers.Add(result.DisplayName);
                //if (null != de.Properties["mail"] && null != de.Properties["mail"].Value)
                //foundUsers.Add(de.Properties["mail"]?.Value?.ToString());
              }
            }
          }
        }
      }
      return foundUsers;
    }
  }
}
