using RecetarioCafeteria.Back.Entity;
using RecetarioCafeteria.Back.Errors.Common;
using RecetarioCafeteria.Back.Errors.Recetas;
using RecetarioCafeteria.Back.Models;
using RecetarioCafeteria.Back.Mappers;
using RecetarioCafeteria.Back.Repositories.Recetas.Base;
using CSharpFunctionalExtensions;
using Microsoft.EntityFrameworkCore;
using Serilog;

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
            Log.Error(ex, "Error al crear la receta {Titulo}", receta.Titulo);
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
            entity.Alergenos = receta.Alergenos;
            entity.Utensilios = receta.Utensilios;
            entity.MiseEnPlace = receta.MiseEnPlace;
            entity.NotaFinal = receta.NotaFinal;

            entity.Ingredientes.Clear();
            entity.Ingredientes.AddRange(receta.Ingredientes.Select(i => i.ToEntity()));

            entity.Pasos.Clear();
            entity.Pasos.AddRange(receta.Pasos.Select(p => p.ToEntity()));

            await context.SaveChangesAsync();
            return Result.Success<Receta, DomainError>(entity.ToReceta());
        }
        catch (Exception ex)
        {
            Log.Error(ex, "Error al editar la receta {Id}", id);
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
            await RenumerarIdsAsync(context, id);
            return Result.Success<Receta, DomainError>(entity.ToReceta());
        }
        catch (Exception ex)
        {
            Log.Error(ex, "Error al eliminar la receta {Id}", id);
            return Result.Failure<Receta, DomainError>(RecetaErrors.DatabaseError(ex.Message));
        }
    }

    /// <summary>
    /// Tras borrar la receta con Id igual a idEliminado, desplaza en -1 el Id de todas las
    /// recetas posteriores (y el RecetaId de sus ingredientes/pasos) para que los Ids queden
    /// contiguos. Los FK de SQLite se desactivan temporalmente porque el desplazamiento pasa
    /// por estados intermedios donde padre e hijo no coinciden.
    /// </summary>
    private static async Task RenumerarIdsAsync(AppDbContext context, int idEliminado)
    {
        var idsAReordenar = await context.Recetas
            .Where(r => r.Id > idEliminado)
            .OrderBy(r => r.Id)
            .Select(r => r.Id)
            .ToListAsync();

        if (idsAReordenar.Count == 0)
            return;

        await context.Database.ExecuteSqlRawAsync("PRAGMA foreign_keys = OFF;");

        await using var transaction = await context.Database.BeginTransactionAsync();

        foreach (var idActual in idsAReordenar)
        {
            var idNuevo = idActual - 1;
            await context.Database.ExecuteSqlInterpolatedAsync($"UPDATE Pasos SET RecetaId = {idNuevo} WHERE RecetaId = {idActual}");
            await context.Database.ExecuteSqlInterpolatedAsync($"UPDATE Ingredientes SET RecetaId = {idNuevo} WHERE RecetaId = {idActual}");
            await context.Database.ExecuteSqlInterpolatedAsync($"UPDATE Recetas SET Id = {idNuevo} WHERE Id = {idActual}");
        }

        await transaction.CommitAsync();

        await context.Database.ExecuteSqlRawAsync("PRAGMA foreign_keys = ON;");
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
