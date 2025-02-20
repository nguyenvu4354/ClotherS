using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ClotherS.Models
{
    public class OrderDetail
    {
        [Key]
        public int DetailId { get; set; }

        [Required]
        [ForeignKey("Product")]
        public int ProductId { get; set; }

        [Required]
        [ForeignKey("Order")]
        public int OId { get; set; }

        [Required]
        [Range(0, int.MaxValue, ErrorMessage = "Price must be a positive number")]
        public int Price { get; set; }

        [Required]
        [Range(1, int.MaxValue, ErrorMessage = "Quantity must be at least 1")]
        public int Quantity { get; set; }

        [Range(0, 100, ErrorMessage = "Discount must be between 0% and 100%")]
        public int Discount { get; set; } = 0; // Mặc định không giảm giá

        // Quan hệ với Product
        public virtual Product Product { get; set; }

        // Quan hệ với Order
        public virtual Order Order { get; set; }
    }
}
