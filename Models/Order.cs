using ClotherS.Models;
using System.ComponentModel.DataAnnotations;

public class Order
{
    [Key]
    public int OId { get; set; }
    public DateTime OrderDate { get; set; }
    public DateTime? ReceiveDate { get; set; }
    public int AccountId { get; set; }
    public string Status { get; set; } = "Pending"; 

    public bool IsCart { get; set; } = true; 

    public Account Account { get; set; }
    public ICollection<OrderDetail> OrderDetails { get; set; }
}
