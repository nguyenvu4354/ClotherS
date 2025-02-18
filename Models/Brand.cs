using System.ComponentModel.DataAnnotations;

namespace ClotherS.Models
{
    public class Brand
    {
        [Key]
        public int BrandId { get; set; }
        public string BrandName { get; set; }

        // Quan hệ
        public ICollection<Product> Products { get; set; }
    }

}
