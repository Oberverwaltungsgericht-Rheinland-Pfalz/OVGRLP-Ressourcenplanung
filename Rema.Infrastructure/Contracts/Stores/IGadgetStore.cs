using System.Collections.Generic;
using Rema.Infrastructure.Models;

namespace Rema.Infrastructure.Contracts.Stores
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
