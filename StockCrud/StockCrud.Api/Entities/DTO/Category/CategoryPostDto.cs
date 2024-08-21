namespace StockCrud.Api.Entities;

public class CategoryPostDto : General
{
    public string Name { get; set; } = null!;
    public string Description { get; set; } = null!;
}