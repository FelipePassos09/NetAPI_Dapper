using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using StockCrud.Api.Data;
using StockCrud.Api.Entities;
using StockCrud.Api.Services;
using StockCrud.Api.Services.Customer;
using StockCrud.Api.Services.Enums;
using StockCrud.Api.Utils.EndpointUtils.SearchParameters;

namespace StockCrud.Api.Controllers;

[Controller]
[Route("/customers")]
public class CustomerController : Controller
{
    private readonly AppDbContext _dbContext;
    private readonly IMapper _mapper;
    private readonly ICustomerService _customerService;

    public CustomerController(AppDbContext context, IMapper mapper, ICustomerService customerService)
    {
        _dbContext = context;
        _mapper = mapper;
        _customerService = customerService;
    }

    [HttpGet]
    public async Task<ActionResult<CustomerGetDto>> GetCustomers()
    {
        if(!ModelState.IsValid) return BadRequest(ErrorMessages.GetMessage(ErrorCodes.BadRequest));
        try
        {
            var customers = await _dbContext.Customers.ToListAsync();

            if (!customers.Any()) return NotFound(ErrorMessages.GetMessage(ErrorCodes.NotFound));
            
            var customersDto = _mapper.Map<IEnumerable<CustomerGetDto>>(customers);

            return Ok(customersDto);
        }
        catch (Exception e)
        {
            Console.WriteLine(e);
            return Problem(ErrorMessages.GetMessage(ErrorCodes.InternalServerError));
        }
    }

    [HttpPost]
    public async Task<ActionResult<CustomerPostDto>> CreateCustomer([FromBody] CustomerPostDto customerIn)
    {
        if(!ModelState.IsValid) return BadRequest(ErrorMessages.GetMessage(ErrorCodes.BadRequest));
        
        var customer = _mapper.Map<Customer>(customerIn);
        
        try
        {
            if (customer != null)
            {
                customer.CreatedDate = DateTime.UtcNow;
                customer.UpdatedDate = DateTime.UtcNow;

                await _dbContext.Customers.AddAsync(customer);
                await _dbContext.SaveChangesAsync();

                return CreatedAtAction("GetCustomerById", new { id = customer.Id }, customer);
            }

            return Problem(ErrorMessages.GetMessage(ErrorCodes.BadRequestIfNull));
        }
        catch (Exception e)
        {
            Console.WriteLine(e);
            return Problem(ErrorMessages.GetMessage(ErrorCodes.InternalServerError));
        }
    }

    [HttpGet("{id:long}")]
    [ActionName("GetCustomerById")]
    public async Task<ActionResult<CustomerGetDto>> GetById([FromRoute] long id)
    {
        if(!ModelState.IsValid) return BadRequest(ErrorMessages.GetMessage(ErrorCodes.BadRequest));
        
        try
        {
            var customer = await _dbContext.Customers.FirstOrDefaultAsync(c => id == c.Id);

            if (customer == null) return NotFound(ErrorMessages.GetMessage(ErrorCodes.NotFound));
            
            var customerDto = _mapper.Map<CustomerGetDto>(customer);

            return Ok(customerDto);
        }
        catch (Exception e)
        {
            Console.WriteLine(e);
            return Problem(ErrorMessages.GetMessage(ErrorCodes.InternalServerError));
        }
    }

    [HttpPut("{id:long}")]
    public async Task<ActionResult<CustomerPostDto>> UpdateCustomer([FromRoute] long id, [FromBody] CustomerPostDto customerIn)
    {

        if(!ModelState.IsValid) return BadRequest(ErrorMessages.GetMessage(ErrorCodes.BadRequest));
        if (customerIn.Id != id) return BadRequest(ErrorMessages.GetMessage(ErrorCodes.BadRequest));
        
        var customer = _mapper.Map<Customer>(customerIn);
        
        try
        {
            customer.UpdatedDate = DateTime.UtcNow;

            var customerToUpdate = await _dbContext.Customers.FirstOrDefaultAsync(c => c.Id == id);

            if (customerToUpdate == null) return NotFound(ErrorMessages.GetMessage(ErrorCodes.NotFound));

            foreach (var atrib in customerToUpdate.GetType().GetProperties())
            {
                var value = atrib.GetValue(customer);

                if (value != null && atrib.Name != "CreatedDate" && atrib.Name != "Id")
                {
                    atrib.SetValue(customerToUpdate, value);
                }
            }

            _dbContext.Customers.Update(customerToUpdate);
            await _dbContext.SaveChangesAsync();

            return Ok(customerToUpdate);
        }
        catch (Exception e)
        {
            Console.WriteLine(e);
            return Problem(ErrorMessages.GetMessage(ErrorCodes.InternalServerError));
        }
    }

    [HttpDelete("{id:long}")]
    public async Task<IActionResult> DeleteCustomer([FromRoute] long id)
    {
        if(!ModelState.IsValid) return BadRequest(ErrorMessages.GetMessage(ErrorCodes.BadRequest));
        
        try
        {
            var customerToDelete = await _dbContext.Customers.FirstOrDefaultAsync(c => c.Id == id);
            
            if (customerToDelete == null) return NotFound(ErrorMessages.GetMessage(ErrorCodes.NotFound));
            
            _dbContext.Customers.Remove(customerToDelete);
            await _dbContext.SaveChangesAsync();
            
            var customerDto = _mapper.Map<CustomerGetDto>(customerToDelete);
            
            return Ok(customerDto);
        }
        catch (Exception e)
        {
            Console.WriteLine(e);
            return Problem(ErrorMessages.GetMessage(ErrorCodes.InternalServerError));
        }
    }

    [HttpPost("search")]
    [ActionName("GetCustomersFiltered")]
    public async Task<ActionResult<CustomerGetDto>> GetCustomersFiltered(
        [FromQuery] CustomerSearchParameters searchParameters)
    {
        if(!ModelState.IsValid) return BadRequest(ErrorMessages.GetMessage(ErrorCodes.BadRequest));
        
        var customers = await _customerService.SearchCustomer(searchParameters);
        
        var customersDto = _mapper.Map<IEnumerable<CustomerGetDto>>(customers);
        
        return Ok(customersDto);
    }
}