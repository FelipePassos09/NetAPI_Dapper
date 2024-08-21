using System.Diagnostics.CodeAnalysis;
using System.Text.Json.Serialization;

namespace StockCrud.Api.Entities;

public class Product : General
{
    public string Name { get; set; } = null!;
    public string Description { get; set; } = null!;
    public decimal Price { get; set; }
    public decimal Cost { get; set; }
    public int Units { get; set; }
    
    public long CategoryId { get; set; }
    public long SupplierId { get; set; }
    
    [JsonIgnore]
    public List<Order>? Orders { get; set;  }
    

    public Product() : base() { }
}