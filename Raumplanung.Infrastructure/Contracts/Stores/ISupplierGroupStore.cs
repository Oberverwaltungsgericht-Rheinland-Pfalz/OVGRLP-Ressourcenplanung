using Raumplanung.Infrastructure.Models;

namespace Raumplanung.Infrastructure.Contracts.Stores
{
  public interface ISupplierGroupStore
  {
    SupplierGroup CreateSupplierGroup(string name, string GroupEmail);

    SupplierGroup GetSupplierGroupById(long id);

    SupplierGroup UpdateSupplierGroup(SupplierGroup supplierGroup);

    bool DeleteSupplierGroup(long id);
  }
}
