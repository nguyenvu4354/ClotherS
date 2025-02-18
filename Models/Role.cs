using System.ComponentModel.DataAnnotations;

namespace ClotherS.Models
{
    public class Role
    {
        [Key]
        public int RoleId { get; set; }
        public string RoleName { get; set; } = string.Empty;

        public ICollection<Account>? Accounts { get; set; }  
    }
}
