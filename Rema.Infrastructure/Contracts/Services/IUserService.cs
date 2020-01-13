using Rema.Infrastructure.Models;

namespace Rema.Infrastructure.Contracts.Services
{
  public interface IUserService
  {
    User GetDetails(long id);
  }
}
