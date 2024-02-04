using ePizzaHub.Core.Entities;
using ePizzaHub.Models;

namespace ePizzaHub.Services.Interfaces
{
    public interface IUserService : IService<User>
    {
        bool CreateUser(User user, string role);
        UserModel ValidateUser(string email, string password);
    }
}
