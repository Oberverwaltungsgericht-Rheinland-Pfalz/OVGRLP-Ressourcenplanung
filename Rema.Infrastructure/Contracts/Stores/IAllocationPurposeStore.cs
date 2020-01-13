using System.Collections.Generic;
using Rema.Infrastructure.Models;

namespace Rema.Infrastructure.Contracts.Stores
{
  public interface IAllocationPurposeStore
  {
    AllocationPurpose CreateAllocationPurpose(AllocationPurpose allocationPurpose);

    AllocationPurpose GetAllocationPurposeById(long id);

    AllocationPurpose UpdateAllocationPurpose(AllocationPurpose allocationPurpose);

    bool DeleteAllocationPurpose(long id);

    IEnumerable<AllocationPurpose> GetPurposes();
  }
}
