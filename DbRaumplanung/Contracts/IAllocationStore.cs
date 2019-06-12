using DbRaumplanung.Models;
using System;
using System.Collections.Generic;
using System.Text;

namespace DbRaumplanung.Contracts
{
    public interface IAllocationStore
    {
        Allocation GetAllocationById(long id);
        IEnumerable<Allocation> GetAllocationsByWeek(int year, int week);
        IEnumerable<Allocation> GetAllocationsByMonth(int year, int month);
        IEnumerable<Allocation> GetAllocationsByDate(DateTime date);
        IEnumerable<Allocation> GetAllocationsByRessource(long ressourceId, int? year, int? month);
        IEnumerable<Allocation> GetAllocationsByStatus(MeetingStatus status, int? year, int? month);
        IEnumerable<Allocation> GetAllocationsByPurpose(long allocationPurposeId, int? year, int? month);

    }
}
