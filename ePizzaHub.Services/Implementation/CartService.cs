using ePizzaHub.Core.Entities;
using ePizzaHub.Models;
using ePizzaHub.Repositories.Interfaces;
using ePizzaHub.Services.Interfaces;
using Microsoft.Extensions.Configuration;

namespace ePizzaHub.Services.Implementation
{
    public class CartService : Service<Cart>, ICartService
    {
        ICartRepository _cartRepo;
        IRepository<CartItem> _cartItem;
        IConfiguration _config;
        public CartService(ICartRepository cartRepo, IRepository<CartItem> cartItem, IConfiguration config) : base(cartRepo)
        {
            _config = config;
            _cartItem = cartItem;
            _cartRepo = cartRepo;

        }

        public Cart AddItem(int UserId, Guid CartId, int ItemId, decimal UnitPrice, int Quantity)
        {
            try
            {
                Cart cart = _cartRepo.GetCart(CartId);
                if (cart == null)
                {
                    cart = new Cart();
                    CartItem cartItem = new CartItem { ItemId = ItemId, Quantity = Quantity, UnitPrice = UnitPrice, CartId = CartId };

                    cart.Id = CartId;
                    cart.UserId = UserId;
                    cart.CreatedDate = System.DateTime.Now;
                    cart.IsActive = true;

                    cart.CartItems.Add(cartItem);
                    _cartRepo.Add(cart);
                    _cartRepo.SaveChanges();
                }
                else
                {
                    CartItem cartItem = cart.CartItems.Where(x => x.ItemId == ItemId).FirstOrDefault();
                    if (cartItem == null)
                    {
                        cartItem = new CartItem { ItemId = ItemId, Quantity = Quantity, UnitPrice = UnitPrice, CartId = CartId };
                        cart.CartItems.Add(cartItem);
                        _cartItem.Update(cartItem);
                        _cartItem.SaveChanges();
                    }
                    else
                    {
                        cartItem.Quantity += Quantity;
                        _cartItem.Update(cartItem);
                        _cartItem.SaveChanges();
                    }
                }
                return cart;
            }
            catch (Exception ex) { }
            return null;
        }

        public int DeleteItem(Guid CartId, int itemId)
        {
            return _cartRepo.DeleteItem(CartId, itemId);
        }

        public int GetCartCount(Guid CartId)
        {
            var cart = _cartRepo.GetCart(CartId);
            if (cart != null)
            {
                return cart.CartItems.Count;
            }
            return 0;
        }

        public CartModel GetCartDetails(Guid CartId)
        {
            var model = _cartRepo.GetCartDetails(CartId);
            if (model != null && model.Items.Count > 0)
            {
                decimal subTotal = 0;
                foreach (var item in model.Items)
                {
                    item.Total = item.UnitPrice * item.Quantity;
                    subTotal += item.Total;

                }
                model.Total = subTotal;
                model.Tax = Math.Round((model.Total * Convert.ToInt32(_config["Tax:GST"])) / 100, 2);
                model.GrandTotal = model.Tax + model.Total;
            }
            return model;
        }

        public int UpdateCart(Guid cartId, int userId)
        {
            return _cartRepo.UpdateCart(cartId, userId);
        }

        public int UpdateQuantity(Guid cartId, int itemId, int Quantity)
        {
            return _cartRepo.UpdateQuantity(cartId, itemId, Quantity);
        }
    }
}
