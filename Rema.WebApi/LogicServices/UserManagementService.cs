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
      var dbUser = _context.Users/*.AsNoTracking()*/.FirstOrDefault(p => p.ActiveDirectoryID == activeDirectoryId);
      user = dbUser;
      return dbUser != null;
    }

    public User GetOrInsertUserFromDB(string adID)
    {
      var isInDb = IsUserInDb(out User dbUser, adID);
      if (isInDb)
      {
        return dbUser;
      }

      var adUserObj = GetUserByActiveDirectoryId(adID);
      UpdateOrInsert(dbUser, adUserObj, false);

      var updatedUser = _context.Users/*.AsNoTracking()*/.FirstOrDefault(p => p.ActiveDirectoryID == adID);
      return updatedUser;
    }

    public User GetAndUpdateOrInsertUserFromDB(string adID)
    {
      var isInDb = IsUserInDb(out User dbUser, adID);
      var adUserObj = GetUserByActiveDirectoryId(adID);
      UpdateOrInsert(dbUser, adUserObj, isInDb);

      var updatedUser = _context.Users/*.AsNoTracking()*/.FirstOrDefault(p => p.ActiveDirectoryID == adID);

      return updatedUser;
    }
    private void UpdateOrInsert(User fromDb, User fromAd, bool isInDb)
    {
      try
      {
        if (!isInDb)  // insert
        {
          _context.Users.Add(fromAd);
          _context.SaveChanges();
         // _context.Entry(fromAd).State = EntityState.Detached;
        }
        else if(fromAd.Email != fromDb.Email || fromAd.Name != fromDb.Name || fromAd.Organisation != fromDb.Organisation) // update
        {
          fromAd.Id = fromDb.Id;
          _context.Entry(fromDb).CurrentValues.SetValues(fromAd);
          _context.SaveChanges();
         // _context.Entry(fromDb).State = EntityState.Detached;
        }
      }
      catch (Exception ex)
      {
        Log.Error(ex, "An error occured while storing or updating the user to the database.");
      }
    }
    

    public User GetUserByActiveDirectoryId(string activeDirectoryID)
    {
      var user = new DirectoryEntry($"LDAP://<SID={activeDirectoryID}>");

      //Ask for only the attributes you want to read.
      //If you omit this, it will end up getting every attribute with a value,
      //which is unnecessary.
      user.RefreshCache(new[] { "givenName", "sn", "mail", "displayName", "company", "name" });
      /*
      foreach(System.DirectoryServices.PropertyValueCollection p in user.Properties) {
        Debug.WriteLine(p.PropertyName);
        Debug.WriteLine(p.Value);
      }
      */

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
