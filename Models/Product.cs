using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace ClotherS.Models
{
    public class Product
    {
        [Key]
        public int ProductId { get; set; }
        public string ProductName { get; set; } = string.Empty;
        public int Quantity { get; set; }
        public int Price { get; set; }
        public int BrandId { get; set; }
        public string Image { get; set; } = string.Empty;
        public string Material { get; set; } = string.Empty;
        public int? CategoryId { get; set; }
        public string Size { get; set; } = string.Empty;
        public int? Discount { get; set; }
        public string Description { get; set; } = string.Empty;
        public bool Disable { get; set; } = false;
        public string Status { get; set; } = "Available";

        // Quan hệ
        public Brand Brand { get; set; }
        public Category Category { get; set; }
        public ICollection<OrderDetail> OrderDetails { get; set; } 
    }
}
