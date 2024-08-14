using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using StockCrud.Api.Data;
using StockCrud.Api.Entities;

namespace StockCrud.Api.Controllers;

[Controller]
[Route("/customers")]
public class CustomerController : Controller
{
    
    private readonly AppDbContext _dbContext;

    public CustomerController(AppDbContext context)
    {
        _dbContext = context;
    }
    
    [HttpGet]
    public async Task<IActionResult> GetCustomers()
    {
        var customers = await _dbContext.Customers.ToListAsync(); 
        
        return Ok(customers);
    }

    [HttpPost]
    public async Task<IActionResult> CreateCustomer([FromBody] Customer customer)
    {
        if (customer != null)
        {
            customer.CreatedDate = DateTime.UtcNow;
            customer.UpdatedDate = DateTime.UtcNow;
            
            await _dbContext.Customers.AddAsync(customer);
            _dbContext.SaveChanges();

            return Created($"/{customer.Id}", customer);
        }

        return Problem();
    }
        

    [HttpGet("/id:int")]
    [ActionName("getById")]
    public async Task<IActionResult> GetById([FromRoute] long id)
    {
        var customer = await _dbContext.Customers.FirstOrDefaultAsync(c => id == c.Id);

        return Ok(customer);
    }
}