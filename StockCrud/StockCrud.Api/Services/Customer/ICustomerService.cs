using Microsoft.AspNetCore.Mvc;
using StockCrud.Api.Utils.EndpointUtils.SearchParameters;


namespace StockCrud.Api.Services.Customer;

public interface ICustomerService
{
    Task<IEnumerable<Entities.Customer>> SearchCustomer(CustomerSearchParameters searchParameters);
}