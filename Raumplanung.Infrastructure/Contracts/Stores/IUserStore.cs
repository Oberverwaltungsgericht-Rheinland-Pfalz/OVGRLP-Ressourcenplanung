using Raumplanung.Infrastructure.Models;
using System;
using System.Collections.Generic;
using System.Text;

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