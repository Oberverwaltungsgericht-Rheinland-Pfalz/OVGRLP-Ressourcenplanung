using DbRaumplanung.Contracts;
using DbRaumplanung.Models;
using ServiceLayer.ServiceContracts;
using System;
using System.Collections.Generic;
using System.Text;

namespace ServiceLayer.Services
{
    public class UserService : IUserService
    {
        private readonly IUserStore _userStore;

        public UserService(IUserStore userStore)
        {
            _userStore = userStore;
        }

        public User GetDetails(long id)
        {
            return _userStore.GetUserById(id);
        }
    }
}
