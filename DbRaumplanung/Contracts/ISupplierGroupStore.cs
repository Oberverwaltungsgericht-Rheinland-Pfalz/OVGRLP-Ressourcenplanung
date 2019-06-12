using DbRaumplanung.Models;
using System;
using System.Collections.Generic;
using System.Text;

namespace DbRaumplanung.Contracts
{
    public interface ISupplierGroupStore
    {
        SupplierGroup CreateSupplierGroup(string name, string GroupEmail);
        SupplierGroup GetSupplierGroupById(long id);

        SupplierGroup UpdateSupplierGroup(SupplierGroup supplierGroup);

        bool DeleteSupplierGroup(long id);
    }
}
