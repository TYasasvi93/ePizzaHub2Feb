using ePizzaHub.Core;
using ePizzaHub.Core.Entities;
using ePizzaHub.Repositories.Implementation;
using ePizzaHub.Repositories.Interfaces;
using ePizzaHub.Services.Implementation;
using ePizzaHub.Services.Interfaces;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace ePizzaHub.Services
{
    public class ConfigureServices
    {
        public static void Configure(IServiceCollection service, IConfiguration configuration)
        {
            service.AddDbContext<AppDbContext>(options =>
            {
                options.UseSqlServer(configuration.GetConnectionString("DbConnection"));

            });

            //repo
            service.AddScoped<IRepository<Category>, Repository<Category>>();
            service.AddScoped<IRepository<User>, Repository<User>>();
            service.AddScoped<IRepository<Item>, Repository<Item>>();
            service.AddScoped<IRepository<CartItem>, Repository<CartItem>>();
            service.AddScoped<IRepository<PaymentDetail>, Repository<PaymentDetail>>();

            service.AddScoped<IUserRepository, UserRepository>();
            service.AddScoped<ICartRepository, CartRepository>();
            service.AddScoped<IOrderRepository, OrderRepository>();

            //services
            service.AddScoped<IUserService, UserService>();
            service.AddScoped<IItemService, ItemService>();
            service.AddScoped<ICartService, CartService>();
            service.AddScoped<IPaymentService, PaymentService>();
            service.AddScoped<IOrderService, OrderService>();


        }
    }
}
