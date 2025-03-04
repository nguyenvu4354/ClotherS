using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace ClotherS.Models
{
    public class Category
    {
        [Key]
        public int CategoryId { get; set; }

        [Required, StringLength(100)]
        public string CategoryName { get; set; }

        [StringLength(500)] // Giới hạn mô tả tối đa 500 ký tự
        public string Description { get; set; }

        public bool Disable { get; set; } = false; // Mặc định là "false"

        public ICollection<Product> Products { get; set; } = new List<Product>(); // Khởi tạo danh sách tránh null
    }
}
