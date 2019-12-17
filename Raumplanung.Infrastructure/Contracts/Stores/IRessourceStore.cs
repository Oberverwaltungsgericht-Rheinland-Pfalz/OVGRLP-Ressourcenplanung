﻿using Raumplanung.Infrastructure.Models;
using System;
using System.Collections.Generic;
using System.Text;

namespace Raumplanung.Infrastructure.Contracts.Stores
{
  public interface IRessourceStore
  {
    Ressource CreateRessource(Ressource ressource);

    Ressource GetRessourceById(long id);

    Ressource GetRessourceByName(string name);

    Ressource UpdateRessource(Ressource ressource);

    bool DeleteRessource(long id);
  }
}