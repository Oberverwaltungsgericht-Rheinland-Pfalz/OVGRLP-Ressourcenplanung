using DbRaumplanung.Models;
using System;
using System.Collections.Generic;
using System.Text;

namespace ServiceLayer.ServiceContracts
{
    public interface IUserService
    {
        User GetDetails(long id);
    }
}
