using LorryBoardAPI.Models;
using LorryBoardAPI.Models.Reports;
using LorryBoardAPI.Models.Requests;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace LorryBoardAPI.Controllers;

[ApiController]
[Route("[controller]/")]
public class CustomersController(LorryBoardDbContext lorryBoardDbContext) : ControllerBase
{
    private LorryBoardDbContext _dbContext = lorryBoardDbContext;

    [HttpGet("fetch/all")]
    public async Task<IActionResult> FetchAllCustomers()
    {
        List<Customer> allCustomers = await _dbContext.Customers.ToListAsync();
        return Ok(allCustomers);
    }

    [HttpGet("fetch/stats")]
    public async Task<IActionResult> FetchStats()
    {
        List<CustomerStats> customerStats = new List<CustomerStats>();
        List<Customer> allCustomers = await _dbContext.Customers.ToListAsync();
        foreach (Customer customer in allCustomers)
        {
            customerStats.Add(new CustomerStats()
            {
                Customer = customer,
                TotalOrders = await _dbContext.Orders.Include(o => o.Customer).Where(o => o.Customer == customer).CountAsync(),
                TotalOnTime = await _dbContext.Orders.Include(o => o.Customer).Where(o => o.Customer == customer && o.OnTime == true).CountAsync(),
                TotalLate = await _dbContext.Orders.Include(o => o.Customer).Where(o => o.Customer == customer && o.OnTime == false).CountAsync()
            });
        }
        return Ok(customerStats);
    }

    [HttpPost("create")]
    public async Task<IActionResult> CreateCustomer([FromForm] CreateCustomerForm createCustomerForm)
    {
        Customer? existingCustomer = await _dbContext.Customers.FirstOrDefaultAsync(c => c.Name == createCustomerForm.Name);
        if(existingCustomer != null) return BadRequest("Customer already exists");
        
        await _dbContext.Customers.AddAsync(new Customer()
        {
            Name = createCustomerForm.Name,
        });
        
        await _dbContext.SaveChangesAsync();

        return Ok();
    }

    [HttpPut("update/{id}")]
    public async Task<IActionResult> UpdateCustomer(int id, [FromForm] string name)
    {
        Customer? customer = await _dbContext.Customers.FirstOrDefaultAsync(c => c.Id == id);
        if(customer == null) return BadRequest("Customer not found");
        
        customer.Name = name;
        await _dbContext.SaveChangesAsync();
        return Ok();
    }

    [HttpDelete("delete/{id}")]
    public async Task<IActionResult> DeleteCustomer(int id)
    {
        Customer? customer = await _dbContext.Customers.FirstOrDefaultAsync(c => c.Id == id);
        if(customer == null) return BadRequest("Customer not found");
        
        _dbContext.Customers.Remove(customer);
        await _dbContext.SaveChangesAsync();
        return Ok();
    }
}