using System.Text.Json;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.ChangeTracking;
using RecetarioCafeteria.Back.Models;

namespace RecetarioCafeteria.Back.Entity;

public class AppDbContext : DbContext
{
    private readonly string _connectionString; //Guarda la ruta al archivo SQLite

    public AppDbContext(string connectionString)
    {
        _connectionString = connectionString;
    }

    public DbSet<RecetaEntity> Recetas { get; set; } = null!;
    public DbSet<IngredienteEntity> Ingredientes { get; set; } = null!;
    public DbSet<PasoEntity> Pasos { get; set; } = null!;

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    {
        if (!optionsBuilder.IsConfigured)
            optionsBuilder.UseSqlite(_connectionString);
    }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<RecetaEntity>()
            .HasMany(r => r.Ingredientes)
            .WithOne(i => i.Receta)
            .HasForeignKey(i => i.RecetaId)
            .OnDelete(DeleteBehavior.Cascade);

        modelBuilder.Entity<RecetaEntity>()
            .HasMany(r => r.Pasos)
            .WithOne(p => p.Receta)
            .HasForeignKey(p => p.RecetaId)
            .OnDelete(DeleteBehavior.Cascade);

        modelBuilder.Entity<RecetaEntity>()
            .Property(r => r.Alergenos)
            .HasConversion(
                v => string.Join(',', v.Select(a => a.ToString())),
                v => v.Length == 0
                    ? new List<Alergeno>()
                    : v.Split(',', StringSplitOptions.RemoveEmptyEntries).Select(Enum.Parse<Alergeno>).ToList())
            .Metadata.SetValueComparer(new ValueComparer<List<Alergeno>>(
                (a, b) => a!.SequenceEqual(b!),
                a => a.Aggregate(0, (hash, v) => HashCode.Combine(hash, v)),
                a => a.ToList()));

        modelBuilder.Entity<RecetaEntity>()
            .Property(r => r.Utensilios)
            .HasConversion(
                v => JsonSerializer.Serialize(v, JsonSerializerOptions.Default),
                v => JsonSerializer.Deserialize<List<string>>(v, JsonSerializerOptions.Default) ?? new List<string>())
            .Metadata.SetValueComparer(new ValueComparer<List<string>>(
                (a, b) => a!.SequenceEqual(b!),
                a => a.Aggregate(0, (hash, v) => HashCode.Combine(hash, v)),
                a => a.ToList()));

        modelBuilder.Entity<RecetaEntity>()
            .Property(r => r.MiseEnPlace)
            .HasConversion(
                v => JsonSerializer.Serialize(v, JsonSerializerOptions.Default),
                v => JsonSerializer.Deserialize<List<string>>(v, JsonSerializerOptions.Default) ?? new List<string>())
            .Metadata.SetValueComparer(new ValueComparer<List<string>>(
                (a, b) => a!.SequenceEqual(b!),
                a => a.Aggregate(0, (hash, v) => HashCode.Combine(hash, v)),
                a => a.ToList()));
    }

    /// <summary>
    /// Crea un archivo .db y las tablas si no existen, si ya existen no hace nada
    /// </summary>
    public async Task EnsureCreatedAsync()
    {
        await Database.EnsureCreatedAsync();
    }
}
