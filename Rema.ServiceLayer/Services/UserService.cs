using Rema.Infrastructure.Contracts.Services;
using Rema.Infrastructure.Contracts.Stores;
using Rema.Infrastructure.Models;

namespace Rema.ServiceLayer.Services
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
