using RecetarioCafeteria.Back.Errors.Common;
using RecetarioCafeteria.Back.Models;
using CSharpFunctionalExtensions;
namespace RecetarioCafeteria.Back.Repositories.Recetas.Base;

public interface IRecetaRepository
{
    Task<IEnumerable<Receta>> GetAllAsync();
    Task<Result<Receta, DomainError>> GetByIdAsync(int id);
    Task<Result<Receta, DomainError>> CreateAsync(Receta receta);
    Task<Result<Receta, DomainError>> UpdateAsync(int id, Receta receta);
    Task<Result<Receta, DomainError>> DeleteAsync(int id);
    Task<IEnumerable<Receta>> GetByCategoriaAsync(CategoriaReceta categoria);
}
