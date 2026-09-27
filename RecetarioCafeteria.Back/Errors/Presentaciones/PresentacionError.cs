using RecetarioCafeteria.Back.Errors.Common;

namespace RecetarioCafeteria.Back.Errors.Presentaciones;

public abstract record PresentacionError(string Message) : DomainError(Message)
{
    public sealed record NotFound(int Id)
        : PresentacionError($"No se ha encontrado ninguna presentación con Id {Id}");

    public sealed record NotFoundByReceta(int RecetaId)
        : PresentacionError($"La receta con Id {RecetaId} no tiene presentación");

    public sealed record Validation(IEnumerable<string> Errors)
        : PresentacionError($"Errores de validacion:{Environment.NewLine}• {string.Join($"{Environment.NewLine}•", Errors)}");

    public sealed record DatabaseError(string Details)
        : PresentacionError($"Error de base de datos: {Details}");
}

public static class PresentacionErrors
{
    public static DomainError NotFound(int id) => new PresentacionError.NotFound(id);
    public static DomainError NotFoundByReceta(int recetaId) => new PresentacionError.NotFoundByReceta(recetaId);
    public static DomainError Validation(IEnumerable<string> errors) => new PresentacionError.Validation(errors);
    public static DomainError DatabaseError(string details) => new PresentacionError.DatabaseError(details);
}
