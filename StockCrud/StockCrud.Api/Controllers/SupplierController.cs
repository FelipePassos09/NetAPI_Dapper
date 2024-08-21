using System.Data;
using System.Data.Common;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using StockCrud.Api.Data;
using StockCrud.Api.Entities;
using StockCrud.Api.Services;
using StockCrud.Api.Services.Enums;

namespace StockCrud.Api.Controllers;

[ApiController]
[Route("/suppliers")]
public class SupplierController : Controller
{
    private readonly AppDbContext _dbContext;
    private readonly IDbConnection _dapperContext;

    public SupplierController(AppDbContext dbContext, DapperContext dapperContext)
    {
        _dbContext = dbContext;
        _dapperContext = dapperContext.CreateConnection();
    }

    [HttpPost]
    public async Task<IActionResult> CreateSupplier([FromBody] Supplier supplier)
    {
        
        if (!ModelState.IsValid) return BadRequest(ErrorMessages.GetMessage(ErrorCodes.BadRequest));
        if (supplier is null) return BadRequest(ErrorMessages.GetMessage(ErrorCodes.BadRequestIfNull));

        try
        {
            supplier.CreatedDate = DateTime.UtcNow;
            supplier.UpdatedDate = DateTime.UtcNow;

            await _dbContext.Suppliers.AddAsync(supplier);
            await _dbContext.SaveChangesAsync();

            return Created($"/{supplier.Id}", supplier);
        }
        catch (Exception e)
        {
            Console.WriteLine(e);
            return Problem(ErrorMessages.GetMessage(ErrorCodes.InternalServerError));
        }
    }

    [HttpGet]
    public async Task<IActionResult> GetSuppliers()
    {
        if (!ModelState.IsValid) return BadRequest(ErrorMessages.GetMessage(ErrorCodes.BadRequest));

        try
        {
            var suppliers = await _dbContext.Suppliers.Where(s => s.Id < 200).ToListAsync();

            if (suppliers is null) return NotFound(ErrorMessages.GetMessage(ErrorCodes.NotFound));

            return Ok(suppliers);
        }
        catch (Exception e)
        {
            Console.WriteLine(e);
            return Problem(ErrorMessages.GetMessage(ErrorCodes.InternalServerError));
        }
        
    }

    [HttpGet("{id:long}")]
    public async Task<IActionResult> GetSupplierById([FromRoute] long id)
    {
        if (!ModelState.IsValid) return BadRequest(ErrorMessages.GetMessage(ErrorCodes.BadRequest));

        try
        {
            var supplier = await _dbContext.Suppliers.FirstOrDefaultAsync(s => s.Id == id);
            
            if (supplier == null) return NotFound(ErrorMessages.GetMessage(ErrorCodes.NotFound));
            
            return Ok(supplier);
        }
        catch (Exception e)
        {
            Console.WriteLine(e);
            return Problem(ErrorMessages.GetMessage(ErrorCodes.InternalServerError));
        }
    }

    [HttpPut("{id:long}")]
    public async Task<IActionResult> UpdateSupplier([FromRoute] long id, [FromBody] Supplier supplier)
    {
        if (!ModelState.IsValid) return BadRequest(ErrorMessages.GetMessage(ErrorCodes.BadRequest));
        if (supplier is null) return BadRequest(ErrorMessages.GetMessage(ErrorCodes.BadRequestIfNull));
        if (supplier.Id != id) return BadRequest(ErrorMessages.GetMessage(ErrorCodes.BadRequest));
        
        supplier.UpdatedDate = DateTime.UtcNow;
        
        try
        {
            var supplierToUpdate = await _dbContext.Suppliers.FirstOrDefaultAsync(s => s.Id == id);
            
            if (supplierToUpdate is null) return NotFound(ErrorMessages.GetMessage(ErrorCodes.NotFound));

            foreach (var atrib in supplier.GetType().GetProperties())
            {
                if (atrib.CanWrite && atrib.Name != "Id" && atrib.Name != "CreatedDate")
                {
                    var newValue = atrib.GetValue(supplier);
                    if (newValue != null) atrib.SetValue(supplierToUpdate, newValue);
                }
            }
            
            _dbContext.Update(supplierToUpdate);
            await _dbContext.SaveChangesAsync();
            
            return Ok(supplierToUpdate);
        }
        catch (Exception e)
        {
            Console.WriteLine(e);
            return Problem(ErrorMessages.GetMessage(ErrorCodes.InternalServerError));
        }
    }

    [HttpDelete("{id:long}")]
    public async Task<IActionResult> DeleteSupplier([FromRoute] long id)
    {
        if (!ModelState.IsValid) return BadRequest(ErrorMessages.GetMessage(ErrorCodes.BadRequest));
        
        try
        {
            var supplier = await _dbContext.Suppliers.FirstOrDefaultAsync(s => s.Id == id);
            if (supplier == null) return NotFound(ErrorMessages.GetMessage(ErrorCodes.NotFound));
            
            _dbContext.Suppliers.Remove(supplier);
            await _dbContext.SaveChangesAsync();
            
            return Ok(supplier);
        }
        catch (Exception e)
        {
            Console.WriteLine(e);
            return Problem(ErrorMessages.GetMessage(ErrorCodes.InternalServerError));
        }
    }
    
}