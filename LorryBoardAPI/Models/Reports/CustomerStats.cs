namespace LorryBoardAPI.Models.Reports;

public class CustomerStats
{
    public Customer Customer { get; set; }
    public int TotalOrders { get; set; }
    public int TotalOnTime { get; set; }
    public int TotalLate { get; set; }
}