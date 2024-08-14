using System.Data;
using System.Data.Common;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using StockCrud.Api.Data;
using StockCrud.Api.Entities;

namespace StockCrud.Api.Controllers;

[ApiController]
[Route("/Supplier")]
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
        if (supplier != null)
        {
            supplier.CreatedDate = DateTime.UtcNow;
            supplier.UpdatedDate = DateTime.UtcNow;

            await _dbContext.Suppliers.AddAsync(supplier);
            await _dbContext.SaveChangesAsync();

            return Created($"/{supplier.Id}", supplier);
        }

        return BadRequest("Supplier não pode ser vazio");
    }

    [HttpGet]
    public async Task<IActionResult> GetSuppliers()
    {
        var suppliers = await _dbContext.Suppliers.Where(s => s.Id < 200).ToListAsync();

        if (suppliers is null) return BadRequest("Não há registros");

        return Ok(suppliers);
        
    }

    [HttpGet("/{id:int}")]
    public async Task<IActionResult> GetSupplierById([FromRoute] long id)
    {
        var supplier = await _dbContext.Suppliers.FirstOrDefaultAsync(s => s.Id == id);

        return Ok(supplier);
    }
}