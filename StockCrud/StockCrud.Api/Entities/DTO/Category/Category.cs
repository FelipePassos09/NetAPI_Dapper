using System.Diagnostics.CodeAnalysis;
using Newtonsoft.Json;


namespace StockCrud.Api.Entities;

public class Category : General
{
    public string Name { get; set; } = null!;
    public string Description { get; set; } = null!;

    [JsonIgnore]
    public ICollection<Product>? Products { get; set;  } = new List<Product>();
    
    public Category() : base() { }
}