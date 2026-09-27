using RecetarioCafeteria.Back.Errors.Common;
using RecetarioCafeteria.Back.Models;
using CSharpFunctionalExtensions;
namespace RecetarioCafeteria.Back.Repositories.Presentaciones.Base;

public interface IPresentacionRepository
{
    Task<IEnumerable<Presentacion>> GetAllAsync();
    Task<Result<Presentacion, DomainError>> GetByRecetaIdAsync(int recetaId);
    Task<Result<Presentacion, DomainError>> CreateAsync(Presentacion presentacion);
    Task<Result<Presentacion, DomainError>> UpdateAsync(int id, Presentacion presentacion);
}
