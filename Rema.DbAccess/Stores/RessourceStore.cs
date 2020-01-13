using System.Linq;
using Rema.Infrastructure.Contracts.Stores;
using Rema.Infrastructure.Models;

namespace Rema.DbAccess.Stores
{
  public class RessourceStore : IRessourceStore
  {
    private readonly RpDbContext _context;

    public RessourceStore(RpDbContext context)
    {
      _context = context;
    }

    public Ressource CreateRessource(Ressource ressource)
    {
      var savedEntity = _context.Ressources.Add(ressource);
      _context.SaveChanges();
      return savedEntity.Entity;
    }

    public bool DeleteRessource(long id)
    {
      try
      {
        var ressource = _context.Ressources.Find(id);
        _context.Ressources.Remove(ressource);
        _context.SaveChanges();
        return true;
      }
      catch
      {
        // todo: log error
        return false;
      }
    }

    public Ressource GetRessourceById(long id)
    {
      var ressource = _context.Ressources.Find(id);
      return ressource;
    }

    public Ressource GetRessourceByName(string name)
    {
      var query = (
        from ressource in _context.Ressources
        where ressource.Name == name
        select new Ressource
        {
          Id = ressource.Id,
          Name = ressource.Name,
          Gadgets = ressource.Gadgets,
          Usability = ressource.Usability,
          FunctionDescription = ressource.FunctionDescription,
          SpecialsDescription = ressource.SpecialsDescription
        }); ;

      return query.SingleOrDefault();
    }

    public Ressource UpdateRessource(Ressource ressource)
    {
      var updatedEntity = _context.Ressources.Update(ressource);
      _context.SaveChanges();
      return updatedEntity.Entity;
    }
  }
}
