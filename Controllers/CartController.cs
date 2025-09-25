using Microsoft.AspNetCore.Mvc;
using RockawayFlower.Models;
using Stripe.Checkout;

namespace RockawayFlower.Controllers
{
    public class CartController : Controller
    {
        private const string CartKey = "CART";

        private List<CartItem> GetCart()
            => HttpContext.Session.GetJson<List<CartItem>>(CartKey) ?? new();

        private void SaveCart(List<CartItem> items)
            => HttpContext.Session.SetJson(CartKey, items);

        public IActionResult Index()
        {
            var cart = GetCart();
            return View(cart);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Add(string id, string name, int priceCents, int qty = 1)
        {
            if (string.IsNullOrWhiteSpace(id) || priceCents <= 0 || qty <= 0)
                return RedirectToAction(nameof(Index));

            var cart = GetCart();
            var existing = cart.FirstOrDefault(x => x.Id == id);
            if (existing is null)
                cart.Add(new CartItem { Id = id, Name = name, PriceCents = priceCents, Qty = qty });
            else
                existing.Qty += qty;

            SaveCart(cart);
            return RedirectToAction(nameof(Index));
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Update(Dictionary<string, int> qty)
        {
            var cart = GetCart();
            foreach (var kv in qty)
            {
                var item = cart.FirstOrDefault(x => x.Id == kv.Key);
                if (item is null) continue;
                var q = Math.Max(0, kv.Value);
                if (q == 0) cart.Remove(item);
                else item.Qty = q;
            }
            SaveCart(cart);
            return RedirectToAction(nameof(Index));
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Remove(string id)
        {
            var cart = GetCart();
            cart.RemoveAll(x => x.Id == id);
            SaveCart(cart);
            return RedirectToAction(nameof(Index));
        }

        // Stripe Checkout
        [HttpGet]
        public IActionResult Checkout()
        {
            var cart = GetCart();
            if (cart.Count == 0) return RedirectToAction(nameof(Index));


            var origin = $"{Request.Scheme}://{Request.Host}";
            var options = new SessionCreateOptions
            {
                Mode = "payment",
                SuccessUrl = $"{origin}/Cart/Success?session_id={{CHECKOUT_SESSION_ID}}",
                CancelUrl = $"{origin}/Cart/Index",
                LineItems = cart.Select(ci => new SessionLineItemOptions
                {
                    Quantity = ci.Qty,
                    PriceData = new SessionLineItemPriceDataOptions
                    {
                        Currency = "usd",
                        UnitAmount = ci.PriceCents,
                        ProductData = new SessionLineItemPriceDataProductDataOptions
                        {
                            Name = ci.Name
                        }
                    }
                }).ToList()
            };

            var service = new SessionService();
            var session = service.Create(options);

            return Redirect(session.Url);
        }

        [HttpGet]
        public IActionResult Success(string session_id)
        {
            // Optional: verify the session via Stripe API, write order to DB, send email, etc.
            HttpContext.Session.Remove(CartKey); // clear cart
            return View(model: session_id);
        }
    }
}






