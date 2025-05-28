using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text.Json.Serialization;

namespace LorryBoardAPI.Models;

[Table("Customers")]
public class Customer
{
    [Key] public int Id { get; set; }
    [Required] public string Name { get; set; }
    [JsonIgnore] public ICollection<Order> Orders { get; set; } = null!;
}