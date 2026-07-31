using RecetarioCafeteria.Back.Errors.Recetas;
using RecetarioCafeteria.Back.Models;
using RecetarioCafeteria.Back.Repositories.Recetas.EfCore;
using FluentAssertions;

namespace RecetarioCafeteria.Test.Repositories;

[TestFixture]
public class RecetaEfRepositoryTest
{
    private string _dbPath;
    private string _connectionString;
    private RecetaEfRepository _repository;

    [SetUp]
    public void Setup()
    {
        // Base de datos temporal única por test
        _dbPath = Path.Combine(Path.GetTempPath(), $"recetario_test_{Guid.NewGuid()}.db");
        _connectionString = $"Data Source={_dbPath}";
        _repository = new RecetaEfRepository(_connectionString);
    }

    [TearDown]
    public void TearDown()
    {
        // Fuerza a SQLite a soltar el archivo antes de borrarlo
        Microsoft.Data.Sqlite.SqliteConnection.ClearAllPools();

        if (File.Exists(_dbPath))
            File.Delete(_dbPath);
    }

    private static Receta RecetaValida() => new()
    {
        Titulo = "Café con leche",
        Categoria = CategoriaReceta.Cafeteria,
        TiempoMinutos = 5,
        FotoUrl = "cafe-con-leche.jpg",
        Ingredientes =
        [
            new Ingrediente { Nombre = "Café", Cantidad = 10, Unidad = UnidadMedida.Gramos },
            new Ingrediente { Nombre = "Leche", Cantidad = 100, Unidad = UnidadMedida.Mililitros }
        ],
        Pasos =
        [
            new Paso { Orden = 1, Descripcion = "Moler el café", Fase = FasePaso.Preparacion },
            new Paso { Orden = 1, Descripcion = "Calentar la leche", Fase = FasePaso.Elaboracion }
        ]
    };

    [Test]
    public async Task CreateAsync_GuardaReceta()
    {
        // Arrange
        var receta = RecetaValida();

        // Act
        var resultado = await _repository.CreateAsync(receta);

        // Assert
        resultado.IsSuccess.Should().BeTrue();
        resultado.Value.Id.Should().BeGreaterThan(0);
    }

    [Test]
    public async Task GetByIdAsync_RecetaExiste_LaDevuelve()
    {
        // Arrange
        var creada = await _repository.CreateAsync(RecetaValida());

        // Act
        var resultado = await _repository.GetByIdAsync(creada.Value.Id);

        // Assert
        resultado.IsSuccess.Should().BeTrue();
        resultado.Value.Titulo.Should().Be("Café con leche");
        resultado.Value.Ingredientes.Should().HaveCount(2);
        resultado.Value.Pasos.Should().HaveCount(2);
    }

    [Test]
    public async Task GetByIdAsync_RecetaNoExiste_DevuelveNotFound()
    {
        // Act
        var resultado = await _repository.GetByIdAsync(999);

        // Assert
        resultado.IsFailure.Should().BeTrue();
        resultado.Error.Should().BeOfType<RecetaError.NotFound>();
    }

    [Test]
    public async Task GetAllAsync_DevuelveTodas()
    {
        // Arrange
        await _repository.CreateAsync(RecetaValida());
        await _repository.CreateAsync(RecetaValida() with { Titulo = "Té verde", Categoria = CategoriaReceta.Infusiones });

        // Act
        var recetas = await _repository.GetAllAsync();

        // Assert
        recetas.Should().HaveCount(2);
    }

    [Test]
    public async Task UpdateAsync_ModificaReceta()
    {
        // Arrange
        var creada = await _repository.CreateAsync(RecetaValida());
        var modificada = creada.Value with { TiempoMinutos = 10 };

        // Act
        var resultado = await _repository.UpdateAsync(creada.Value.Id, modificada);

        // Assert
        resultado.IsSuccess.Should().BeTrue();
        resultado.Value.TiempoMinutos.Should().Be(10);
    }

    [Test]
    public async Task DeleteAsync_EliminaReceta()
    {
        // Arrange
        var creada = await _repository.CreateAsync(RecetaValida());

        // Act
        var resultado = await _repository.DeleteAsync(creada.Value.Id);

        // Assert
        resultado.IsSuccess.Should().BeTrue();
        var comprobar = await _repository.GetByIdAsync(creada.Value.Id);
        comprobar.IsFailure.Should().BeTrue();
    }

    [Test]
    public async Task GetByCategoriaAsync_DevuelveSoloLaCategoriaIndicada()
    {
        // Arrange
        await _repository.CreateAsync(RecetaValida());
        await _repository.CreateAsync(RecetaValida() with { Titulo = "Té verde", Categoria = CategoriaReceta.Infusiones });

        // Act
        var resultado = await _repository.GetByCategoriaAsync(CategoriaReceta.Cafeteria);

        // Assert
        resultado.Should().HaveCount(1);
        resultado.First().Titulo.Should().Be("Café con leche");
    }
}
