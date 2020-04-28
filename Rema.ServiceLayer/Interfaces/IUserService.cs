using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using AutoMapper;
using Microsoft.EntityFrameworkCore;
using Rema.Infrastructure.LDAP;
using Rema.Infrastructure.Models;
using Rema.WebApi.ViewModels;

namespace Rema.ServiceLayer.Interfaces
{
  public interface IUserService
  {
    public Task<AdUserViewModel> GetUser(DbSet<User> set, long id);
    public Task<ContactUser> GetUserName(DbSet<User> set, IMapper mapper, long id);
    public Task<List<ContactUser>> GetUserNames(DbSet<User> set, IMapper mapper, IEnumerable<long> ids);

  }
}
