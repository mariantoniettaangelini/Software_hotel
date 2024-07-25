using Software_hotel.Models;

namespace Software_hotel.Services
{
    public interface IAuthService
    {
        User ValidateUser(string username, string password);
    }
}
