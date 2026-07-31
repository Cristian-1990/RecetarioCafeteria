using RecetarioCafeteria.Back.Models;
using RecetarioCafeteria.Back.Validators.Recetas;
using FluentAssertions;

namespace RecetarioCafeteria.Test.Validators;

[TestFixture]
public class RecetaValidadorTest
{
    private RecetaValidador _validador;

    [SetUp]
    public void Setup()
    {
        _validador = new RecetaValidador();
    }

    private static Receta RecetaValida() => new()
    {
        Titulo = "Café con leche",
        Categoria = CategoriaReceta.Cafeteria,
        TiempoMinutos = 5,
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
    public void Validar_RecetaValida_DevuelveSuccess()
    {
        // Arrange
        var receta = RecetaValida();

        // Act
        var resultado = _validador.Validar(receta);

        // Assert
        resultado.IsSuccess.Should().BeTrue();
    }

    [TestCase(0)]
    [TestCase(-1)]
    public void Validar_CantidadIngredienteInvalida_DevuelveFailure(decimal cantidad)
    {
        // Arrange
        var receta = RecetaValida();
        receta.Ingredientes[0].Cantidad = cantidad;

        // Act
        var resultado = _validador.Validar(receta);

        // Assert
        resultado.IsFailure.Should().BeTrue();
    }

    [Test]
    public void Validar_OrdenDuplicadoEnMismaFase_DevuelveFailure()
    {
        // Arrange
        var receta = RecetaValida();
        receta.Pasos.Add(new Paso { Orden = 1, Descripcion = "Servir", Fase = FasePaso.Preparacion });

        // Act
        var resultado = _validador.Validar(receta);

        // Assert
        resultado.IsFailure.Should().BeTrue();
    }

    [Test]
    public void Validar_SinPasoDePreparacion_DevuelveFailure()
    {
        // Arrange
        var receta = RecetaValida();
        receta.Pasos = receta.Pasos.Where(p => p.Fase != FasePaso.Preparacion).ToList();

        // Act
        var resultado = _validador.Validar(receta);

        // Assert
        resultado.IsFailure.Should().BeTrue();
    }

    [Test]
    public void Validar_SinPasoDeElaboracion_DevuelveFailure()
    {
        // Arrange
        var receta = RecetaValida();
        receta.Pasos = receta.Pasos.Where(p => p.Fase != FasePaso.Elaboracion).ToList();

        // Act
        var resultado = _validador.Validar(receta);

        // Assert
        resultado.IsFailure.Should().BeTrue();
    }
}
