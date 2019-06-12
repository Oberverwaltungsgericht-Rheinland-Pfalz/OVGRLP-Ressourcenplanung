using DbRaumplanung.Contracts;
using DbRaumplanung.Models;
using System;
using System.Collections.Generic;
using System.Text;

namespace DbRaumplanung.DataAccess
{
    public class UserStore:IUserStore
    {
        private readonly RpDbContext _context;

        public UserStore(RpDbContext context)
        {
            _context = context;
        }

        public bool DeleteUserById(long id)
        {
            try
            {
                var user = _context.Users.Find(id);
                _context.Users.Remove(user);
                _context.SaveChanges();
                return true;
            }catch(Exception ex)
            {
                // todo: log error
                return false;
            }
        }

        public User GetUserById(long id)
        {
            var user = _context.Users.Find(id);
            return user;
        }

        public IEnumerable<User> GetUsersBySupplierGroup(long supplierGroupId)
        {
            throw new NotImplementedException();
        }

        public User SaveUser(User user)
        {
            var userSaved = _context.Users.Add(user);
            _context.SaveChanges();
            return userSaved.Entity;
        }

        public User UpdateUser(User user)
        {
            var updatedUser = _context.Users.Update(user);
            _context.SaveChanges();
            return updatedUser.Entity;
        }
    }
}
