using Raumplanung.Infrastructure.Models;
using System;
using System.Collections.Generic;
using System.Text;

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