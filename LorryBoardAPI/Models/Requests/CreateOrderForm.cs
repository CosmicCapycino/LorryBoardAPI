namespace LorryBoardAPI.Models.Requests;

public class CreateOrderForm
{
    public string Customer { get; set; }
    public int Bay { get; set; }
    public DateTime ArrivalTime { get; set; }
    public DateTime? DepartureTime { get; set; }
    public bool SafeToLoad { get; set; }
    public bool HasKeys { get; set; }
    public string Status { get; set; }
}