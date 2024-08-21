namespace StockCrud.Api.Entities;

public class ProductGetDto : General
{
    public string? Name { get; set; }
    public string? Description { get; set; }
    public decimal Price { get; set; }
    public decimal Cost { get; set; }
    public int Units { get; set; }
    
    public long CategoryId { get; set; }
    public long SupplierId { get; set; }
}