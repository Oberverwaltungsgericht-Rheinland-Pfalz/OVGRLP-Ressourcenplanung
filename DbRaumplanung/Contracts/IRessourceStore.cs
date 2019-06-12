using DbRaumplanung.Models;
using System;
using System.Collections.Generic;
using System.Text;

namespace DbRaumplanung.Contracts
{
    public interface IRessourceStore
    {
        Ressource GetRessourceById(long id);
        Ressource GetRessourceByName(string name);

    }
}
