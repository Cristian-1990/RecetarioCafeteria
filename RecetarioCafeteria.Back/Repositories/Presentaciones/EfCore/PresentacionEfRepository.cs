using RecetarioCafeteria.Back.Entity;
using RecetarioCafeteria.Back.Errors.Common;
using RecetarioCafeteria.Back.Errors.Presentaciones;
using RecetarioCafeteria.Back.Models;
using RecetarioCafeteria.Back.Mappers;
using RecetarioCafeteria.Back.Repositories.Presentaciones.Base;
using CSharpFunctionalExtensions;
using Microsoft.EntityFrameworkCore;
using Serilog;

namespace RecetarioCafeteria.Back.Repositories.Presentaciones.EfCore;

public class PresentacionEfRepository : IPresentacionRepository
{
    private readonly string _connectionString;
    private bool _initialized;

    public PresentacionEfRepository(string connection)
    {
        _connectionString = connection;
    }

    private AppDbContext CreateContext() => new AppDbContext(_connectionString);

    private async Task InitializeAsync()
    {
        if (_initialized) return;
        using var context = CreateContext();
        await context.EnsureCreatedAsync();
        _initialized = true;
    }

    public async Task<IEnumerable<Presentacion>> GetAllAsync()
    {
        await InitializeAsync();
        using var context = CreateContext();
        return await context.Presentaciones
            .Include(e => e.Pasos)
            .Select(e => e.ToPresentacion())
            .ToListAsync();
    }

    public async Task<Result<Presentacion, DomainError>> GetByRecetaIdAsync(int recetaId)
    {
        await InitializeAsync();
        using var context = CreateContext();
        var entity = await context.Presentaciones
            .Include(e => e.Pasos)
            .FirstOrDefaultAsync(e => e.RecetaId == recetaId);
        if (entity is null)
            return Result.Failure<Presentacion, DomainError>(PresentacionErrors.NotFoundByReceta(recetaId));
        return Result.Success<Presentacion, DomainError>(entity.ToPresentacion());
    }

    public async Task<Result<Presentacion, DomainError>> CreateAsync(Presentacion presentacion)
    {
        await InitializeAsync();
        try
        {
            using var context = CreateContext();
            var entity = presentacion.ToEntity();
            context.Presentaciones.Add(entity);
            await context.SaveChangesAsync();
            return Result.Success<Presentacion, DomainError>(entity.ToPresentacion());
        }
        catch (Exception ex)
        {
            Log.Error(ex, "Error al crear la presentación de la receta {RecetaId}", presentacion.RecetaId);
            return Result.Failure<Presentacion, DomainError>(PresentacionErrors.DatabaseError(ex.Message));
        }
    }

    public async Task<Result<Presentacion, DomainError>> UpdateAsync(int id, Presentacion presentacion)
    {
        await InitializeAsync();
        using var context = CreateContext();
        var entity = await context.Presentaciones
            .Include(e => e.Pasos)
            .FirstOrDefaultAsync(e => e.Id == id);
        if (entity is null)
            return Result.Failure<Presentacion, DomainError>(PresentacionErrors.NotFound(id));
        try
        {
            entity.FotoUrl = presentacion.FotoUrl;

            entity.Pasos.Clear();
            entity.Pasos.AddRange(presentacion.Pasos.Select(p => p.ToEntity()));

            await context.SaveChangesAsync();
            return Result.Success<Presentacion, DomainError>(entity.ToPresentacion());
        }
        catch (Exception ex)
        {
            Log.Error(ex, "Error al editar la presentación {Id}", id);
            return Result.Failure<Presentacion, DomainError>(PresentacionErrors.DatabaseError(ex.Message));
        }
    }
}
