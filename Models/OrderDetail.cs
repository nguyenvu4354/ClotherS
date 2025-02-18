using System.ComponentModel.DataAnnotations;

namespace ClotherS.Models
{
    public class OrderDetail
    {
        [Key]
        public int DetailId { get; set; }
        public int ProductId { get; set; }
        public int OId { get; set; }
        public int Price { get; set; }
        public int Quantity { get; set; }
        public int Discount { get; set; }

        // Quan hệ
        public Product Product { get; set; }
        public Order Order { get; set; }
    }

}
