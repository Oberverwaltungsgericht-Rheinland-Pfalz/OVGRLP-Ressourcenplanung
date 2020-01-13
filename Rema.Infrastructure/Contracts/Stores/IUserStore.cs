using System.Collections.Generic;
using Rema.Infrastructure.Models;

namespace Rema.Infrastructure.Contracts.Stores
{
  public interface IUserStore
  {
    User SaveUser(User user);

    User GetUserById(long id);

    IEnumerable<User> GetUsersBySupplierGroup(long supplierGroupId);

    User UpdateUser(User user);

    bool DeleteUserById(long id);
  }
}
