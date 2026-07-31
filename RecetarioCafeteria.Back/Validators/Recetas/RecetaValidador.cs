using RecetarioCafeteria.Back.Errors.Common;
using RecetarioCafeteria.Back.Errors.Recetas;
using RecetarioCafeteria.Back.Models;
using RecetarioCafeteria.Back.Validators.Common;
using CSharpFunctionalExtensions;

namespace RecetarioCafeteria.Back.Validators.Recetas;

public class RecetaValidador : IValidador<Receta>
{
    public Result<Receta, DomainError> Validar(Receta receta)
    {
        var errores = new List<string>();

        foreach (var ingrediente in receta.Ingredientes)
        {
            if (ingrediente.Cantidad <= 0)
                errores.Add($"La cantidad del ingrediente '{ingrediente.Nombre}' debe ser mayor que 0");
        }

        var ordenesDuplicados = receta.Pasos
            .GroupBy(p => new { p.Fase, p.Orden })
            .Where(g => g.Count() > 1)
            .Select(g => g.Key);
        foreach (var duplicado in ordenesDuplicados)
            errores.Add($"El orden {duplicado.Orden} está duplicado en la fase {duplicado.Fase}");

        if (!receta.Pasos.Any(p => p.Fase == FasePaso.Preparacion))
            errores.Add("La receta debe tener al menos un paso de Preparación");

        if (!receta.Pasos.Any(p => p.Fase == FasePaso.Elaboracion))
            errores.Add("La receta debe tener al menos un paso de Elaboración");

        if (errores.Any())
            return Result.Failure<Receta, DomainError>(RecetaErrors.Validation(errores));
        return Result.Success<Receta, DomainError>(receta);
    }
}
