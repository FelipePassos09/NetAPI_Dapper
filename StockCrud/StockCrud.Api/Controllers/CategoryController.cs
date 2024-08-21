using AutoMapper;
using Dapper;
using Microsoft.AspNetCore.Http.HttpResults;
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
    private readonly DapperContext _dapperContext;
    private readonly AppDbContext _dbContext;
    private readonly IMapper _mapper;

    public CategoryController(AppDbContext dbContext, DapperContext dapperContext, IMapper mapper)
    {
        _dbContext = dbContext;
        _dapperContext = dapperContext;
        _mapper = mapper;
    }

    [HttpGet]
    public async Task<IActionResult> GetCategories()
    {
        var categoryList = await _dbContext.Categories.ToListAsync();
        
        var categories = _mapper.Map<IEnumerable<Category>>(categoryList);

        return Ok(categories);
    }

    [HttpGet("{id:long}")]
    [ActionName("GetById")]
    public async Task<ActionResult<CategoryGetDto>> GetCategoryById([FromRoute] long id)
    {
        if (!ModelState.IsValid) return BadRequest(ErrorMessages.GetMessage(ErrorCodes.BadRequest));
        
        try
        {
            using (var db = _dapperContext.CreateConnection())
            {
                var query = "Select * from categories where id = @Id";

                var category = await db.QuerySingleOrDefaultAsync<Category>(query, new { Id = id });
                
                if (category == null) return BadRequest(ErrorMessages.GetMessage(ErrorCodes.NotFound));
                
                var categoryGetDto = _mapper.Map<CategoryGetDto>(category);
                
                return Ok(categoryGetDto);
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine(ex);
            return Problem(ErrorMessages.GetMessage(ErrorCodes.InternalServerError));
        }
    }
    
    [HttpPost]
    public ActionResult CreateCategory([FromBody] CategoryPostDto categoryIn)
    {
        if (!ModelState.IsValid) return BadRequest(ErrorMessages.GetMessage(ErrorCodes.BadRequest));
        
        var category = _mapper.Map<Category>(categoryIn);
        category.CreatedDate = DateTime.UtcNow;
        category.UpdatedDate = DateTime.UtcNow;

        using (var db = _dapperContext.CreateConnection())
        {
            try
            {
                var query = "INSERT INTO categories (Name, Description, CreatedDate, UpdatedDate)" +
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
                Console.WriteLine(e);
                return Problem(ErrorMessages.GetMessage(ErrorCodes.InternalServerError));
            }
        }
    }

    [HttpPut("{id:long}")]
    public ActionResult UpdateCategory([FromRoute] long id, [FromBody] CategoryPostDto categoryIn)
    {
        if (!ModelState.IsValid) return BadRequest(ErrorMessages.GetMessage(ErrorCodes.BadRequest));
        if (id != categoryIn.Id) return BadRequest(ErrorMessages.GetMessage(ErrorCodes.BadRequest));
        
        var categoryUpd = _mapper.Map<Category>(categoryIn);
        
        try
        {
            using (var db = _dapperContext.CreateConnection())
            {
                var category = db.QuerySingleOrDefault<Category>(@"select * from categories where id = @Id", new { Id = id });
                
                if (category == null) return NotFound(ErrorMessages.GetMessage(ErrorCodes.NotFound));
                
                category.UpdatedDate = DateTime.UtcNow;

                foreach (var atrib in categoryUpd.GetType().GetProperties())
                {
                    if (atrib.CanWrite && atrib.Name != "Id" && atrib.Name != nameof(categoryUpd.CreatedDate))
                    {
                        var value = atrib.GetValue(categoryUpd);

                        if (value != null) category.GetType().GetProperty(atrib.Name)?.SetValue(category, value);
                    }
                }

                var updateQuery = @"update categories SET 
                                    name  = @Name,
                                    description = @Description,
                                    updatedDate = @UpdatedDate
                                    where id = @Id;";

                db.Execute(updateQuery, new
                {
                    Id = category.Id,
                    Name = category.Name,
                    Description = category.Description,
                    UpdatedDate = category.UpdatedDate
                });

                return Ok(category);
            }
        }
        catch (Exception)
        {
            return Problem(ErrorMessages.GetMessage(ErrorCodes.InternalServerError));
        }
    }

    [HttpDelete("{id:long}")]
    public async Task<IActionResult> DeleteCategory([FromRoute] long id)
    {
        
        try
        {
            using (var db = _dapperContext.CreateConnection())
            {
                Category category = await db.QuerySingleAsync<Category>($"select * from categories where id = {id}");

                if (category == null) return BadRequest(ErrorMessages.GetMessage(ErrorCodes.NotFound));

                await db.ExecuteAsync($"delete from categories where id = {id}");
                
                var categoryDto = _mapper.Map<CategoryGetDto>(category);

                return Ok(categoryDto);
            }
        }
        catch (Exception)
        {
            return Problem(ErrorMessages.GetMessage(ErrorCodes.InternalServerError));
        }
    }
}