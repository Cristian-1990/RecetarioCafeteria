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
    public DbSet<PresentacionEntity> Presentaciones { get; set; } = null!;
    public DbSet<PasoPresentacionEntity> PasosPresentacion { get; set; } = null!;

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

        // Relación 1 a 1: cada receta tiene como máximo una presentación
        modelBuilder.Entity<PresentacionEntity>()
            .HasOne(p => p.Receta)
            .WithOne()
            .HasForeignKey<PresentacionEntity>(p => p.RecetaId)
            .OnDelete(DeleteBehavior.Cascade);

        modelBuilder.Entity<PresentacionEntity>()
            .HasIndex(p => p.RecetaId)
            .IsUnique();

        modelBuilder.Entity<PresentacionEntity>()
            .HasMany(p => p.Pasos)
            .WithOne(p => p.Presentacion)
            .HasForeignKey(p => p.PresentacionId)
            .OnDelete(DeleteBehavior.Cascade);
    }

    /// <summary>
    /// Crea un archivo .db y las tablas si no existen, si ya existen no hace nada
    /// </summary>
    public async Task EnsureCreatedAsync()
    {
        await Database.EnsureCreatedAsync();

        // EnsureCreatedAsync solo genera el esquema completo cuando el archivo .db es nuevo:
        // si ya existía (como en cualquier instalación previa a la Presentación), lo deja
        // intacto y las tablas añadidas después nunca se crean. Como el proyecto no usa
        // migraciones de EF, se crean aquí a mano y de forma idempotente para que las bases
        // de datos ya existentes se pongan al día sin perder los datos que ya tenían.
        await EnsureTablaPresentacionesAsync();
        await EnsureColumnaConsejoEnPasosPresentacionAsync();
    }

    private async Task EnsureTablaPresentacionesAsync()
    {
        await Database.ExecuteSqlRawAsync("""
            CREATE TABLE IF NOT EXISTS "Presentaciones" (
                "Id" INTEGER NOT NULL CONSTRAINT "PK_Presentaciones" PRIMARY KEY AUTOINCREMENT,
                "FotoUrl" TEXT NOT NULL,
                "RecetaId" INTEGER NOT NULL,
                CONSTRAINT "FK_Presentaciones_Recetas_RecetaId" FOREIGN KEY ("RecetaId") REFERENCES "Recetas" ("Id") ON DELETE CASCADE
            );
            """);
        await Database.ExecuteSqlRawAsync(
            """CREATE UNIQUE INDEX IF NOT EXISTS "IX_Presentaciones_RecetaId" ON "Presentaciones" ("RecetaId");""");

        await Database.ExecuteSqlRawAsync("""
            CREATE TABLE IF NOT EXISTS "PasosPresentacion" (
                "Id" INTEGER NOT NULL CONSTRAINT "PK_PasosPresentacion" PRIMARY KEY AUTOINCREMENT,
                "Orden" INTEGER NOT NULL,
                "Descripcion" TEXT NOT NULL,
                "Consejo" TEXT NULL,
                "PresentacionId" INTEGER NOT NULL,
                CONSTRAINT "FK_PasosPresentacion_Presentaciones_PresentacionId" FOREIGN KEY ("PresentacionId") REFERENCES "Presentaciones" ("Id") ON DELETE CASCADE
            );
            """);
        await Database.ExecuteSqlRawAsync(
            """CREATE INDEX IF NOT EXISTS "IX_PasosPresentacion_PresentacionId" ON "PasosPresentacion" ("PresentacionId");""");
    }

    /// <summary>
    /// La columna Consejo se añadió después de que la tabla PasosPresentacion ya existiera en
    /// bases de datos previas (el CREATE TABLE IF NOT EXISTS de arriba no la añade si la tabla
    /// ya está creada). SQLite no soporta "ADD COLUMN IF NOT EXISTS", así que se intenta el
    /// ALTER y se ignora el error si la columna ya existe (bases de datos nuevas, donde el
    /// CREATE TABLE de arriba ya la incluye desde el principio).
    /// </summary>
    private async Task EnsureColumnaConsejoEnPasosPresentacionAsync()
    {
        try
        {
            await Database.ExecuteSqlRawAsync("""ALTER TABLE "PasosPresentacion" ADD COLUMN "Consejo" TEXT NULL;""");
        }
        catch (Microsoft.Data.Sqlite.SqliteException ex) when (ex.SqliteErrorCode == 1)
        {
        }
    }
}
