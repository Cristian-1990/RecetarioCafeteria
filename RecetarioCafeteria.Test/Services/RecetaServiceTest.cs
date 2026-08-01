using RecetarioCafeteria.Back.Errors.Common;
using RecetarioCafeteria.Back.Errors.Recetas;
using RecetarioCafeteria.Back.Models;
using RecetarioCafeteria.Back.Repositories.Recetas.Base;
using RecetarioCafeteria.Back.Services.Recetas;
using RecetarioCafeteria.Back.Validators.Recetas;
using CSharpFunctionalExtensions;
using FluentAssertions;
using Moq;

namespace RecetarioCafeteria.Test.Services;

[TestFixture]
public class RecetaServiceTest
{
    private Mock<IRecetaRepository> _repositoryMock;
    private RecetaService _service;

    [SetUp]
    public void Setup()
    {
        _repositoryMock = new Mock<IRecetaRepository>();
        _service = new RecetaService(_repositoryMock.Object, new RecetaValidador());
    }

    private static Receta RecetaValida() => new()
    {
        Titulo = "Café con leche",
        Categoria = CategoriaReceta.Cafe,
        TiempoMinutos = 5,
        Ingredientes =
        [
            new Ingrediente { Nombre = "Café", Cantidad = 10, Unidad = UnidadMedida.Gramos }
        ],
        Pasos =
        [
            new Paso { Orden = 1, Descripcion = "Moler el café", Fase = FasePaso.Preparacion },
            new Paso { Orden = 1, Descripcion = "Calentar la leche", Fase = FasePaso.Elaboracion }
        ]
    };

    [Test]
    public async Task CreateAsync_RecetaValida_LlamaAlRepositorio()
    {
        // Arrange
        var receta = RecetaValida();
        _repositoryMock
            .Setup(r => r.CreateAsync(It.IsAny<Receta>()))
            .ReturnsAsync(Result.Success<Receta, DomainError>(receta));

        // Act
        var resultado = await _service.CreateAsync(receta);

        // Assert
        resultado.IsSuccess.Should().BeTrue();
        _repositoryMock.Verify(r => r.CreateAsync(It.IsAny<Receta>()), Times.Once);
    }

    [Test]
    public async Task CreateAsync_RecetaInvalida_NoLlamaAlRepositorio()
    {
        // Arrange
        var receta = RecetaValida();
        receta.Pasos = receta.Pasos.Where(p => p.Fase != FasePaso.Elaboracion).ToList();

        // Act
        var resultado = await _service.CreateAsync(receta);

        // Assert
        resultado.IsFailure.Should().BeTrue();
        _repositoryMock.Verify(r => r.CreateAsync(It.IsAny<Receta>()), Times.Never);
    }

    [Test]
    public async Task UpdateAsync_RecetaValida_LlamaAlRepositorio()
    {
        // Arrange
        var receta = RecetaValida();
        _repositoryMock
            .Setup(r => r.UpdateAsync(1, It.IsAny<Receta>()))
            .ReturnsAsync(Result.Success<Receta, DomainError>(receta));

        // Act
        var resultado = await _service.UpdateAsync(1, receta);

        // Assert
        resultado.IsSuccess.Should().BeTrue();
        _repositoryMock.Verify(r => r.UpdateAsync(1, It.IsAny<Receta>()), Times.Once);
    }

    [Test]
    public async Task UpdateAsync_RecetaInvalida_NoLlamaAlRepositorio()
    {
        // Arrange
        var receta = RecetaValida();
        receta.Ingredientes[0].Cantidad = 0;

        // Act
        var resultado = await _service.UpdateAsync(1, receta);

        // Assert
        resultado.IsFailure.Should().BeTrue();
        _repositoryMock.Verify(r => r.UpdateAsync(It.IsAny<int>(), It.IsAny<Receta>()), Times.Never);
    }

    [Test]
    public async Task GetByIdAsync_RecetaExiste_DevuelveReceta()
    {
        // Arrange
        var receta = RecetaValida() with { Id = 1 };
        _repositoryMock
            .Setup(r => r.GetByIdAsync(1))
            .ReturnsAsync(Result.Success<Receta, DomainError>(receta));

        // Act
        var resultado = await _service.GetByIdAsync(1);

        // Assert
        resultado.IsSuccess.Should().BeTrue();
        resultado.Value.Titulo.Should().Be("Café con leche");
    }

    [Test]
    public async Task GetByIdAsync_RecetaNoExiste_DevuelveFailure()
    {
        // Arrange
        _repositoryMock
            .Setup(r => r.GetByIdAsync(99))
            .ReturnsAsync(Result.Failure<Receta, DomainError>(RecetaErrors.NotFound(99)));

        // Act
        var resultado = await _service.GetByIdAsync(99);

        // Assert
        resultado.IsFailure.Should().BeTrue();
    }

    [Test]
    public async Task DeleteAsync_DelegaEnRepositorio()
    {
        // Arrange
        var receta = RecetaValida() with { Id = 1 };
        _repositoryMock
            .Setup(r => r.DeleteAsync(1))
            .ReturnsAsync(Result.Success<Receta, DomainError>(receta));

        // Act
        var resultado = await _service.DeleteAsync(1);

        // Assert
        resultado.IsSuccess.Should().BeTrue();
        _repositoryMock.Verify(r => r.DeleteAsync(1), Times.Once);
    }

    [Test]
    public async Task GetAllAsync_DelegaEnRepositorio()
    {
        // Arrange
        var recetas = new List<Receta> { RecetaValida(), RecetaValida() with { Titulo = "Té verde" } };
        _repositoryMock.Setup(r => r.GetAllAsync()).ReturnsAsync(recetas);

        // Act
        var resultado = await _service.GetAllAsync();

        // Assert
        resultado.Should().HaveCount(2);
    }

    [Test]
    public async Task GetByCategoriaAsync_DelegaEnRepositorio()
    {
        // Arrange
        var recetas = new List<Receta> { RecetaValida() };
        _repositoryMock
            .Setup(r => r.GetByCategoriaAsync(CategoriaReceta.Cafe))
            .ReturnsAsync(recetas);

        // Act
        var resultado = await _service.GetByCategoriaAsync(CategoriaReceta.Cafe);

        // Assert
        resultado.Should().HaveCount(1);
        _repositoryMock.Verify(r => r.GetByCategoriaAsync(CategoriaReceta.Cafe), Times.Once);
    }
}
