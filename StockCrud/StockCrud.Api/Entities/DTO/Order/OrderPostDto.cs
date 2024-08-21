namespace StockCrud.Api.Entities;

public class OrderPostDto : General
{
    public long CustomerId { get; set; }
    public long ProductId { get; set; }
    public int Units { get; set; }
    public string Annotation { get; set; } = string.Empty;
}