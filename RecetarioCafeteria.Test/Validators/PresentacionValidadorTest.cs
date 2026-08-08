using RecetarioCafeteria.Back.Models;
using RecetarioCafeteria.Back.Validators.Presentaciones;
using FluentAssertions;

namespace RecetarioCafeteria.Test.Validators;

[TestFixture]
public class PresentacionValidadorTest
{
    private PresentacionValidador _validador;

    [SetUp]
    public void Setup()
    {
        _validador = new PresentacionValidador();
    }

    private static Presentacion PresentacionValida() => new()
    {
        RecetaId = 1,
        FotoUrl = "carrot-cake-frosting.jpg",
        Pasos =
        [
            new PasoPresentacion { Orden = 1, Descripcion = "Cubrir con el frosting" },
            new PasoPresentacion { Orden = 2, Descripcion = "Decorar con nueces" }
        ]
    };

    [Test]
    public void Validar_PresentacionValida_DevuelveSuccess()
    {
        // Arrange
        var presentacion = PresentacionValida();

        // Act
        var resultado = _validador.Validar(presentacion);

        // Assert
        resultado.IsSuccess.Should().BeTrue();
    }

    [Test]
    public void Validar_SinPasos_DevuelveFailure()
    {
        // Arrange
        var presentacion = PresentacionValida() with { Pasos = [] };

        // Act
        var resultado = _validador.Validar(presentacion);

        // Assert
        resultado.IsFailure.Should().BeTrue();
    }
}
