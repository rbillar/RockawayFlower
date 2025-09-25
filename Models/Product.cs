using Microsoft.AspNetCore.Mvc;

namespace RockawayFlower.Models
{
    public class Product
    {
        public string Id { get; set; } = default!;
        public string Name { get; set; } = default!;
        public int PriceCents { get; set; }
        public string? Description { get; set; }
        public bool IsActive { get; set; } = true;
    }
}
