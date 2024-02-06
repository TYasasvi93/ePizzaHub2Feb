using ePizzaHub.Core.Entities;
using ePizzaHub.Models;
using ePizzaHub.Services.Interfaces;
using ePizzaHub.UI.Helpers;
using Microsoft.AspNetCore.Mvc;

namespace ePizzaHub.UI.Controllers
{
    public class PaymentController : BaseController
    {
        IPaymentService _paymentService;
        IConfiguration _configuration;
        public PaymentController(IPaymentService paymentService, IConfiguration configuration)
        {
            _paymentService = paymentService;
            _configuration = configuration;
        }
        public IActionResult Index()
        {
            CartModel cartModel = TempData.Get<CartModel>("Cart");
            //AddressModel addressModel=TempData.Get<AddressModel>("Address");

            PaymentModel payment = new PaymentModel
            {
                GrandTotal = cartModel.GrandTotal,
                Description = string.Join(',', cartModel.Items.Select(x => x.Name)),
                Currency = "INR",
                RazorpayKey = _configuration["Razorpay:Key"],
                Receipt = Guid.NewGuid().ToString()
            };
            payment.OrderId = _paymentService.CreateOrder(payment.GrandTotal * 100, payment.Currency, payment.Receipt);


            return View(payment);
        }

        public IActionResult Status(IFormCollection form)
        {
            string paymentId = form["rzp_paymentid"];
            string orderId = form["rzp_orderid"];
            string signature = form["rzp_signature"];
            string transactionId = form["Receipt"];
            string currency = form["Currency"];

            var payment = _paymentService.GetPaymentDetails(paymentId);
            bool isValid = _paymentService.VerifySignature(signature, orderId, paymentId);
            if (isValid && payment != null)
            {
                CartModel cart = TempData.Peek<CartModel>("Cart");
                PaymentDetail model = new PaymentDetail();

                model.CartId = cart.Id;
                model.Total = cart.Total;
                model.Tax = cart.Tax;
                model.GrandTotal = cart.GrandTotal;
                model.CreatedDate = DateTime.Now;

                model.Status = payment.Attributes["status"]; //captured
                model.TransactionId = transactionId;
                model.Currency = payment.Attributes["currency"];
                model.Email = payment.Attributes["email"];
                model.Id = paymentId;
                model.UserId = CurrentUser.Id;

                var response = _paymentService.SavePaymentDetails(model);
                if (response > 0)
                {
                    Response.Cookies.Append("CartId", ""); //remove cart cookie
                    TempData["Cart"] = null;
                    TempData.Set("PaymentDetail", model);
                    var adress = TempData.Get<AddressModel>("Address");
                    //_orderService.PlaceOrder(CurrentUser.Id, orderId, paymentId, cart, adress);
                    return RedirectToAction("Receipt", "Payment");
                }
            }
            return View();
        }
        public IActionResult Receipt()
        {
            PaymentDetail model = TempData.Get<PaymentDetail>("PaymentDetail");
            return View(model);
        }
    }
}
