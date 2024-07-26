
using Software_hotel.Models;

namespace Software_hotel.Services
{
    public class AuthService : SqlServiceSvcBase, IAuthService
    {
        public AuthService(IConfiguration config) : base(config)
        {
            _users = new List<User>
            {
                new User {Username = "admin", Password = "123", Role = "Admin"}
            };
        }

        private readonly List<User> _users = new List<User>();
        public User ValidateUser(string username, string password)
        {
            foreach (var user in _users)
            {
                if (user.Username == username && user.Password == password)
                {
                    return user;
                }
            }
                return null;
        }
    }
}
