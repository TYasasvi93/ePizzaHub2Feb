using ePizzaHub.Core;
using ePizzaHub.Core.Entities;
using ePizzaHub.Models;
using ePizzaHub.Repositories.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace ePizzaHub.Repositories.Implementation
{
    public class OrderRepository : Repository<Order>, IOrderRepository
    {
        public OrderRepository(AppDbContext db) : base(db)
        {
        }

        public OrderModel GetOrderDetails(string id)
        {
            var model = (from order in _db.Orders
                         join payment in _db.PaymentDetails
                         on order.PaymentId equals payment.Id
                         where order.Id == id
                         select new OrderModel
                         {
                             Id = order.Id,
                             UserId = order.UserId,
                             CreatedDate = order.CreatedDate,
                             Locality = order.Locality,
                             Total = payment.Total,
                             Tax = payment.Tax,
                             GrandTotal = payment.GrandTotal,
                             Items = (from orderItem in _db.OrderItems
                                      join item in _db.Items
                                      on orderItem.ItemId equals item.Id
                                      where orderItem.OrderId == id
                                      select new ItemModel
                                      {
                                          Id = orderItem.Id,
                                          Quantity = orderItem.Quantity,
                                          UnitPrice = orderItem.UnitPrice,
                                          ItemId = item.Id,
                                          Name = item.Name,
                                          Description = item.Description,
                                          ImageUrl = item.ImageUrl
                                      }).ToList()
                         }).FirstOrDefault();
            return model;
        }

        public IEnumerable<Order> GetUserOrders(int UserId)
        {
            return _db.Orders.Include(o => o.OrderItems).Where(o => o.UserId == UserId).ToList();
        }
    }
}
