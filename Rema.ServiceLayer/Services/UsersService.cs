using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AutoMapper;
using Microsoft.EntityFrameworkCore;
using Rema.Infrastructure.LDAP;
using Rema.Infrastructure.Models;
using Rema.WebApi.ViewModels;
using Serilog;

namespace Rema.ServiceLayer.Services
{
  public class UsersService : IUserService
  {
  public async Task<AdUserViewModel> GetUser(DbSet<User> set, long id)
    {
      try
      {
        var user = await set.FindAsync(id);
        if (user == null)
        {
          return null;
        }
        var returnUser = new AdUserViewModel() { ActiveDirectoryID = user.ActiveDirectoryID, Email = user.Email, Name = user.Name };
        return returnUser;
      }
      catch (Exception ex)
      {
        Log.Error(ex, "error while getting user");
        return null;
      }
    }

    public async Task<ContactUser> GetUserName(DbSet<User> set, IMapper mapper, long id)
    {
      User user;
      try
      {
        user = await set.FindAsync(id);
        if (user == null)
        {
          return null;
        }
      }
      catch (Exception ex)
      {
        Log.Error(ex, "error while getting user");
        return null;
      }

      try
      {
        var userVM = mapper.Map<User, ContactUser>(user);
        return userVM;
      }
      catch (Exception ex)
      {
        Log.Error(ex, "error while mapping user");
        return null;
      }
    }

    public async Task<List<ContactUser>> GetUserNames(DbSet<User> set, IMapper mapper, IEnumerable<long> ids)
    {
      IList<User> users;
      try
      {
        users = await set.Where(e => ids.Contains(e.Id)).ToListAsync();
        if (users == null)
        {
          return null;
        }
      }
      catch (Exception ex)
      {
        Log.Error(ex, "error while getting user");
        return null;
      }

      try
      {
        var userVM = users.Select(mapper.Map<User, ContactUser>).ToList();
        return userVM;
      }
      catch (Exception ex)
      {
        Log.Error(ex, "error while mapping user");
        return null;
      }
    }
  }
}
