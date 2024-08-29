namespace StockCrud.Api.Services.Product;

public interface IProductService
{
    Task RemoveProductQuantityAsync(long productId, int quantity);
}