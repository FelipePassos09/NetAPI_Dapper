using System.ComponentModel.DataAnnotations;
using System.Runtime.InteropServices.JavaScript;

namespace StockCrud.Api.Utils.EndpointUtils.SearchParameters;

public class CustomerSearchParameters
{
    public DateTime? CreatedIni { get; set; }
    public DateTime? CreatedEnd { get; set; }
    public DateOnly? BirthDate { get; set; }
    public String? Email { get; set; }
    public String? NameContains { get; set; }
    public String? Cpf { get; set; }
    public String? City { get; set; }
    public string? Uf { get; set; }
}