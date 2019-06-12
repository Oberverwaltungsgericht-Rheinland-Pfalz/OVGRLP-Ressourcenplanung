using DbRaumplanung.Models;
using System;
using System.Collections.Generic;
using System.Text;

namespace DbRaumplanung.Contracts
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
