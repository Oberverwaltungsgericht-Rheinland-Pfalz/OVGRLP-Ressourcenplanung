using System.Collections.Generic;
using Raumplanung.Infrastructure.Models;

namespace Raumplanung.Infrastructure.Contracts.Stores
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
