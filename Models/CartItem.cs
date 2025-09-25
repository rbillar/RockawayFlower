using Microsoft.AspNetCore.Mvc;

namespace RockawayFlower.Models
{
    public class CartItem 
    {
        public string Id { get; set; } = default!;
        public string Name { get; set; } = default!;
        public int PriceCents { get; set; }   // store cents as int
        public int Qty { get; set; }
    }
}
