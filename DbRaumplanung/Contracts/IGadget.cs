using DbRaumplanung.Models;
using System;
using System.Collections.Generic;
using System.Text;

namespace DbRaumplanung.Contracts
{
    public interface IGadgets
    {
        Gadget GetGadgetById(long id);
        IEnumerable<Gadget> GetGadgetsBySupplierGroup(long supplierGroupId);
    }
}
