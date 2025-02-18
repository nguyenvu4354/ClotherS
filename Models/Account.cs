using System.ComponentModel.DataAnnotations;

namespace ClotherS.Models
{
    public class Account
    {
        [Key]
        public int AccountId { get; set; }
        public string Email { get; set; } = string.Empty; // Giá trị mặc định
        public string FirstName { get; set; } = string.Empty; // Giá trị mặc định
        public string LastName { get; set; } = string.Empty; // Giá trị mặc định
        public string Phone { get; set; } = string.Empty; // Giá trị mặc định
        public string Password { get; set; } = string.Empty; // Giá trị mặc định
        public string AccountImage { get; set; } = string.Empty; 
        public string Address { get; set; } = string.Empty; 
        public string Gender { get; set; } = string.Empty; 
        public bool? Active { get; set; }
        public string Description { get; set; } = string.Empty; 
        public int? RoleId { get; set; }
        public string DateOfBirth { get; set; } = string.Empty; 
        public bool Disable { get; set; } = false;

        public Role Role { get; set; }
        public ICollection<Order> Orders { get; set; }
    }
}
