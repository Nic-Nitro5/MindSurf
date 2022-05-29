using MindSurf.Models;
using System.Linq;

namespace MindSurf.Data
{
    public class UserRepository : IUserRepository
    {
        private readonly UserContext _context;
        public UserRepository(UserContext context)
        {
            _context = context;
        }
        public User Create(User user)
        {
            _context.Users.Add(user);
            user.Id = _context.SaveChanges();

            return user;
        }

        public User UpdateUser(User user)
        {
            var attemtingUser = GetByEmail(user.Email);

            if (attemtingUser != null)
            {
                attemtingUser.RememberMe = user.RememberMe;

                _context.Users.Update(user);
                _context.SaveChanges();
            }         

            return user;
        }

        public User GetByEmail(string email) {
            return _context.Users.FirstOrDefault(u => u.Email == email);
        }

        public User GetById(int id) { 
            return _context.Users.FirstOrDefault(u => u.Id == id);
        }
    }
}
