﻿using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

namespace StockCrud.Api.Entities;

public class Supplier : General
{
    public string Fantasia { get; set; } = null!;
    public string RazaoSocial { get; set; } = null!;
    public string Cnpj { get; set; } = null!;
    public string Address { get; set; } = string.Empty;
    public string City { get; set; } = null!;
    [MaxLength(2)] [MinLength(2)] public string Uf { get; set; } = string.Empty;
    public long Telephone { get; set; }
    public string Email { get; set; } = string.Empty;

    [JsonIgnore]
    public ICollection<Product> Products { get; set; } = new List<Product>();

    public Supplier() : base()
    {
        
    }

}