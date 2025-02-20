using System.ComponentModel.DataAnnotations;

namespace ClotherS.Models
{
    public class Role
    {
        [Key]
        public int RoleId { get; set; }
        public string RoleName { get; set; } = string.Empty;
        public bool Disable { get; set; } = false;

        public ICollection<Account>? Accounts { get; set; }  
    }
}
