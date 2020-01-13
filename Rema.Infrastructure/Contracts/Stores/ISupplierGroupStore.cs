using Rema.Infrastructure.Models;

namespace Rema.Infrastructure.Contracts.Stores
{
  public interface ISupplierGroupStore
  {
    SupplierGroup CreateSupplierGroup(string name, string GroupEmail);

    SupplierGroup GetSupplierGroupById(long id);

    SupplierGroup UpdateSupplierGroup(SupplierGroup supplierGroup);

    bool DeleteSupplierGroup(long id);
  }
}
