using ePizzaHub.Core.Entities;
using ePizzaHub.Models;
using ePizzaHub.Services.Interfaces;
using ePizzaHub.UI.Helpers;
using Microsoft.AspNetCore.Mvc;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace ePizzaHub.UI.Controllers
{
    public class CartController : BaseController
    {
        ICartService _cartService;
        public CartController(ICartService cartService)
        {
            _cartService = cartService;
        }

        Guid CartId
        {
            get
            {
                Guid cartId = Guid.Empty;
                if (Request.Cookies["CartId"] == null)
                {
                    cartId = Guid.NewGuid();
                    Response.Cookies.Append("CartId", cartId.ToString());
                }
                else
                {
                    cartId = Guid.Parse(Request.Cookies["CartId"]);
                }
                return cartId;
            }
        }
        public IActionResult Index()
        {
            CartModel model = _cartService.GetCartDetails(CartId);
            return View(model);
        }
        [Route("Cart/AddToCart/{itemId}/{unitPrice}/{quantity}")]
        public JsonResult AddToCart(int itemId, decimal unitPrice, int quantity)
        {
            if (itemId > 0 && unitPrice > 0 && quantity > 0)
            {
                int userId = CurrentUser != null ? CurrentUser.Id : 0;
                if (User.Identity.IsAuthenticated)
                {
                    userId = int.Parse(User.Identity.Name);
                }
                Cart cart = _cartService.AddItem(userId, CartId, itemId, unitPrice, quantity);
                JsonSerializerOptions options = new JsonSerializerOptions
                {
                    ReferenceHandler = ReferenceHandler.IgnoreCycles
                };
                var data = JsonSerializer.Serialize(cart, options);
                return Json(data);
            }
            return Json("");
        }

        [Route("Cart/UpdateQuantity/{id}/{quantity}")]
        public IActionResult UpdateQuantity(int Id, int Quantity)
        {
            int count = _cartService.UpdateQuantity(CartId, Id, Quantity);
            return Json(count);
        }
        [Route("Cart/DeleteItem/{id}")]
        public IActionResult DeleteItem(int id)
        {
            int count = _cartService.DeleteItem(CartId, id);
            return Json(count);
        }

        public IActionResult GetCartCount()
        {
            if (CartId != null)
            {
                int count = _cartService.GetCartCount(CartId);
                return Json(count);
            }
            return Json(0);
        }

        public IActionResult CheckOut()
        {
            return View();
        }
        [HttpPost]
        public IActionResult CheckOut(AddressModel model)
        {
            if (ModelState.IsValid)
            {
                if (CurrentUser != null)
                {
                    CartModel cart = _cartService.GetCartDetails(CartId);
                    _cartService.UpdateCart(CartId, CurrentUser.Id);
                    TempData.Set("Cart", cart);
                    TempData.Set("Address", model);
                    return RedirectToAction("Index", "Payment");
                }
            }
            return View();
        }

    }
}
