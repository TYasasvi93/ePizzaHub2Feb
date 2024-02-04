using ePizzaHub.Core.Entities;
using ePizzaHub.Models;
using ePizzaHub.Repositories.Interfaces;
using ePizzaHub.Services.Interfaces;

namespace ePizzaHub.Services.Implementation
{
    internal class UserService : Service<User>, IUserService
    {
        private readonly IUserRepository _userRepo;
        public UserService(IUserRepository userRepo) : base(userRepo)
        {
            _userRepo = userRepo;
        }
        public bool CreateUser(User user, string role)
        {
            _userRepo.CreateUser(user, role);
            return true;
        }

        public UserModel ValidateUser(string email, string password)
        {
            return _userRepo.ValidateUser(email, password);
        }
    }
}
