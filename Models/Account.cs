using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ClotherS.Models
{
    public class Account
    {
        [Key]
        public int AccountId { get; set; }

        [Required(ErrorMessage = "Email is required.")]
        public string Email { get; set; } = string.Empty;

        [Required(ErrorMessage = "First Name is required.")]
        public string FirstName { get; set; } = string.Empty;

        [Required(ErrorMessage = "Last Name is required.")]
        public string LastName { get; set; } = string.Empty;

        [Required(ErrorMessage = "Phone number is required.")]
        public string Phone { get; set; } = string.Empty;

        [Required(ErrorMessage = "Password is required.")]
        public string Password { get; set; } = string.Empty;
        public string AccountImage { get; set; } = string.Empty;
        public string Address { get; set; } = string.Empty;
        public string Gender { get; set; } = string.Empty;
        public bool? Active { get; set; }
        public string Description { get; set; } = string.Empty;

        [Required(ErrorMessage = "The Role field is required.")]
        public int? RoleId { get; set; }

        public string DateOfBirth { get; set; } = string.Empty;
        public bool Disable { get; set; } = false;

        [ForeignKey("RoleId")]
        public virtual Role? Role { get; set; }
    }
}
