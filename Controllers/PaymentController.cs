using Microsoft.AspNetCore.Mvc;
using Stripe.Checkout;

namespace StripePaymentDemoApp.Controllers
{
    public class PaymentController : Controller
    {
        private readonly IConfiguration _configuration;

        public PaymentController(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        public IActionResult Checkout()
        {
            // Pass the Stripe Publishable Key to the view
            ViewBag.StripePublishableKey = _configuration["Stripe:PublishableKey"];
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> CreateCheckoutSession()
        {
            // Create a Stripe Checkout Session
            var options = new SessionCreateOptions
            {
                PaymentMethodTypes = new List<string> { "card" },
                LineItems = new List<SessionLineItemOptions>
                {
                    new SessionLineItemOptions
                    {
                        PriceData = new SessionLineItemPriceDataOptions
                        {
                            Currency = "usd",
                            ProductData = new SessionLineItemPriceDataProductDataOptions
                            {
                                Name = "Test Product",
                            },
                            UnitAmount = 2000, // $20.00 (in cents)
                        },
                        Quantity = 1,
                    },
                },
                Mode = "payment",
                SuccessUrl = Url.Action("Success", "Payment", null, Request.Scheme),
                CancelUrl = Url.Action("Cancel", "Payment", null, Request.Scheme),
            };

            var service = new SessionService();
            var session = await service.CreateAsync(options);

            // Redirect to Stripe Checkout
            return Redirect(session.Url);
        }

        public IActionResult Success()
        {
            return View();
        }

        public IActionResult Cancel()
        {
            return View();
        }
    }
}