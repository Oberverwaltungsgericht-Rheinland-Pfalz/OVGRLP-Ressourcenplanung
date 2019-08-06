using DbRaumplanung.Contracts;
using DbRaumplanung.Models;
using System;
using System.Collections.Generic;
using System.Text;

namespace DbRaumplanung.DataAccess
{
    public class SupplierGroupStore : ISupplierGroupStore
    {
        private readonly RpDbContext _context;
        public SupplierGroupStore(RpDbContext context)
        {
            _context = context;
        }

        public SupplierGroup CreateSupplierGroup(string name, string groupEmail)
        {
            var supplierGroup = new SupplierGroup() { Title = name, GroupEmail = groupEmail };
            var savedEntity = _context.SupplierGroups.Add(supplierGroup);
            return savedEntity.Entity;
        }

        public bool DeleteSupplierGroup(long id)
        {
            try
            {
                var supplierGroup= _context.SupplierGroups.Find(id);
                _context.SupplierGroups.Remove(supplierGroup);
                _context.SaveChanges();
                return true;
            }
            catch (Exception ex)
            {
                // todo: log error
                return false;
            }
        }

        public SupplierGroup GetSupplierGroupById(long id)
        {
            var supplierGroup = _context.SupplierGroups.Find(id);
            return supplierGroup;
        }

        public SupplierGroup UpdateSupplierGroup(SupplierGroup supplierGroup)
        {
            var updatedEntity = _context.SupplierGroups.Update(supplierGroup);
            _context.SaveChanges();
            return updatedEntity.Entity;
        }
    }
}
