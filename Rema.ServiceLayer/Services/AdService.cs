// SPDX-FileCopyrightText: © 2019 Oberverwaltungsgericht Rheinland-Pfalz <poststelle@ovg.jm.rlp.de>
// SPDX-License-Identifier: EUPL-1.2
using System;
using System.Collections.Generic;
using System.DirectoryServices;
using System.DirectoryServices.AccountManagement;
using System.DirectoryServices.ActiveDirectory;
using System.Linq;
using System.Security.Principal;
using System.Text;
using Rema.Infrastructure.LDAP;
using Rema.Infrastructure.Models;

namespace Rema.ServiceLayer.Services
{
  public interface IAdService
  {
    List<T> SearchAdUsers<T>(string userName) where T : class;

  }
  public class AdService : IAdService
  {
    public List<string> DomainsToSearch;

    public AdService() { }

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

    public List<T> SearchAdUsers<T>(string userName) where T : class
    {
      List<T> foundUsers = new List<T>();
      bool modelType = typeof(T) == typeof(AdUserViewModel);

      foreach (string domain in this.DomainsToSearch)
      {
        Console.WriteLine(domain);
        PrincipalContext ctx = new PrincipalContext(ContextType.Domain, domain);

        UserPrincipal qbeUser = new UserPrincipal(ctx);
        qbeUser.Name = "*" + userName + "*";
        qbeUser.Enabled = true;

        // create your principal searcher passing in the QBE principal    
        PrincipalSearcher srch = new PrincipalSearcher(qbeUser);

        // find all matches
        foreach (var found in srch.FindAll())
        {
          // do whatever here - "found" is of type "Principal"
          UserPrincipal userFound = found as UserPrincipal;
          if (userFound.SamAccountName.StartsWith("Admin.", StringComparison.OrdinalIgnoreCase)) continue;

          object user = userFound.DisplayName;

          if (userFound != null && modelType)
          {
            user = new AdUserViewModel()
            {
              ActiveDirectoryID = userFound.Sid.Value,
              Name = userFound.DisplayName,
              Email = userFound.EmailAddress,
              Phone = userFound.VoiceTelephoneNumber
            };
          }

          foundUsers.Add(user as T);
        }
      }

      return foundUsers;
    }
  }
}
