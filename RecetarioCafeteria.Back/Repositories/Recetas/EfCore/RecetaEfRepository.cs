using RecetarioCafeteria.Back.Entity;
using RecetarioCafeteria.Back.Errors.Common;
using RecetarioCafeteria.Back.Errors.Recetas;
using RecetarioCafeteria.Back.Models;
using RecetarioCafeteria.Back.Mappers;
using RecetarioCafeteria.Back.Repositories.Recetas.Base;
using CSharpFunctionalExtensions;
using Microsoft.EntityFrameworkCore;

namespace RecetarioCafeteria.Back.Repositories.Recetas.EfCore;

public class RecetaEfRepository : IRecetaRepository
{
    private readonly string _connectionString;
    private bool _initialized;

    public RecetaEfRepository(string connection)
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

    public async Task<IEnumerable<Receta>> GetAllAsync()
    {
        await InitializeAsync();
        using var context = CreateContext();
        return await context.Recetas
            .Include(e => e.Ingredientes)
            .Include(e => e.Pasos)
            .Select(e => e.ToReceta())
            .ToListAsync();
    }

    public async Task<Result<Receta, DomainError>> GetByIdAsync(int id)
    {
        await InitializeAsync();
        using var context = CreateContext();
        var entity = await context.Recetas
            .Include(e => e.Ingredientes)
            .Include(e => e.Pasos)
            .FirstOrDefaultAsync(e => e.Id == id);
        if (entity is null)
            return Result.Failure<Receta, DomainError>(RecetaErrors.NotFound(id));
        return Result.Success<Receta, DomainError>(entity.ToReceta());
    }

    public async Task<Result<Receta, DomainError>> CreateAsync(Receta receta)
    {
        await InitializeAsync();
        try
        {
            using var context = CreateContext();
            var entity = receta.ToEntity();
            context.Recetas.Add(entity);
            await context.SaveChangesAsync();
            return Result.Success<Receta, DomainError>(entity.ToReceta());
        }
        catch (Exception ex)
        {
            return Result.Failure<Receta, DomainError>(RecetaErrors.DatabaseError(ex.Message));
        }
    }

    public async Task<Result<Receta, DomainError>> UpdateAsync(int id, Receta receta)
    {
        await InitializeAsync();
        using var context = CreateContext();
        var entity = await context.Recetas
            .Include(e => e.Ingredientes)
            .Include(e => e.Pasos)
            .FirstOrDefaultAsync(e => e.Id == id);
        if (entity is null)
            return Result.Failure<Receta, DomainError>(RecetaErrors.NotFound(id));
        try
        {
            entity.Titulo = receta.Titulo;
            entity.Categoria = receta.Categoria;
            entity.TiempoMinutos = receta.TiempoMinutos;
            entity.FotoUrl = receta.FotoUrl;

            entity.Ingredientes.Clear();
            entity.Ingredientes.AddRange(receta.Ingredientes.Select(i => i.ToEntity()));

            entity.Pasos.Clear();
            entity.Pasos.AddRange(receta.Pasos.Select(p => p.ToEntity()));

            await context.SaveChangesAsync();
            return Result.Success<Receta, DomainError>(entity.ToReceta());
        }
        catch (Exception ex)
        {
            return Result.Failure<Receta, DomainError>(RecetaErrors.DatabaseError(ex.Message));
        }
    }

    public async Task<Result<Receta, DomainError>> DeleteAsync(int id)
    {
        await InitializeAsync();
        using var context = CreateContext();
        var entity = await context.Recetas.FindAsync(id);
        if (entity is null)
            return Result.Failure<Receta, DomainError>(RecetaErrors.NotFound(id));
        try
        {
            context.Recetas.Remove(entity);
            await context.SaveChangesAsync();
            return Result.Success<Receta, DomainError>(entity.ToReceta());
        }
        catch (Exception ex)
        {
            return Result.Failure<Receta, DomainError>(RecetaErrors.DatabaseError(ex.Message));
        }
    }

    public async Task<IEnumerable<Receta>> GetByCategoriaAsync(CategoriaReceta categoria)
    {
        await InitializeAsync();
        using var context = CreateContext();
        return await context.Recetas
            .Include(e => e.Ingredientes)
            .Include(e => e.Pasos)
            .Where(e => e.Categoria == categoria)
            .Select(e => e.ToReceta())
            .ToListAsync();
    }
}
