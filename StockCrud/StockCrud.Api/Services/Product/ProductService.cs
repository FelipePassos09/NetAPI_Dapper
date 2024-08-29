using StockCrud.Api.Data;

namespace StockCrud.Api.Services.Product;

public class ProductService : IProductService
{
    private readonly AppDbContext _context;

    public ProductService(AppDbContext context)
    {
        _context = context;
    }

    public async Task RemoveProductQuantityAsync(long productId, int quantity)
    {
        var product = await _context.Products.FindAsync(productId);

        if (product == null)
        {
            throw new ArgumentNullException("Product not found");
        }

        if (product.Units < quantity)
        {
            throw new ArgumentOutOfRangeException("Quantity out of range");
        }
        
        product.Units -= quantity;
        
        _context.Products.Update(product);
        await _context.SaveChangesAsync();
    }
}