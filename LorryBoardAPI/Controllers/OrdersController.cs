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
        Customer? customer = await _dbContext.Customers.Where(e => e.Id == order.Customer).FirstOrDefaultAsync();
        if(customer == null) return BadRequest("Customer not found");
        
        await _dbContext.Orders.AddAsync(new Order()
        {
            Customer = customer,
            ArrivalTime = order.ArrivalTime,
            DepartureTime = order.DepartureTime,
            Bay = order.Bay,
            SafeToLoad = order.SafeToLoad,
            HasKeys = order.HasKeys,
            Status = order.Status,
        });
        
        await _dbContext.SaveChangesAsync();
        return Ok(order);
    }
}