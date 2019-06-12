using DbRaumplanung.Models;
using System;
using System.Collections.Generic;
using System.Text;

namespace DbRaumplanung.Contracts
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
