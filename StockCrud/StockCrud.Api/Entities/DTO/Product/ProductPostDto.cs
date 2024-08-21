namespace StockCrud.Api.Entities;

public class ProductPostDto : General
{
    public string Name { get; set; } = null!;
    public string Description { get; set; } = null!;
    public decimal Price { get; set; }
    public decimal Cost { get; set; }
    public int Units { get; set; }
    
    public long CategoryId { get; set; }
    public long SupplierId { get; set; }
}