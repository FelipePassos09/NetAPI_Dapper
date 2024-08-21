using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;
using StockCrud.Api.Data;
using StockCrud.Api.Entities;
using StockCrud.Api.Services;
using StockCrud.Api.Services.Enums;

namespace StockCrud.Api.Controllers;

[Route("/orders")]
public class OrderController : Controller
{
    private readonly DapperContext _dapperContext;
    private readonly AppDbContext _dbContext;
    private readonly IMapper _mapper;

    public OrderController(AppDbContext dbContext, DapperContext dapperContext, IMapper mapper)
    {
        _dbContext = dbContext;
        _dapperContext = dapperContext;
        _mapper = mapper;
    }

    [HttpGet]
    public async Task<ActionResult<IEnumerable<OrderGetDto>>> GetOrders()
    {
        if (!ModelState.IsValid) return BadRequest(ErrorMessages.GetMessage(ErrorCodes.BadRequest));
        try
        {
            var orders = await _dbContext.Orders
                .Include(o => o.Customer)
                .Include(o => o.Product)
                .Where(c => c.Id < 250).ToListAsync();

            if (!orders.Any()) return NotFound(ErrorMessages.GetMessage(ErrorCodes.NotFound));

            var orderDto = _mapper.Map<IEnumerable<OrderGetDto>>(orders);

            return Ok(orderDto);
        }
        catch (Exception e)
        {
            Console.WriteLine(e);
            return Problem(ErrorMessages.GetMessage(ErrorCodes.InternalServerError));
        }
    }

    [HttpGet("{id:long}")]
    [ActionName("GetOrderById")]
    public async Task<IActionResult> GetOrderById([FromRoute] long id)
    {
        if (!ModelState.IsValid) return BadRequest(ErrorMessages.GetMessage(ErrorCodes.BadRequest));

        try
        {
            var order = await _dbContext.Orders
                .Include(o => o.Customer)
                .Include(o => o.Product)
                .FirstOrDefaultAsync(c => c.Id == id);

            if (order is null) return NotFound(ErrorMessages.GetMessage(ErrorCodes.NotFound));
            
            var orderDto = _mapper.Map<OrderGetDto>(order);
            
            return Ok(orderDto);
        }
        catch (Exception e)
        {
            Console.WriteLine(e);
            return Problem(ErrorMessages.GetMessage(ErrorCodes.InternalServerError));
        }
    }

    [HttpPost]
    public async Task<ActionResult<OrderPostDto>> CreateOrder([FromBody] OrderPostDto order)
    {
        if (!ModelState.IsValid) return BadRequest(ErrorMessages.GetMessage(ErrorCodes.BadRequest));
        if (order is null) return BadRequest(ErrorMessages.GetMessage(ErrorCodes.BadRequestIfNull));

        order.CreatedDate = DateTime.UtcNow;
        order.UpdatedDate = DateTime.UtcNow;

        try
        {
            var orderDto = _mapper.Map<Order>(order);
            await _dbContext.Orders.AddAsync(orderDto);
            _dbContext.SaveChanges();

            return CreatedAtAction("GetOrderById", new { id = order.Id }, order);
        }
        catch (Exception e)
        {
            Console.WriteLine(e);
            return Problem(ErrorMessages.GetMessage(ErrorCodes.InternalServerError));
        }
    }

    [HttpPut("{id:long}")]
    public async Task<IActionResult> UpdateOrder([FromRoute] long id, [FromBody] Order order)
    {
        if (!ModelState.IsValid) return BadRequest(ErrorMessages.GetMessage(ErrorCodes.BadRequest));
        if (order.Id != id) return BadRequest(ErrorMessages.GetMessage(ErrorCodes.BadRequest));

        try
        {
            var orderToUpdate = await _dbContext.Orders.FirstOrDefaultAsync(c => c.Id == id);

            foreach (var atrib in order.GetType().GetProperties())
            {
                if (atrib.Name != "Id" && atrib.CanWrite && atrib.Name != "CreatedDate")
                {
                    var value = atrib.GetValue(order);

                    if (value != null)
                    {
                        atrib.SetValue(orderToUpdate, atrib.GetValue(order));
                    }
                }
            }

            _dbContext.Orders.Update(orderToUpdate);
            await _dbContext.SaveChangesAsync();

            return Ok(orderToUpdate);
        }
        catch (Exception e)
        {
            Console.WriteLine(e);
            return Problem(ErrorMessages.GetMessage(ErrorCodes.InternalServerError));
        }
    }

    [HttpDelete("{id:long}")]
    public async Task<ActionResult<OrderGetDto>> DeleteOrder([FromRoute] long id)
    {
        if (!ModelState.IsValid) return BadRequest(ErrorMessages.GetMessage(ErrorCodes.BadRequest));
        try
        {
            var order = await _dbContext.Orders.FirstOrDefaultAsync(c => c.Id == id);

            if (order is null) return NotFound(ErrorMessages.GetMessage(ErrorCodes.NotFound));
            
            _dbContext.Orders.Remove(order);
            await _dbContext.SaveChangesAsync();
            
            var orderDto = _mapper.Map<OrderGetDto>(order);
            
            return Ok(orderDto);
        }
        catch (Exception e)
        {
            Console.WriteLine(e);
            return Problem(ErrorMessages.GetMessage(ErrorCodes.InternalServerError));
        }
    }
}