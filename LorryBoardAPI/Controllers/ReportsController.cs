using System.Runtime.InteropServices.JavaScript;
using LorryBoardAPI.Models;
using LorryBoardAPI.Models.Reports;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace LorryBoardAPI.Controllers;

[ApiController]
[Route("[controller]/")]
public class ReportsController(LorryBoardDbContext lorryBoardDbContext) : ControllerBase
{
    private LorryBoardDbContext _dbContext = lorryBoardDbContext;

    [HttpGet("statusPie")]
    public async Task<IActionResult> GetStatusPieStatistics()
    {
        List<Order> ordersNotArrived = _dbContext.Orders.ToList().Where(o => o.Status == "Not Arrived" && o.ArrivalTime.Date == DateTime.UtcNow.Date).ToList();
        List<Order> ordersOnBay = _dbContext.Orders.ToList().Where(o => o.Status == "On Bay" && o.ArrivalTime.Date == DateTime.UtcNow.Date).ToList();
        List<Order> ordersCompleted= _dbContext.Orders.ToList().Where(o => o.Status == "Complete" && o.ArrivalTime.Date == DateTime.UtcNow.Date).ToList();

        List<int> stats = new List<int>()
        {
            ordersNotArrived.Count,
            ordersOnBay.Count,
            ordersCompleted.Count
        };
        
        return Ok(stats);
    }
    
    [HttpGet("onTimePie")]
    public async Task<IActionResult> GetOnTimePieStatistics()
    {
        List<Order> onTime = _dbContext.Orders.ToList().Where(o => o.OnTime == true).ToList();
        List<Order> late = _dbContext.Orders.ToList().Where(o => o.OnTime == false).ToList();

        List<int> stats = new List<int>()
        {
            onTime.Count,
            late.Count,
        };
        
        return Ok(stats);
    }

    [HttpGet("previousWeek")]
    public async Task<IActionResult> GetPreviousWeekStatistics()
    {
        DateTime currentDate = DateTime.UtcNow.Date;
        DateTime prevWeekDate = currentDate.AddDays(-6);
        
        List<DateTime> labels = Enumerable.Range(0, 7)
            .Select(offset => prevWeekDate.AddDays(offset))
            .ToList();
        
        List<Order> orders = await _dbContext.Orders
            .Where(o => o.ArrivalTime.Date >= prevWeekDate && o.ArrivalTime.Date <= currentDate.Date)
            .ToListAsync();

        var series = new
        {
            OnTime = labels.Select(date =>
                orders.Count(o => o.OnTime == true && o.ArrivalTime.Date == date)).ToList(),

            Late = labels.Select(date =>
                orders.Count(o => o.OnTime == false && o.ArrivalTime.Date == date)).ToList(),
        };

        return Ok(new
        {
            labels = labels.Select(d => d.ToString("yyyy-MM-dd")).ToList(),
            series
        });
    }

    [HttpGet("recent")]
    public async Task<IActionResult> GetRecentOrders()
    {
        List<Order> result = _dbContext.Orders
            .Include(e => e.Customer)
            .OrderBy(o => o.ArrivalTime)
            .Take(5)
            .ToList();
        return Ok(result);
    }
}