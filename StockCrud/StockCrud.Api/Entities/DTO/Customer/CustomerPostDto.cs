using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;
using StockCrud.Api.Services;

namespace StockCrud.Api.Entities;

public class CustomerPostDto : General
{
    [DataType("varchar(25)")]
    public string Name { get; set; } = null!;
    [DataType("varchar(25)")]
    public string LastName { get; set; } = null!;
    [DataType("varchar(100)")]
    public string CompleteName { get; set; } = null!;
    [JsonConverter(typeof(CustomJsonConverters.DateOnlyConverter))]
    public DateOnly BirthDate { get; set; }
    public string? Email { get; set; }
    public string Cpf { get; set; } = null!;
    public long Telephone { get; set; }
    public string Address { get; set; } = string.Empty;
    public string City { get; set; } = string.Empty;
    [MaxLength(2)]
    [MinLength(2)]
    public string  Uf { get; set; } = string.Empty;
}