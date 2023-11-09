// SPDX-FileCopyrightText: © 2019 Oberverwaltungsgericht Rheinland-Pfalz <poststelle@ovg.jm.rlp.de>
// SPDX-License-Identifier: EUPL-1.2
using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.EntityFrameworkCore;
using Rema.DbAccess;
using Rema.Infrastructure.Models;
using System.DirectoryServices;
using System.Linq;
using System.Security.Principal;
using Rema.ServiceLayer.Services;
using Microsoft.AspNetCore.Http;
using Serilog;
using System.Threading.Tasks;
using Rema.WebApi.ViewModels;

namespace Rema.WebApi.LogicServices
{
  public class UserManagementService
  {
    private RpDbContext _context;

    public UserManagementService(RpDbContext context)
    {
      this._context = context;
    }

    private bool IsUserInDb(out User user, string activeDirectoryId)
    {
      var dbUser = _context.Users.FirstOrDefault(p => p.ActiveDirectoryID == activeDirectoryId);
      user = dbUser;
      return dbUser != null;
    }

    /**
     * Liefert Benutzerobjekt zurück, falls noch nicht in DB ohne ID
     * letzteres sollte nur passieren in der Sekunde in welcher 
     * Nutzer noch nicht gespeichert wurde
     */
    public User GetUser(string adID)
    {
      var isInDb = IsUserInDb(out User dbUser, adID);
      if (isInDb)
      {
        return dbUser;
      }
      var adUserObj = GetUserByActiveDirectoryId(adID);
      return adUserObj;
    }

    public User GetAndUpdateOrInsertUserFromDB(string adID)
    {
      var isInDb = IsUserInDb(out User dbUser, adID);
      var adUserObj = GetUserByActiveDirectoryId(adID);

      try
      {
        if (!isInDb)  // insert
        {
          _context.Users.Add(adUserObj);
          _context.SaveChanges();
        }
        else if (adUserObj.Email != dbUser.Email || adUserObj.Name != dbUser.Name || adUserObj.Organisation != dbUser.Organisation) // update
        {
          adUserObj.Id = dbUser.Id;
          _context.Entry(dbUser).CurrentValues.SetValues(adUserObj);
          _context.SaveChanges();
        }
      }
      catch (Exception ex)
      {
        Log.Error(ex, "An error occured while storing or updating the user to the database.");
      }
      var updatedUser = _context.Users.FirstOrDefault(p => p.ActiveDirectoryID == adID);

      return updatedUser;
    }

    public User GetUserByActiveDirectoryId(string activeDirectoryID)
    {
      var user = new DirectoryEntry($"LDAP://<SID={activeDirectoryID}>");

      //Ask for only the attributes you want to read.
      //If you omit this, it will end up getting every attribute with a value,
      //which is unnecessary.
      user.RefreshCache(new[] { "givenName", "sn", "mail", "displayName", "company", "name" });

      var firstName = user.Properties["givenName"].Value;
      var lastName = user.Properties["sn"].Value;
      var mail = (string)user.Properties["mail"].Value ?? "support@ovg.jm.rlp.de"; // todo: falls nutzer keine Email in Ad hat, sollte Ausnahme sein. Bessere Lösung finden
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

    public List<Role> SearchUserGroups(WindowsIdentity identity)
    {
      var roles = new List<Role>();
      foreach (var group in identity.Groups)
      {
        string groupTitle = group.Translate(typeof(NTAccount)).ToString();
        var role = Startup.Roles.Find(e => e.AdDescription.Equals(groupTitle));
        if (role != null)
        {
          roles.Add(role);
        }
      }
      return roles;
    }
  }
}
