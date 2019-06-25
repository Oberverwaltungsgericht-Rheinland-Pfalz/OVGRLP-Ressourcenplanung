using DbRaumplanung.Contracts;
using DbRaumplanung.Models;
using System;
using System.Collections.Generic;
using System.Text;
using System.Linq;

namespace DbRaumplanung.DataAccess
{
    public class GadgetStore: IGadgetStore
    {
        private readonly RpDbContext _context;

        public GadgetStore(RpDbContext context)
        {
            _context = context;
        }

        public Gadget CreateGadget(Gadget gadget)
        {
            var entity = _context.Gadgets.Add(gadget);
            _context.SaveChanges();
            return entity.Entity;
        }

        public bool DeleteGadget(long id)
        {
            try
            {
                _context.Remove(_context.Gadgets.Single(a => a.Id == id));
                _context.SaveChanges();
                return true;
            }
            catch (Exception ex)
            {
                // todo: log error
                return false;
            }
        }

        public Gadget GetGadgetById(long id)
        {
            var entity = _context.Gadgets.Find(id);
            return entity;
        }
        public IEnumerable<Gadget> GetGadgets()
        {
            return _context.Gadgets.ToList();
        }

        public IEnumerable<Gadget> GetGadgetsBySupplierGroup(long supplierGroupId)
        {
            return _context.Gadgets
                .Where(e => e.SuppliedBy.Id == supplierGroupId)
                .ToList();

            //var query = (
            //    from gadget in _context.Gadgets
            //    where gadget.SuppliedBy.Id == supplierGroupId
            //    select new Gadget
            //    {
            //        Id= gadget.Id,
            //        Name = gadget.Name,
            //        SuppliedBy = gadget.SuppliedBy
            //    });
            //return query.ToList();
        }

        public IEnumerable<Gadget> GetGadgetsByRessourceId(long ressourceId)
        {
            return _context.Ressources.Where(s => s.Id == ressourceId).SelectMany(e => e.Gadgets);
        }
        public Gadget UpdateGadget(Gadget gadget)
        {
            var entity = _context.Gadgets.Update(gadget);
            _context.SaveChanges();
            return entity.Entity;
        }
    }
}
