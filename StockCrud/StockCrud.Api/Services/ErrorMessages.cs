using System.Diagnostics;
using StockCrud.Api.Services.Enums;

namespace StockCrud.Api.Services;

public static class ErrorMessages
{
    public static string GetMessage(ErrorCodes error)
    {
        return error switch
        {
            ErrorCodes.NotFound => "Nenhum resultado foi encontrado.",
            ErrorCodes.InternalServerError => "Um erro ocorreu ao executar a operação.",
            ErrorCodes.BadRequestIfNull => "Parâmetro não pode ser vazio.",
            ErrorCodes.BadRequest => "Conteúdo da solicitação incorreto.",
            ErrorCodes.BadRequestOutStock => "Quantidade em estoque insuficiente.",
            ErrorCodes.InvalidRequest => "Solicitação inválida",
            ErrorCodes.Unauthorized => "Operação não permitida",
            ErrorCodes.Forbidden => "Problema",
            ErrorCodes.Conflict => "Operação não permitida",
            _ => throw new ArgumentOutOfRangeException(nameof(error), error, null)
        };
    }
}