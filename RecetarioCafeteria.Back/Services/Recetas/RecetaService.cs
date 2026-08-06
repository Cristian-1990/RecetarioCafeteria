using CSharpFunctionalExtensions;
using RecetarioCafeteria.Back.Errors.Common;
using RecetarioCafeteria.Back.Models;
using RecetarioCafeteria.Back.Repositories.Recetas.Base;
using RecetarioCafeteria.Back.Validators.Common;
using Serilog;

namespace RecetarioCafeteria.Back.Services.Recetas;

public class RecetaService : IRecetaService
{
    private readonly IRecetaRepository _repository;
    private readonly IValidador<Receta> _validador;

    public RecetaService(IRecetaRepository repository, IValidador<Receta> validador)
    {
        _repository = repository;
        _validador = validador;
    }

    public async Task<IEnumerable<Receta>> GetAllAsync()
    {
        return await _repository.GetAllAsync();
    }

    public async Task<Result<Receta, DomainError>> GetByIdAsync(int id)
    {
        return await _repository.GetByIdAsync(id);
    }

    public async Task<Result<Receta, DomainError>> CreateAsync(Receta receta)
    {
        var validacion = _validador.Validar(receta);
        if (validacion.IsFailure)
            return validacion;

        var resultado = await _repository.CreateAsync(receta);
        if (resultado.IsSuccess)
            Log.Information("Receta creada: Id={Id}, Titulo={Titulo}", resultado.Value.Id, resultado.Value.Titulo);
        return resultado;
    }

    public async Task<Result<Receta, DomainError>> UpdateAsync(int id, Receta receta)
    {
        var validacion = _validador.Validar(receta);
        if (validacion.IsFailure)
            return validacion;

        var resultado = await _repository.UpdateAsync(id, receta);
        if (resultado.IsSuccess)
            Log.Information("Receta editada: Id={Id}, Titulo={Titulo}", id, receta.Titulo);
        return resultado;
    }

    public async Task<Result<Receta, DomainError>> DeleteAsync(int id)
    {
        var resultado = await _repository.DeleteAsync(id);
        if (resultado.IsSuccess)
            Log.Information("Receta eliminada: Id={Id}", id);
        return resultado;
    }

    public async Task<IEnumerable<Receta>> GetByCategoriaAsync(CategoriaReceta categoria)
    {
        return await _repository.GetByCategoriaAsync(categoria);
    }
}
