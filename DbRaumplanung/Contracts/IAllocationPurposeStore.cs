using DbRaumplanung.Models;
using System;
using System.Collections.Generic;
using System.Text;

namespace DbRaumplanung.Contracts
{
    public interface IAllocationPurposeStore
    {
        AllocationPurpose GetAllocationPurposeById(long id);

    }
}
