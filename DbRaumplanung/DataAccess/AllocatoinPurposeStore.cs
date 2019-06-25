using DbRaumplanung.Contracts;
using DbRaumplanung.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace DbRaumplanung.DataAccess
{
    public class AllocatoinPurposeStore: IAllocationPurposeStore
    {
        private readonly RpDbContext _context;
        public AllocatoinPurposeStore(RpDbContext context)
        {
            _context = context;
        }

        public AllocationPurpose CreateAllocationPurpose(AllocationPurpose allocationPurpose)
        {
            var entity = _context.AllocationPurposes.Add(allocationPurpose);
            _context.SaveChanges();
            return entity.Entity;
        }

        public IEnumerable<AllocationPurpose> GetPurposes()
        {
            return _context.AllocationPurposes.ToList();
        }

        public AllocationPurpose GetAllocationPurposeById(long id)
        {
            var entity = _context.AllocationPurposes.Find(id);
            return entity;
        }

        public AllocationPurpose UpdateAllocationPurpose(AllocationPurpose allocationPurpose)
        {
            var entity = _context.AllocationPurposes.Update(allocationPurpose);
            _context.SaveChanges();
            return entity.Entity;
        }

        public bool DeleteAllocationPurpose(long id)
        {
            try
            {
                var entity = _context.AllocationPurposes.Find(id);
                _context.AllocationPurposes.Remove(entity);
                _context.SaveChanges();
                return true;
            }
            catch (Exception ex)
            {
                // todo: log error
                return false;
            }
        }

    }
}
