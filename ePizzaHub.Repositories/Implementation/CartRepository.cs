using ePizzaHub.Core;
using ePizzaHub.Core.Entities;
using ePizzaHub.Models;
using ePizzaHub.Repositories.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace ePizzaHub.Repositories.Implementation
{
    public class CartRepository : Repository<Cart>, ICartRepository
    {
        public CartRepository(AppDbContext db) : base(db)
        {
        }

        public int DeleteItem(Guid CartId, int Id)
        {
            var item = _db.CartItems.Where(x => x.CartId == CartId && x.Id == Id).FirstOrDefault();
            if (item != null)
            {
                _db.CartItems.Remove(item);
                return _db.SaveChanges();
            }
            return 0;
        }

        public Cart GetCart(Guid CartId)
        {
            return _db.Carts.Include(c => c.CartItems).Where(c => c.Id == CartId && c.IsActive == true).FirstOrDefault();
        }

        public CartModel GetCartDetails(Guid CartId)
        {
            var model = (from cart in _db.Carts
                         where cart.Id == CartId && cart.IsActive == true
                         select new CartModel
                         {
                             Id = cart.Id,
                             UserId = cart.UserId,
                             CreatedDate = cart.CreatedDate,
                             Items = (from cartItem in _db.CartItems
                                      join item in _db.Items
                                      on cartItem.ItemId equals item.Id
                                      where cartItem.CartId == CartId
                                      select new ItemModel
                                      {
                                          Id = cartItem.Id,
                                          Quantity = cartItem.Quantity,
                                          UnitPrice = cartItem.UnitPrice,
                                          ItemId = item.Id,
                                          Name = item.Name,
                                          Description = item.Description,
                                          ImageUrl = item.ImageUrl
                                      }).ToList()
                         }).FirstOrDefault();
            return model;
        }

        public int UpdateCart(Guid cartId, int userId)
        {
            Cart cart = _db.Carts.Where(cart => cart.Id == cartId && cart.IsActive == true).FirstOrDefault();
            if (cart != null)
            {
                cart.UserId = userId;
                return _db.SaveChanges();
            }
            return 0;
        }

        public int UpdateQuantity(Guid CartId, int itemId, int quantity)
        {
            bool flag = false;
            var cart = GetCart(CartId);
            if (cart != null)
            {
                var cartItems = cart.CartItems.ToList();
                for (int i = 0; i < cart.CartItems.Count; i++)
                {
                    if (cartItems[i].ItemId == itemId)
                    {
                        cartItems[i].Quantity += quantity;
                        flag = true;
                        break;
                    }

                }
                if (flag)
                {
                    cart.CartItems = cartItems;
                    return _db.SaveChanges();
                }
            }
            return 0;
        }
    }
}
