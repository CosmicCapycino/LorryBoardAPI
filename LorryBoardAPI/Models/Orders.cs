using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text.Json.Serialization;

namespace LorryBoardAPI.Models;

[Table("Orders")]
public class Order
{
    [Key] public int Id { get; set; }
    [JsonIgnore] public int CustomerId { get; set; } // Foreign key property
    public Customer Customer { get; set; } = null!; // Navigation reference to Customer principal
    public DateTime ArrivalTime { get; set; }
    public DateTime DepartureTime { get; set; }
    public int Bay;
    public bool SafeToLoad { get; set; }
    public bool HasKeys { get; set; }
    [Required] public string Status { get; set; }
}