using RecetarioCafeteria.Back.Entity;
using RecetarioCafeteria.Back.Mappers;
using RecetarioCafeteria.Back.Models;
using Microsoft.EntityFrameworkCore;

namespace RecetarioCafeteria.Back.Seed;

/// <summary>
/// Siembra unas pocas recetas de ejemplo para probar el mecanismo de siembra.
/// Las recetas reales de la cafetería se cargarán más adelante con contenido definitivo.
/// </summary>
public static class RecetaSeeder
{
    public static async Task SeedAsync(AppDbContext context)
    {
        await context.EnsureCreatedAsync();

        if (await context.Recetas.AnyAsync())
            return;

        var recetas = new List<Receta>
        {
            new Receta
            {
                Titulo = "Café con leche",
                Categoria = CategoriaReceta.Cafe,
                TiempoMinutos = 5,
                FotoUrl = "https://example.com/fotos/cafe-con-leche.jpg",
                Ingredientes =
                [
                    new Ingrediente { Nombre = "Café molido", Cantidad = 18, Unidad = UnidadMedida.Gramos },
                    new Ingrediente { Nombre = "Leche", Cantidad = 150, Unidad = UnidadMedida.Mililitros },
                    new Ingrediente { Nombre = "Agua", Cantidad = 30, Unidad = UnidadMedida.Mililitros }
                ],
                Pasos =
                [
                    new Paso { Orden = 1, Descripcion = "Moler el café", Fase = FasePaso.Preparacion },
                    new Paso { Orden = 2, Descripcion = "Calentar el agua a 92°C", Fase = FasePaso.Preparacion },
                    new Paso { Orden = 1, Descripcion = "Extraer el espresso", Fase = FasePaso.Elaboracion },
                    new Paso { Orden = 2, Descripcion = "Vaporizar la leche", Fase = FasePaso.Elaboracion },
                    new Paso { Orden = 3, Descripcion = "Verter la leche sobre el café", Fase = FasePaso.Elaboracion }
                ]
            },
            new Receta
            {
                Titulo = "Té verde",
                Categoria = CategoriaReceta.Bebidas,
                TiempoMinutos = 4,
                FotoUrl = "https://example.com/fotos/te-verde.jpg",
                Ingredientes =
                [
                    new Ingrediente { Nombre = "Hojas de té verde", Cantidad = 3, Unidad = UnidadMedida.Gramos },
                    new Ingrediente { Nombre = "Agua", Cantidad = 200, Unidad = UnidadMedida.Mililitros }
                ],
                Pasos =
                [
                    new Paso { Orden = 1, Descripcion = "Calentar el agua a 80°C", Fase = FasePaso.Preparacion },
                    new Paso { Orden = 1, Descripcion = "Infusionar las hojas durante 3 minutos", Fase = FasePaso.Elaboracion },
                    new Paso { Orden = 2, Descripcion = "Colar y servir", Fase = FasePaso.Elaboracion }
                ]
            },
            new Receta
            {
                Titulo = "Tostada con aguacate",
                Categoria = CategoriaReceta.Salados,
                TiempoMinutos = 10,
                FotoUrl = "https://example.com/fotos/tostada-aguacate.jpg",
                Ingredientes =
                [
                    new Ingrediente { Nombre = "Pan", Cantidad = 80, Unidad = UnidadMedida.Gramos },
                    new Ingrediente { Nombre = "Aguacate", Cantidad = 100, Unidad = UnidadMedida.Gramos },
                    new Ingrediente { Nombre = "Sal", Cantidad = 2, Unidad = UnidadMedida.Gramos }
                ],
                Pasos =
                [
                    new Paso { Orden = 1, Descripcion = "Cortar el aguacate por la mitad y retirar el hueso", Fase = FasePaso.Preparacion },
                    new Paso { Orden = 1, Descripcion = "Tostar el pan", Fase = FasePaso.Elaboracion },
                    new Paso { Orden = 2, Descripcion = "Machacar el aguacate y extenderlo sobre el pan", Fase = FasePaso.Elaboracion },
                    new Paso { Orden = 3, Descripcion = "Salpimentar al gusto", Fase = FasePaso.Elaboracion }
                ]
            }
        };

        context.Recetas.AddRange(recetas.Select(r => r.ToEntity()));
        await context.SaveChangesAsync();
    }
}
