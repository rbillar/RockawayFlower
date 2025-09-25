using Microsoft.AspNetCore.Mvc;
using RockawayFlower.Models;

namespace RockawayFlower.Controllers
{
    public class ShopController : Controller
    {
        // Replace with DB fetch later
        private static readonly List<Product> _products = new()
    {
        new() { Id = "sku_rose12", Name = "Rose Bouquet (12)", PriceCents = 4999, Description = "12 long-stem roses" },
        new() { Id = "sku_tulip",  Name = "Tulip Mix",         PriceCents = 3499, Description = "Spring tulip assortment" }
    };

        public IActionResult Index() => View(_products);
    }
}
