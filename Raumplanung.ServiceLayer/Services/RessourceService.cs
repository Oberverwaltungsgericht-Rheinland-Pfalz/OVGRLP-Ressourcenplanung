using System;
using System.Collections.Generic;
using Raumplanung.Infrastructure.Contracts.Services;
using Raumplanung.Infrastructure.Contracts.Stores;
using Raumplanung.Infrastructure.Models;

namespace ServiceLayer.Services
{
  public class RessourceService : IRessourceService
  {
    //private readonly IAllocationPurposeStore _allocationPurposeStore;
    private readonly IAllocationStore _allocationStore;

    private readonly IRessourceStore _ressourceStore;
    private readonly IGadgetStore _gadgetStore;
    //private readonly IUserStore _userStore;

    public RessourceService(IAllocationStore allocationStore, IRessourceStore ressourceStore, IGadgetStore gadgetStore)
    {
      _allocationStore = allocationStore;
      _ressourceStore = ressourceStore;
      _gadgetStore = gadgetStore;
    }

    public IEnumerable<Allocation> GetAllocations(long ressourceId, DateTime? from = null, DateTime? to = null)
    {
      return this._allocationStore.GetAllocationsByRessource(ressourceId, from, to);
    }

    public IEnumerable<Allocation> GetAllocationsThisMonth(long ressourceId)
    {
      int year = DateTime.Now.Year;
      int month = DateTime.Now.Month;
      DateTime firstDay = new DateTime(year, month, 1);
      DateTime lastDay = new DateTime(year, month, DateTime.DaysInMonth(year, month), 23, 59, 59);
      return this._allocationStore.GetAllocationsByRessource(ressourceId, firstDay, lastDay);
    }

    public IEnumerable<Allocation> GetAllocationsThisYear(long ressourceId)
    {
      int year = DateTime.Now.Year;
      int month = DateTime.Now.Month;
      DateTime firstDay = new DateTime(year, month, 1);
      DateTime lastDay = new DateTime(year, month, DateTime.DaysInMonth(year, month), 23, 59, 59);

      return this._allocationStore.GetAllocationsByRessource(ressourceId, firstDay, lastDay);
    }

    public IEnumerable<Gadget> GetAssignedGadgets(long ressourceId)
    {
      return _gadgetStore.GetGadgetsByRessourceId(ressourceId);
    }

    public Ressource GetDetails(long id)
    {
      return _ressourceStore.GetRessourceById(id);
    }
  }
}
