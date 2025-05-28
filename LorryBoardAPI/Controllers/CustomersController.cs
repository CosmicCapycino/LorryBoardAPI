using LorryBoardAPI.Models;
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
}