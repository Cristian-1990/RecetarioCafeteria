using RecetarioCafeteria.Back.Errors.Common;

namespace RecetarioCafeteria.Back.Errors.Recetas;

public abstract record RecetaError(string Message) : DomainError(Message)
{
    public sealed record NotFound(int Id)
        : RecetaError($"No se ha encontrado ninguna receta con Id {Id}");

    public sealed record Validation(IEnumerable<string> Errors)
        : RecetaError($"Errores de validacion:{Environment.NewLine}• {string.Join($"{Environment.NewLine}•", Errors)}");

    public sealed record DatabaseError(string Details)
        : RecetaError($"Error de base de datos: {Details}");
}

public static class RecetaErrors
{
    public static DomainError NotFound(int id) => new RecetaError.NotFound(id);
    public static DomainError Validation(IEnumerable<string> errors) => new RecetaError.Validation(errors);
    public static DomainError DatabaseError(string details) => new RecetaError.DatabaseError(details);
}
