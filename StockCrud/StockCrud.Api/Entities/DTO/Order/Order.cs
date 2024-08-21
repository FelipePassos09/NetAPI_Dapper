using System.ComponentModel.DataAnnotations.Schema;
using System.Diagnostics.CodeAnalysis;
using Newtonsoft.Json;

namespace StockCrud.Api.Entities;

public class Order : General
{
    public long CustomerId { get; set; }
    public long ProductId { get; set; }
    public int Units { get; set; }
    public string Annotation { get; set; } = string.Empty;
    [JsonIgnore]
    public Product? Product { get; set; }
    [JsonIgnore]
    public Customer? Customer { get; set; }
    
    public Order() : base()
    {   
        
    }
}