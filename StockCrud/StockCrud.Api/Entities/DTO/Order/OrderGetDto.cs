namespace StockCrud.Api.Entities;

public class OrderGetDto : General
{
    public long CustomerId { get; set; }
    public long ProductId { get; set; }
    public int Units { get; set; }
    public string Annotation { get; set; } = string.Empty;
    public ProductGetDto? Product { get; set; }
    public CustomerGetDto? Customer { get; set; }
    
}