using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using Rema.Infrastructure.Models;

namespace Rema.ServiceLayer.Interfaces
{
  public interface ISupplierGroupsService
  {
    public Task<IEnumerable<SupplierGroup>> GetSupplierGroups();
    
    public Task<SupplierGroup> GetSupplierGroup(long id);

    public Task<bool> PutSupplierGroup(SupplierGroup supplierGroup);

    public Task<bool> PostSupplierGroup(SupplierGroup supplierGroup);
    

    public Task<bool> DeleteSupplierGroup(long id);
    }
}
