using Microsoft.EntityFrameworkCore;
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
    }

    /// <summary>
    /// Crea un archivo .db y las tablas si no existen, si ya existen no hace nada
    /// </summary>
    public async Task EnsureCreatedAsync()
    {
        await Database.EnsureCreatedAsync();
    }
}
