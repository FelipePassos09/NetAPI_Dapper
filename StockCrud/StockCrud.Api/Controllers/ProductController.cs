using System.Data;
using Dapper;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using StockCrud.Api.Data;
using StockCrud.Api.Entities;

namespace StockCrud.Api.Controllers;

[ApiController]
[Route("/products")]
public class ProductController : Controller
{
    private readonly AppDbContext _dbContext;
    private readonly IDbConnection _dapperContext;

    public ProductController(AppDbContext context, DapperContext dapperContext)
    {
        _dbContext = context;
        _dapperContext = dapperContext.CreateConnection();
    }

    [HttpGet]
    public async Task<IActionResult> GetProducts()
    {
        if (!ModelState.IsValid)
        {
            return BadRequest();
        }

        try
        {
            var products = await _dbContext.Products.ToListAsync();

            if (products is null) return NotFound("Do not results to return.");
        
            return Ok(products);
        }
        catch (Exception e)
        {
            Console.WriteLine(e);
            return Problem("An error ocourred at the execute action.");
        }
        
    }

    [HttpPost]
    public async Task<IActionResult> CreateProduct([FromBody] Product product)
    {
        if (!ModelState.IsValid)
        {
            return BadRequest();
        }

        if (product is null) return BadRequest("Product cannot be null.");
        
        product.CreatedDate = DateTime.UtcNow;
        product.UpdatedDate = DateTime.UtcNow;

        try
        {
            var query = @"INSERT INTO products (name, description, price, categoryid, supplierid, createddate, updateddate) 
                  VALUES (@Name, @Description, @Price, @CategoryId, @SupplierId, @CreatedDate, @UpdatedDate)
                  RETURNING id;";
        
            var id = await _dapperContext.ExecuteScalarAsync<int>(query, new 
            {
                product.Name,
                product.Description,
                product.Price,
                product.CategoryId,
                product.SupplierId,
                product.CreatedDate,
                product.UpdatedDate
            });

            product.Id = id;

            return Created($"/{product.Id}", product);
        }
        catch (Exception e)
        {
            Console.WriteLine(e);
            return Problem();
        }
        
    }

    [HttpGet("{id}")]
    public async Task<ActionResult<Product>> GetProductById([FromRoute] long id)
    {
        string query = @"Select * from products where id = @id";
        var product = await _dapperContext.QueryFirstOrDefaultAsync(query, new {id});
        
        if (product is null)
        {
            return BadRequest("Id not exists.");
        }
        
        return Ok(product);
    }
}