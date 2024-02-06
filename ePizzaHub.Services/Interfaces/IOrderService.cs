using ePizzaHub.Core.Entities;
using ePizzaHub.Models;

namespace ePizzaHub.Services.Interfaces
{
    internal interface IOrderService : IService<Order>

    {
        OrderModel GetOrderDetails(string OrderId);
        IEnumerable<Order> GetUserOrders(int USerId);
        int PlacrOrder(int userId, string orderId, string paymentId, CartModel cart, AddressModel address);
    }
}
