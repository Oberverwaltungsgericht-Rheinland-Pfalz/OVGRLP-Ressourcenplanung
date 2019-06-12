using DbRaumplanung.Models;
using System;
using System.Collections.Generic;
using System.Text;

namespace DbRaumplanung.Contracts
{
    public interface IUserStore
    {
        User GetUserById(long id);
        IEnumerable<User> GetUsersBySupplierGroup(long supplierGroupId);
    }
}
