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

        public bool IsCart { get; set; } = true;

        public virtual Account Account { get; set; }

        public virtual ICollection<OrderDetail>? OrderDetails { get; set; } = new List<OrderDetail>();
    }
}
