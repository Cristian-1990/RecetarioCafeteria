using CSharpFunctionalExtensions;
using RecetarioCafeteria.Back.Errors.Common;
using RecetarioCafeteria.Back.Models;

namespace RecetarioCafeteria.Back.Services.Presentaciones;

public interface IPresentacionService
{
    Task<IEnumerable<Presentacion>> GetAllAsync();
    Task<Result<Presentacion, DomainError>> GetByRecetaIdAsync(int recetaId);
    Task<Result<Presentacion, DomainError>> CreateAsync(Presentacion presentacion);
    Task<Result<Presentacion, DomainError>> UpdateAsync(int id, Presentacion presentacion);
}
