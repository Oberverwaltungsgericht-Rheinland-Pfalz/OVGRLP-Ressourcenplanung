using Raumplanung.Infrastructure.Models;
using System;
using System.Collections.Generic;
using System.Text;

namespace Raumplanung.Infrastructure.Contracts.Services
{
  public interface IUserService
  {
    User GetDetails(long id);
  }
}