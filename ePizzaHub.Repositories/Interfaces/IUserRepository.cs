using ePizzaHub.Core.Entities;
using ePizzaHub.Models;

namespace ePizzaHub.Repositories.Interfaces
{
    public interface IUserRepository : IRepository<User>
    {
        bool CreateUser(User user, string role);
        UserModel ValidateUser(string email, string password);
    }
}
