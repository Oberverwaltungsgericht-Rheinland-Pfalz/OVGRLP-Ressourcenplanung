using DbRaumplanung.Models;
using System;
using System.Collections.Generic;
using System.Text;

namespace DbRaumplanung.Contracts
{
    public interface IAllocationStore
    {
        Allocation CreateAllocation(Allocation allocation);

        Allocation GetAllocationById(long id);
        IEnumerable<Allocation> GetAllocationsByRange(DateTime from, DateTime to);
        IEnumerable<Allocation> GetAllocationsByRessource(long ressourceId, DateTime? from, DateTime? to);
        IEnumerable<Allocation> GetAllocationsByStatus(MeetingStatus status, DateTime? from, DateTime? to);
        IEnumerable<Allocation> GetAllocationsByPurpose(long allocationPurposeId, DateTime? from, DateTime? to);

        Allocation UpdateAllocation(Allocation allocation);

        bool DeleteAllocation(long id);
    }
}
