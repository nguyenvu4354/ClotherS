using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Collections.Generic;
using ClotherS.Models;
namespace ClotherS.Models  // ⚠️ THÊM DÒNG NÀY
{
    public class Order
    {
        [Key]
        public int OId { get; set; }

        [Required]
        public DateTime OrderDate { get; set; }

        public DateTime? ReceiveDate { get; set; }

        [Required]
        [ForeignKey("Account")]
        public int AccountId { get; set; }

        [Required]
        public string Status { get; set; } = "Pending";

        public bool IsCart { get; set; } = true;

        public virtual Account Account { get; set; }

        public virtual ICollection<OrderDetail>? OrderDetails { get; set; } = new List<OrderDetail>();
    }
}