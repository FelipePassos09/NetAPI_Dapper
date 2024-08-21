using System.Data;
using AutoMapper;
using Dapper;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using StockCrud.Api.Data;
using StockCrud.Api.Entities;
using StockCrud.Api.Services;
using StockCrud.Api.Services.Enums;

namespace StockCrud.Api.Controllers;

[ApiController]
[Route("/products")]
public class ProductController : Controller
{
    private readonly AppDbContext _dbContext;
    private readonly IDbConnection _dapperContext;
    private readonly IMapper _mapper;

    public ProductController(AppDbContext context, DapperContext dapperContext, IMapper mapper)
    {
        _dbContext = context;
        _dapperContext = dapperContext.CreateConnection();
        _mapper = mapper;
    }

    [HttpGet]
    public async Task<ActionResult<ProductGetDto>> GetProducts()
    {
        if (!ModelState.IsValid) return BadRequest(ErrorMessages.GetMessage(ErrorCodes.Unauthorized));
        
        try
        {
            var products = await _dbContext.Products.ToListAsync();

            if (!products.Any()) return NotFound("Do not results to return.");
            
            var productDto = _mapper.Map<IEnumerable<ProductGetDto>>(products);
            
            return Ok(productDto);
        }
        catch (Exception e)
        {
            Console.WriteLine(e);
            return Problem("An error ocourred at the execute action.");
        }
        
    }

    [HttpPost]
    public async Task<ActionResult<ProductPostDto>> CreateProduct([FromBody] ProductPostDto productIn)
    {
        if (!ModelState.IsValid) return BadRequest(ErrorMessages.GetMessage(ErrorCodes.BadRequest));
        
        var product = _mapper.Map<Product>(productIn);
        
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

            return CreatedAtAction("GetProductById", new{id = product.Id }, product);
        }
        catch (Exception e)
        {
            Console.WriteLine(e);
            return Problem();
        }
        
    }

    [HttpGet("{id}")]
    [ActionName("GetProductById")]
    public async Task<ActionResult<ProductGetDto>> GetProductById([FromRoute] long id)
    {
        if (!ModelState.IsValid) return BadRequest(ErrorMessages.GetMessage(ErrorCodes.BadRequest));
        
        string query = @"Select * from products where id = @id";
        var product = await _dapperContext.QueryFirstOrDefaultAsync(query, new {id});
        
        if (product is null) return BadRequest("Id not exists.");
        
        var productDto = _mapper.Map<ProductGetDto>(product);
        
        return Ok(productDto);
    }

    [HttpPut("{id:long}")]
    public async Task<ActionResult<ProductPostDto>> UpdateProduct([FromRoute] long id,
        [FromBody] ProductPostDto productIn)
    {
        if (!ModelState.IsValid) return BadRequest(ErrorMessages.GetMessage(ErrorCodes.BadRequest));
        if (id != productIn.Id) return BadRequest(ErrorMessages.GetMessage(ErrorCodes.BadRequest));
        
        var product = _mapper.Map<Product>(productIn);

        try
        {
            var productToUpdate = await _dbContext.Products.FirstOrDefaultAsync(x => x.Id == id);
            
            if (productToUpdate is null) return NotFound(ErrorMessages.GetMessage(ErrorCodes.NotFound));
            
            foreach (var atrib in product.GetType().GetProperties())
            {
                if (atrib.Name != "Id" && atrib.Name != "CreatedDate" && atrib.CanWrite)
                {
                    var value = atrib.GetValue(product);

                    if (value != null)
                    {
                        atrib.SetValue(productToUpdate, value);
                    }
                }
            }
            
            await _dbContext.SaveChangesAsync();
            
            return Ok(productToUpdate);
        }
        catch (Exception e)
        {
            Console.WriteLine(e);
            return Problem(ErrorMessages.GetMessage(ErrorCodes.InternalServerError));
        }
    }

    [HttpDelete("{id:long}")]
    public async Task<ActionResult<ProductGetDto>> DeleteProduct([FromRoute] long id)
    {
        if (!ModelState.IsValid) return BadRequest(ErrorMessages.GetMessage(ErrorCodes.BadRequest));
        try
        {
            var product = await _dbContext.Products.FirstOrDefaultAsync(x => x.Id == id);
            
            if (product is null) return NotFound(ErrorMessages.GetMessage(ErrorCodes.NotFound));
            
            _dbContext.Products.Remove(product);
            await _dbContext.SaveChangesAsync();
            
            var productDto = _mapper.Map<ProductGetDto>(product);
            
            return Ok(productDto);
        }
        catch (Exception e)
        {
            Console.WriteLine(e);
            return Problem(ErrorMessages.GetMessage(ErrorCodes.InternalServerError));
        }
    }
}