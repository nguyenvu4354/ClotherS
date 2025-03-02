using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
namespace ClotherS.Models
{
    public class Order
    {
        [Key]
        public int OId { get; set; }

        [Required]
        [ForeignKey("Account")]
        public int AccountId { get; set; }
        public DateTime OrderDate { get; set; } = DateTime.UtcNow;
        public bool IsCart { get; set; } = true;
        public virtual Account Account { get; set; }

        [Required(ErrorMessage = "Shipping Address is required.")]
        public string ShippingAddress { get; set; } = string.Empty;

        [Phone(ErrorMessage = "Invalid phone number format.")]
        public string PhoneNumber { get; set; } = string.Empty;
        public string? CustomerNote { get; set; }
        public virtual ICollection<OrderDetail>? OrderDetails { get; set; } = new List<OrderDetail>();
    }
}
