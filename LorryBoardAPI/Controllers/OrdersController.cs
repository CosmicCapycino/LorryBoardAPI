using LorryBoardAPI.Models;
using LorryBoardAPI.Models.Requests;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace LorryBoardAPI.Controllers;

[ApiController]
[Route("[controller]/")]
public class OrdersController(LorryBoardDbContext dbContext) : ControllerBase
{
    private LorryBoardDbContext _dbContext = dbContext;

    [HttpGet("fetch/all")]
    public async Task<IActionResult> GetAll()
    {
        return Ok(await _dbContext.Orders.Include(e => e.Customer).ToListAsync());
    }

    [HttpGet("fetch/{id}")]
    public async Task<IActionResult> GetById(int id)
    {
        return Ok(await _dbContext.Orders.FindAsync(id));
    }

    [HttpDelete("delete/{id}")]
    public async Task<IActionResult> DeleteOrder(int id)
    {
        var order = await _dbContext.Orders.FindAsync(id);
        if (order == null) return BadRequest("Order not found");

        _dbContext.Orders.Remove(order);
        await _dbContext.SaveChangesAsync();
        return NoContent();
    }

    [HttpPost("create")]
    public async Task<IActionResult> Create([FromForm] CreateOrderForm order)
    {
        Customer? customer = await _dbContext.Customers.Where(e => e.Name == order.Customer).FirstOrDefaultAsync();
        if(customer == null) return BadRequest("Customer not found");

        Order newOrder = new Order()
        {
            Customer = customer,
            ArrivalTime = order.ArrivalTime,
            DepartureTime = order.DepartureTime,
            TargetDepartureTime = order.ArrivalTime.AddHours(1),
            Bay = order.Bay,
            SafeToLoad = order.SafeToLoad,
            HasKeys = order.HasKeys,
            Status = order.Status,
        };
        
        if (newOrder.Status == "Complete")
        {
            if (order.DepartureTime <= newOrder.TargetDepartureTime)
            {
                newOrder.OnTime = true;
            }
            else
            {
                newOrder.OnTime = false;
            }
            newOrder.DepartureTime = order.DepartureTime;
        }
        
        await _dbContext.Orders.AddAsync(newOrder);
        
        await _dbContext.SaveChangesAsync();
        return Ok(order);
    }

    [HttpPut("update/{id}")]
    public async Task<IActionResult> UpdateOrder(int id, [FromForm] CreateOrderForm order)
    {
        Customer? customer = await _dbContext.Customers.Where(e => e.Name == order.Customer).FirstOrDefaultAsync();
        if(customer == null) return BadRequest("Customer not found");
        
        var existingOrder = await _dbContext.Orders.FindAsync(id);
        if (existingOrder == null) return BadRequest("Order not found");

        existingOrder.Customer = customer;
        existingOrder.ArrivalTime = order.ArrivalTime;
        existingOrder.TargetDepartureTime = order.ArrivalTime.AddHours(1);
        existingOrder.Bay = order.Bay;
        existingOrder.SafeToLoad = order.SafeToLoad;
        existingOrder.HasKeys = order.HasKeys;
        existingOrder.Status = order.Status;

        if (existingOrder.Status == "Complete")
        {
            if (order.DepartureTime <= existingOrder.TargetDepartureTime)
            {
                existingOrder.OnTime = true;
            }
            else
            {
                existingOrder.OnTime = false;
            }
            existingOrder.DepartureTime = order.DepartureTime;
        }
        
        await _dbContext.SaveChangesAsync();
        return NoContent();
    }
}