using System.Data;
using System.Linq.Expressions;
using System.Text;
using Dapper;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using StockCrud.Api.Data;
using StockCrud.Api.Utils.EndpointUtils.SearchParameters;
using StockCrud.Api.Entities;
using StockCrud.Api.Services.Enums;

namespace StockCrud.Api.Services.Customer;

public class CustomerService : ICustomerService
{
    private AppDbContext _context;

    public CustomerService(AppDbContext context)
    {
        _context = context;
    }

    public async Task<IEnumerable<Entities.Customer>> SearchCustomer(CustomerSearchParameters searchParameters)
    {
        var query = _context.Customers.AsQueryable();

        foreach (var prop in searchParameters.GetType().GetProperties())
        {
            var value = prop.GetValue(searchParameters);
            if (value == null) continue;
            
            if (prop.Name == nameof(CustomerSearchParameters.BirthDate) && value is DateOnly date)
            {
                
                query = query.Where(c => c.BirthDate == date);
            }
            
            if (prop.PropertyType == typeof(DateTime) && value is DateTime dateValue)
            {
                if (prop.Name == nameof(CustomerSearchParameters.CreatedIni))
                    query = query.Where(c => c.CreatedDate >= dateValue);

                if (prop.Name == nameof(CustomerSearchParameters.CreatedEnd))
                    query = query.Where(c => c.CreatedDate <= dateValue);
                
            }
            else if (prop.PropertyType == typeof(string) && value is string strValue && !string.IsNullOrEmpty(strValue))
            {
                if (prop.Name == nameof(CustomerSearchParameters.NameContains))
                    query = query.Where(c => c.Name.Contains(strValue));
                else
                {
                    var parameter = Expression.Parameter(typeof(Entities.Customer), "c");
                    var property = Expression.Property(parameter, prop.Name);
                    var constant = Expression.Constant(value);
                    var equalExpression = Expression.Equal(property, constant);
                    var lambda = Expression.Lambda<Func<Entities.Customer, bool>>(equalExpression, parameter);

                    query = query.Where(lambda);
                }
            }
        }
        
        return await query.ToListAsync();
    }
}

