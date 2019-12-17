using Raumplanung.Infrastructure.Contracts.Services;
using Raumplanung.Infrastructure.Contracts.Stores;
using Raumplanung.Infrastructure.Models;
using System;
using System.Collections.Generic;
using System.Text;

namespace ServiceLayer.Services
{
  public class UserService : IUserService
  {
    private readonly IUserStore _userStore;

    public UserService(IUserStore userStore)
    {
      _userStore = userStore;
    }

    public User GetDetails(long id)
    {
      return _userStore.GetUserById(id);
    }
  }
}