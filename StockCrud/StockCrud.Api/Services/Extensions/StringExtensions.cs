namespace StockCrud.Api.Services.Extensions;

public static class StringExtensions
{
    public static bool IsCPF(this string value)
    {
        return value.Length <= 11;
    }

    public static bool IsEmail(this string value)
    {
        return value.Contains('@');
    }

}