using RecetarioCafeteria.Back.Errors.Common;
using RecetarioCafeteria.Back.Errors.Presentaciones;
using RecetarioCafeteria.Back.Models;
using RecetarioCafeteria.Back.Repositories.Presentaciones.Base;
using RecetarioCafeteria.Back.Services.Presentaciones;
using RecetarioCafeteria.Back.Validators.Presentaciones;
using CSharpFunctionalExtensions;
using FluentAssertions;
using Moq;

namespace RecetarioCafeteria.Test.Services;

[TestFixture]
public class PresentacionServiceTest
{
    private Mock<IPresentacionRepository> _repositoryMock;
    private PresentacionService _service;

    [SetUp]
    public void Setup()
    {
        _repositoryMock = new Mock<IPresentacionRepository>();
        _service = new PresentacionService(_repositoryMock.Object, new PresentacionValidador());
    }

    private static Presentacion PresentacionValida() => new()
    {
        RecetaId = 1,
        FotoUrl = "carrot-cake-frosting.jpg",
        Pasos =
        [
            new PasoPresentacion { Orden = 1, Descripcion = "Cubrir con el frosting" }
        ]
    };

    [Test]
    public async Task CreateAsync_PresentacionValida_LlamaAlRepositorio()
    {
        // Arrange
        var presentacion = PresentacionValida();
        _repositoryMock
            .Setup(r => r.CreateAsync(It.IsAny<Presentacion>()))
            .ReturnsAsync(Result.Success<Presentacion, DomainError>(presentacion));

        // Act
        var resultado = await _service.CreateAsync(presentacion);

        // Assert
        resultado.IsSuccess.Should().BeTrue();
        _repositoryMock.Verify(r => r.CreateAsync(It.IsAny<Presentacion>()), Times.Once);
    }

    [Test]
    public async Task CreateAsync_SinPasos_NoLlamaAlRepositorio()
    {
        // Arrange
        var presentacion = PresentacionValida() with { Pasos = [] };

        // Act
        var resultado = await _service.CreateAsync(presentacion);

        // Assert
        resultado.IsFailure.Should().BeTrue();
        _repositoryMock.Verify(r => r.CreateAsync(It.IsAny<Presentacion>()), Times.Never);
    }

    [Test]
    public async Task UpdateAsync_PresentacionValida_LlamaAlRepositorio()
    {
        // Arrange
        var presentacion = PresentacionValida();
        _repositoryMock
            .Setup(r => r.UpdateAsync(1, It.IsAny<Presentacion>()))
            .ReturnsAsync(Result.Success<Presentacion, DomainError>(presentacion));

        // Act
        var resultado = await _service.UpdateAsync(1, presentacion);

        // Assert
        resultado.IsSuccess.Should().BeTrue();
        _repositoryMock.Verify(r => r.UpdateAsync(1, It.IsAny<Presentacion>()), Times.Once);
    }

    [Test]
    public async Task GetByRecetaIdAsync_SinPresentacion_DevuelveFailure()
    {
        // Arrange
        _repositoryMock
            .Setup(r => r.GetByRecetaIdAsync(99))
            .ReturnsAsync(Result.Failure<Presentacion, DomainError>(PresentacionErrors.NotFoundByReceta(99)));

        // Act
        var resultado = await _service.GetByRecetaIdAsync(99);

        // Assert
        resultado.IsFailure.Should().BeTrue();
    }

    [Test]
    public async Task GetByRecetaIdAsync_ConPresentacion_LaDevuelve()
    {
        // Arrange
        var presentacion = PresentacionValida() with { Id = 1 };
        _repositoryMock
            .Setup(r => r.GetByRecetaIdAsync(1))
            .ReturnsAsync(Result.Success<Presentacion, DomainError>(presentacion));

        // Act
        var resultado = await _service.GetByRecetaIdAsync(1);

        // Assert
        resultado.IsSuccess.Should().BeTrue();
        resultado.Value.FotoUrl.Should().Be("carrot-cake-frosting.jpg");
    }

    [Test]
    public async Task GetAllAsync_DelegaEnRepositorio()
    {
        // Arrange
        var presentaciones = new List<Presentacion> { PresentacionValida(), PresentacionValida() with { RecetaId = 2 } };
        _repositoryMock.Setup(r => r.GetAllAsync()).ReturnsAsync(presentaciones);

        // Act
        var resultado = await _service.GetAllAsync();

        // Assert
        resultado.Should().HaveCount(2);
    }
}
