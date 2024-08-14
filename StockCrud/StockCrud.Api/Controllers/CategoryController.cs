
using System.Data;
using Dapper;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using StockCrud.Api.Data;
using StockCrud.Api.Entities;
using StockCrud.Api.Services;
using StockCrud.Api.Services.Enums;

namespace StockCrud.Api.Controllers;

[ApiController]
[Route("/category")]
public class CategoryController : Controller
{
    private readonly AppDbContext _dbContext;
    private readonly DapperContext _dapperContext;

    public CategoryController(AppDbContext dbContext, DapperContext dapperContext)
    {
        _dbContext = dbContext;
        _dapperContext = dapperContext;
    }

    [HttpGet]
    public async Task<IActionResult> GetCategories()
    {
        var categoryList = await _dbContext.Categories.ToListAsync();

        return Ok(categoryList);
    }

    [HttpPost]
    public ActionResult CreateCategory([FromBody] Category category)
    {
        ModelState.Remove("products");
        
        if (!ModelState.IsValid)
        {
            return BadRequest(ErrorMessages.GetMessage(ErrorCodes.BadRequest));
        }
        
        category.CreatedDate = DateTime.UtcNow;
        category.UpdatedDate = DateTime.UtcNow;

        using (IDbConnection db = _dapperContext.CreateConnection())
        {
            try
            {
                string query = "INSERT INTO categories (Name, Description, CreatedDate, UpdatedDate)" +
                               " VALUES (@Name, @Description, @CreatedDate, @UpdatedDate)" +
                               " RETURNING Id";
                var id = db.QuerySingle<long>(query, new
                {
                    category.Name,
                    category.Description,
                    category.CreatedDate,
                    category.UpdatedDate
                });
                category.Id = id;
                
                return CreatedAtAction("GetById", new { id = category.Id }, category);
            }
            catch (Exception e)
            {
                return Problem(ErrorMessages.GetMessage(ErrorCodes.InternalServerError));
            }
        }
    }

    [HttpGet("{id:long}")]
    [ActionName("GetById")]
    public IActionResult GetCategoryById([FromRoute] long id)
    {
        using (IDbConnection db = _dapperContext.CreateConnection())
        {
            var query = "Select * from categories where id = @id";
            
            var category = db.QuerySingleOrDefault<Category>(query, new { Id = id });

            return Ok(category);
        }
        
    }

    [HttpPut("/{id:int}")]
    public IActionResult UpdateCategory([FromRoute] long id, [FromBody] Category categoryUpd)
    {
        if (id == null) return BadRequest(ErrorMessages.GetMessage(ErrorCodes.BadRequestIfNull));
        if (!ModelState.IsValid) return BadRequest(ErrorMessages.GetMessage(ErrorCodes.BadRequest));

        try
        {
            using (IDbConnection db = _dapperContext.CreateConnection())
            {
                var category = db.QuerySingle($"select * from categories where id = {id}");

                foreach (var atrib in categoryUpd.GetType().GetProperties())
                {
                    category.atrib = atrib.GetValue(categoryUpd);
                }
                
            }
        }
        catch (Exception e)
        {
            Console.WriteLine(e);
            throw;
        }
    }
}