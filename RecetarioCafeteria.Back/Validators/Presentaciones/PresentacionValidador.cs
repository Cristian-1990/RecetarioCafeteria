using RecetarioCafeteria.Back.Errors.Common;
using RecetarioCafeteria.Back.Errors.Presentaciones;
using RecetarioCafeteria.Back.Models;
using RecetarioCafeteria.Back.Validators.Common;
using CSharpFunctionalExtensions;

namespace RecetarioCafeteria.Back.Validators.Presentaciones;

public class PresentacionValidador : IValidador<Presentacion>
{
    public Result<Presentacion, DomainError> Validar(Presentacion presentacion)
    {
        var errores = new List<string>();

        if (presentacion.Pasos.Count == 0)
            errores.Add("La presentación debe tener al menos un paso");

        if (errores.Any())
            return Result.Failure<Presentacion, DomainError>(PresentacionErrors.Validation(errores));
        return Result.Success<Presentacion, DomainError>(presentacion);
    }
}
