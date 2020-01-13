﻿using System;
using System.Collections.Generic;
using Raumplanung.Infrastructure.Models;

namespace Raumplanung.Infrastructure.Contracts.Services
{
  public interface IRessourceService
  {
    IEnumerable<Allocation> GetAllocations(long ressourceId, DateTime? from = null, DateTime? to = null);

    IEnumerable<Allocation> GetAllocationsThisYear(long ressourceId);

    IEnumerable<Allocation> GetAllocationsThisMonth(long ressourceId);

    Ressource GetDetails(long id);

    IEnumerable<Gadget> GetAssignedGadgets(long ressourceId);
  }
}
