﻿using Rema.Infrastructure.Models;

namespace Rema.Infrastructure.Contracts.Stores
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