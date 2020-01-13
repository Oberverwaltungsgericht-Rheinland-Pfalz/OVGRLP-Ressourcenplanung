using System.Collections.Generic;
using Raumplanung.Infrastructure.Models;

namespace Raumplanung.Infrastructure.Contracts.Stores
{
  public interface IGadgetStore
  {
    Gadget CreateGadget(Gadget gadget);

    IEnumerable<Gadget> GetGadgets();

    Gadget GetGadgetById(long id);

    IEnumerable<Gadget> GetGadgetsBySupplierGroup(long supplierGroupId);

    Gadget UpdateGadget(Gadget gadget);

    bool DeleteGadget(long id);

    IEnumerable<Gadget> GetGadgetsByRessourceId(long ressourceId);
  }
}
