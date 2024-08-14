using System.Text.Json.Serialization;

namespace StockCrud.Api.Entities;

public class General
{
    public long Id { get; set; }
    [JsonIgnore] public DateTime CreatedDate { get; set;  } = DateTime.UtcNow;
    [JsonIgnore] public DateTime UpdatedDate { get; set; } = DateTime.UtcNow;

}