﻿using LorryBoardAPI.Models;
using LorryBoardAPI.Models.Requests;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace LorryBoardAPI.Controllers;

[ApiController]
[Route("[controller]/")]
public class OrderController(LorryBoardDbContext dbContext) : ControllerBase
{
    private LorryBoardDbContext _dbContext = dbContext;

    [HttpGet("fetch/all")]
    public async Task<IActionResult> GetAll()
    {
        return Ok(await _dbContext.Orders.ToListAsync());
    }

    [HttpGet("fetch/{id}")]
    public async Task<IActionResult> GetById(int id)
    {
        return Ok(await _dbContext.Orders.FindAsync(id));
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
            DepartureTime = order.ArrivalTime.AddHours(1),
            Bay = order.Bay,
            SafeToLoad = order.SafeToLoad,
            HasKeys = order.HasKeys,
            Status = order.Status,
        });
        
        await _dbContext.SaveChangesAsync();
        return Ok(order);
    }
}