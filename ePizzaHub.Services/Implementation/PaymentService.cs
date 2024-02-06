using ePizzaHub.Core.Entities;
using ePizzaHub.Repositories.Interfaces;
using ePizzaHub.Services.Interfaces;
using Microsoft.Extensions.Configuration;
using Razorpay.Api;
using System.Security.Cryptography;
using System.Text;

namespace ePizzaHub.Services.Implementation
{
    public class PaymentService : IPaymentService
    {
        private readonly RazorpayClient _razorpayClient;
        IConfiguration _configuration;
        ICartRepository _cartRepository;
        IRepository<PaymentDetail> _paymentRepo;
        public PaymentService(IConfiguration configuration, IRepository<PaymentDetail> paymetRepo, ICartRepository cartRepository)
        {
            _configuration = configuration;
            _razorpayClient = new RazorpayClient(_configuration["RazorPay:Key"], _configuration["RazorPay:Secret"]);
            _paymentRepo = paymetRepo;
            _cartRepository = cartRepository;
        }
        public string CreateOrder(decimal amount, string currency, string receipt)
        {
            Dictionary<string, object> options = new Dictionary<string, object>();
            options.Add("amount", amount);
            options.Add("receipt", receipt);
            options.Add("currency", currency);
            Razorpay.Api.Order order = _razorpayClient.Order.Create(options);

            return order["id"].ToString();
        }

        public Payment GetPaymentDetails(string paymentId)
        {
            return _razorpayClient.Payment.Fetch(paymentId);
        }

        public int SavePaymentDetails(PaymentDetail model)
        {
            _paymentRepo.Add(model);
            var cart = _cartRepository.GetCart(model.CartId);
            cart.IsActive = false;
            return _paymentRepo.SaveChanges();

        }

        public bool VerifySignature(string signature, string orderId, string paymentId)
        {
            string payload = string.Format("{0}|{1}", orderId, paymentId);
            string secret = RazorpayClient.Secret;
            string actualSignature = getActualSignature(payload, secret);
            return actualSignature.Equals(signature);
        }

        private static string getActualSignature(string payload, string secret)
        {
            byte[] secretBytes = StringEncode(secret);
            HMACSHA256 hashHmac = new HMACSHA256(secretBytes);
            var bytes = StringEncode(payload);

            return HashEncode(hashHmac.ComputeHash(bytes));
        }

        private static byte[] StringEncode(string text)
        {
            var encoding = new ASCIIEncoding();
            return encoding.GetBytes(text);
        }

        private static string HashEncode(byte[] hash)
        {
            return BitConverter.ToString(hash).Replace("-", "").ToLower();
        }
    }
}
