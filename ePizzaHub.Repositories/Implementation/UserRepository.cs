using ePizzaHub.Core;
using ePizzaHub.Core.Entities;
using ePizzaHub.Models;
using ePizzaHub.Repositories.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace ePizzaHub.Repositories.Implementation
{
    public class UserRepository : Repository<User>, IUserRepository
    {
        public UserRepository(AppDbContext db) : base(db)
        {

        }
        public bool CreateUser(User user, string role)
        {
            try
            {
                user.Password = BCrypt.Net.BCrypt.HashPassword(user.Password);
                _db.Users.Add(user);
                _db.SaveChanges();
                return true;
            }
            catch (Exception ex)
            {

                throw;
                return false;
            }
        }

        public UserModel ValidateUser(string email, string password)
        {
            try
            {
                var user = _db.Users.Include(u => u.Roles).Where(u => u.Email == email).FirstOrDefault();
                if (user != null)
                {
                    bool isVerified = BCrypt.Net.BCrypt.Verify(password, user.Password);
                    if (isVerified)
                    {
                        UserModel userModel = new UserModel();
                        userModel.Id = user.Id;
                        userModel.Name = user.Name;
                        userModel.Email = user.Email;
                        userModel.PhoneNumber = user.PhoneNumber;
                        userModel.Roles = user.Roles.Select(u => u.Name).ToArray();
                        return userModel;
                    }
                }
                return null;

            }
            catch (Exception ex)
            {
                //throw;
                return null;
            }
        }
    }
}
