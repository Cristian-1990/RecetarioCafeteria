using RecetarioCafeteria.Back.Errors.Presentaciones;
using RecetarioCafeteria.Back.Models;
using RecetarioCafeteria.Back.Repositories.Presentaciones.EfCore;
using RecetarioCafeteria.Back.Repositories.Recetas.EfCore;
using FluentAssertions;

namespace RecetarioCafeteria.Test.Repositories;

[TestFixture]
public class PresentacionEfRepositoryTest
{
    private string _dbPath;
    private string _connectionString;
    private PresentacionEfRepository _repository;
    private int _recetaId;

    [SetUp]
    public async Task Setup()
    {
        // Base de datos temporal única por test
        _dbPath = Path.Combine(Path.GetTempPath(), $"recetario_test_{Guid.NewGuid()}.db");
        _connectionString = $"Data Source={_dbPath}";
        _repository = new PresentacionEfRepository(_connectionString);

        // La presentación exige una receta existente (relación 1 a 1 con FK)
        var recetaRepository = new RecetaEfRepository(_connectionString);
        var receta = await recetaRepository.CreateAsync(RecetaValida());
        _recetaId = receta.Value.Id;
    }

    private static Receta RecetaValida() => new()
    {
        Titulo = "Carrot cake",
        Categoria = CategoriaReceta.Reposteria,
        TiempoMinutos = 60,
        Pasos =
        [
            new Paso { Orden = 1, Descripcion = "Mezclar", Fase = FasePaso.Preparacion },
            new Paso { Orden = 1, Descripcion = "Hornear", Fase = FasePaso.Elaboracion }
        ]
    };

    [TearDown]
    public void TearDown()
    {
        // Fuerza a SQLite a soltar el archivo antes de borrarlo
        Microsoft.Data.Sqlite.SqliteConnection.ClearAllPools();

        if (File.Exists(_dbPath))
            File.Delete(_dbPath);
    }

    private Presentacion PresentacionValida() => new()
    {
        RecetaId = _recetaId,
        FotoUrl = "carrot-cake-frosting.jpg",
        Pasos =
        [
            new PasoPresentacion { Orden = 1, Descripcion = "Cubrir con el frosting" },
            new PasoPresentacion { Orden = 2, Descripcion = "Decorar con nueces" }
        ]
    };

    [Test]
    public async Task CreateAsync_GuardaPresentacion()
    {
        // Arrange
        var presentacion = PresentacionValida();

        // Act
        var resultado = await _repository.CreateAsync(presentacion);

        // Assert
        resultado.IsSuccess.Should().BeTrue();
        resultado.Value.Id.Should().BeGreaterThan(0);
    }

    [Test]
    public async Task GetByRecetaIdAsync_PresentacionExiste_LaDevuelve()
    {
        // Arrange
        await _repository.CreateAsync(PresentacionValida());

        // Act
        var resultado = await _repository.GetByRecetaIdAsync(_recetaId);

        // Assert
        resultado.IsSuccess.Should().BeTrue();
        resultado.Value.Pasos.Should().HaveCount(2);
    }

    [Test]
    public async Task GetByRecetaIdAsync_PresentacionNoExiste_DevuelveNotFoundByReceta()
    {
        // Act
        var resultado = await _repository.GetByRecetaIdAsync(999);

        // Assert
        resultado.IsFailure.Should().BeTrue();
        resultado.Error.Should().BeOfType<PresentacionError.NotFoundByReceta>();
    }

    [Test]
    public async Task UpdateAsync_ModificaPresentacion()
    {
        // Arrange
        var creada = await _repository.CreateAsync(PresentacionValida());
        var modificada = creada.Value with { FotoUrl = "otra-foto.jpg" };

        // Act
        var resultado = await _repository.UpdateAsync(creada.Value.Id, modificada);

        // Assert
        resultado.IsSuccess.Should().BeTrue();
        resultado.Value.FotoUrl.Should().Be("otra-foto.jpg");
    }

    [Test]
    public async Task UpdateAsync_PresentacionNoExiste_DevuelveNotFound()
    {
        // Act
        var resultado = await _repository.UpdateAsync(999, PresentacionValida());

        // Assert
        resultado.IsFailure.Should().BeTrue();
        resultado.Error.Should().BeOfType<PresentacionError.NotFound>();
    }

    [Test]
    public async Task GetAllAsync_DevuelveTodas()
    {
        // Arrange
        var recetaRepository = new RecetaEfRepository(_connectionString);
        var otraReceta = await recetaRepository.CreateAsync(RecetaValida() with { Titulo = "Lemon pie" });

        await _repository.CreateAsync(PresentacionValida());
        await _repository.CreateAsync(PresentacionValida() with { RecetaId = otraReceta.Value.Id });

        // Act
        var resultado = await _repository.GetAllAsync();

        // Assert
        resultado.Should().HaveCount(2);
    }
}
