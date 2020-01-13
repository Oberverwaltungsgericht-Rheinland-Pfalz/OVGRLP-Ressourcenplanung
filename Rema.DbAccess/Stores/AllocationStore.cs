using System;
using System.Collections.Generic;
using System.Linq;
using Rema.Infrastructure.Contracts.Stores;
using Rema.Infrastructure.Models;

namespace Rema.DbAccess.Stores
{
  public class AllocationStore : IAllocationStore
  {
    private readonly RpDbContext _context;

    public AllocationStore(RpDbContext context)
    {
      _context = context;
    }

    public Allocation CreateAllocation(Allocation allocation)
    {
      var entity = _context.Allocations.Add(allocation);
      _context.SaveChanges();
      return entity.Entity;
    }

    public bool DeleteAllocation(long id)
    {
      try
      {
        _context.Remove(_context.Allocations.Single(a => a.Id == id));
        _context.SaveChanges();
        return true;
      }
      catch
      {
        // todo: log error
        return false;
      }
    }

    public Allocation GetAllocationById(long id)
    {
      var entity = _context.Allocations.Find(id);
      return entity;
    }

    public Allocation UpdateAllocation(Allocation allocation)
    {
      var entity = _context.Allocations.Update(allocation);
      return entity.Entity;
    }

    public IEnumerable<Allocation> GetAllocationsByRange(DateTime from, DateTime to)
    {
      return _context.Allocations
          .Where(e =>
              (e.From >= from && e.From < to) ||
              (e.To > from && e.To <= to) ||
              (e.From < from && e.To > to)
          )
          .ToList();
    }

    public IEnumerable<Allocation> GetAllocationsByRessource(long ressourceId, DateTime? from, DateTime? to)
    {
      return _context.Allocations
          .Where(e =>
              e.Ressource.Id == ressourceId
              && !from.HasValue || (
                  (e.From >= from && e.From < to) ||
                  (e.To > from && e.To <= to) ||
                  (e.From < from && e.To > to)
              )
          )
          .ToList();
    }

    public IEnumerable<Allocation> GetAllocationsByStatus(MeetingStatus status, DateTime? from, DateTime? to)
    {
      return _context.Allocations
          .Where(e =>
              e.Status == status
              && !from.HasValue || (
                  (e.From >= from && e.From < to) ||
                  (e.To > from && e.To <= to) ||
                  (e.From < from && e.To > to)
              )
          )
          .ToList();
    }

    public IEnumerable<Allocation> GetAllocationsByPurpose(long allocationPurposeId, DateTime? from, DateTime? to)
    {
      return _context.Allocations
          .Where(e =>
              e.Purpose.Id == allocationPurposeId
              && !from.HasValue || (
                  (e.From >= from && e.From < to) ||
                  (e.To > from && e.To <= to) ||
                  (e.From < from && e.To > to)
              )
          )
          .ToList();
    }
  }
}
