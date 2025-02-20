using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ClotherS.Models
{
    public class Product
    {
        [Key]
        public int ProductId { get; set; }

        [Required(ErrorMessage = "Product name is required.")]
        public string ProductName { get; set; } = string.Empty;

        [Range(0, int.MaxValue, ErrorMessage = "Quantity must be a positive number.")]
        public int Quantity { get; set; }

        [Range(0, int.MaxValue, ErrorMessage = "Price must be a positive number.")]
        public int Price { get; set; }

        [Required(ErrorMessage = "Brand is required.")]
        public int BrandId { get; set; }

        public string Image { get; set; } = string.Empty;

        public string Material { get; set; } = string.Empty;

        public int? CategoryId { get; set; }

        public string Size { get; set; } = string.Empty;

        [Range(0, 100, ErrorMessage = "Discount must be between 0 and 100.")]
        public int? Discount { get; set; }

        public string Description { get; set; } = string.Empty;

        public bool Disable { get; set; } = false;

        public string Status { get; set; } = "Available";

        // Khóa ngoại
        [ForeignKey("BrandId")]
        public virtual Brand? Brand { get; set; }

        [ForeignKey("CategoryId")]
        public virtual Category? Category { get; set; }

        public virtual ICollection<OrderDetail>? OrderDetails { get; set; } = new List<OrderDetail>();
    }
}
