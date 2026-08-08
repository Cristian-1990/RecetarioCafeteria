using CSharpFunctionalExtensions;
using RecetarioCafeteria.Back.Errors.Common;
using RecetarioCafeteria.Back.Models;
using RecetarioCafeteria.Back.Repositories.Presentaciones.Base;
using RecetarioCafeteria.Back.Validators.Common;
using Serilog;

namespace RecetarioCafeteria.Back.Services.Presentaciones;

public class PresentacionService : IPresentacionService
{
    private readonly IPresentacionRepository _repository;
    private readonly IValidador<Presentacion> _validador;

    public PresentacionService(IPresentacionRepository repository, IValidador<Presentacion> validador)
    {
        _repository = repository;
        _validador = validador;
    }

    public async Task<IEnumerable<Presentacion>> GetAllAsync()
    {
        return await _repository.GetAllAsync();
    }

    public async Task<Result<Presentacion, DomainError>> GetByRecetaIdAsync(int recetaId)
    {
        return await _repository.GetByRecetaIdAsync(recetaId);
    }

    public async Task<Result<Presentacion, DomainError>> CreateAsync(Presentacion presentacion)
    {
        var validacion = _validador.Validar(presentacion);
        if (validacion.IsFailure)
            return validacion;

        var resultado = await _repository.CreateAsync(presentacion);
        if (resultado.IsSuccess)
            Log.Information("Presentación creada: Id={Id}, RecetaId={RecetaId}", resultado.Value.Id, resultado.Value.RecetaId);
        return resultado;
    }

    public async Task<Result<Presentacion, DomainError>> UpdateAsync(int id, Presentacion presentacion)
    {
        var validacion = _validador.Validar(presentacion);
        if (validacion.IsFailure)
            return validacion;

        var resultado = await _repository.UpdateAsync(id, presentacion);
        if (resultado.IsSuccess)
            Log.Information("Presentación editada: Id={Id}, RecetaId={RecetaId}", id, presentacion.RecetaId);
        return resultado;
    }
}
