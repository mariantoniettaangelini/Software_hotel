
using Software_hotel.Models;

namespace Software_hotel.Services
{
    public class AuthService : SqlServiceSvcBase, IAuthService
    {
        public AuthService(IConfiguration config) : base(config)
        {
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
