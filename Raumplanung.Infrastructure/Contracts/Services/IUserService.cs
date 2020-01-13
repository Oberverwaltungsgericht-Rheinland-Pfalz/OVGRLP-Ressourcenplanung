using Raumplanung.Infrastructure.Models;

namespace Raumplanung.Infrastructure.Contracts.Services
{
  public interface IUserService
  {
    User GetDetails(long id);
  }
}
