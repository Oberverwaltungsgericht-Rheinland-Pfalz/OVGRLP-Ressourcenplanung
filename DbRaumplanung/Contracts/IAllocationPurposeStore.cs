using DbRaumplanung.Models;
using System;
using System.Collections.Generic;
using System.Text;

namespace DbRaumplanung.Contracts
{
    public interface IAllocationPurposeStore
    {
        AllocationPurpose CreateAllocationPurpose(AllocationPurpose allocationPurpose);
        AllocationPurpose GetAllocationPurposeById(long id);

        AllocationPurpose UpdateAllocationPurpose(AllocationPurpose allocationPurpose);

        bool DeleteAllocationPurpose(long id);
    }
}
