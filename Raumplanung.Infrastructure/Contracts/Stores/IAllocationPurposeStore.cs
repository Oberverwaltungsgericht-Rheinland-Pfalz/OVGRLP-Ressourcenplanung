using System.Collections.Generic;
using Raumplanung.Infrastructure.Models;

namespace Raumplanung.Infrastructure.Contracts.Stores
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
