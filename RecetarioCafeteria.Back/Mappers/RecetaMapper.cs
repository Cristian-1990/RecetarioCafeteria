using RecetarioCafeteria.Back.Entity;
using RecetarioCafeteria.Back.Models;
namespace RecetarioCafeteria.Back.Mappers;

public static class RecetaMapper
{
    public static RecetaEntity ToEntity(this Receta receta)
    {
        return new RecetaEntity
        {
            Id = receta.Id,
            Titulo = receta.Titulo,
            Categoria = receta.Categoria,
            TiempoMinutos = receta.TiempoMinutos,
            FotoUrl = receta.FotoUrl,
            Ingredientes = receta.Ingredientes.Select(i => i.ToEntity()).ToList(),
            Pasos = receta.Pasos.Select(p => p.ToEntity()).ToList(),
            Alergenos = receta.Alergenos.ToList(),
            Utensilios = receta.Utensilios.ToList(),
            MiseEnPlace = receta.MiseEnPlace.ToList(),
            NotaFinal = receta.NotaFinal
        };
    }

    public static Receta ToReceta(this RecetaEntity entity)
    {
        return new Receta
        {
            Id = entity.Id,
            Titulo = entity.Titulo,
            Categoria = entity.Categoria,
            TiempoMinutos = entity.TiempoMinutos,
            FotoUrl = entity.FotoUrl,
            Ingredientes = entity.Ingredientes.Select(i => i.ToIngrediente()).ToList(),
            Pasos = entity.Pasos.Select(p => p.ToPaso()).ToList(),
            Alergenos = entity.Alergenos.ToList(),
            Utensilios = entity.Utensilios.ToList(),
            MiseEnPlace = entity.MiseEnPlace.ToList(),
            NotaFinal = entity.NotaFinal
        };
    }

    public static IngredienteEntity ToEntity(this Ingrediente ingrediente)
    {
        return new IngredienteEntity
        {
            Id = ingrediente.Id,
            Nombre = ingrediente.Nombre,
            Cantidad = ingrediente.Cantidad,
            Unidad = ingrediente.Unidad
        };
    }

    public static Ingrediente ToIngrediente(this IngredienteEntity entity)
    {
        return new Ingrediente
        {
            Id = entity.Id,
            Nombre = entity.Nombre,
            Cantidad = entity.Cantidad,
            Unidad = entity.Unidad
        };
    }

    public static PasoEntity ToEntity(this Paso paso)
    {
        return new PasoEntity
        {
            Id = paso.Id,
            Orden = paso.Orden,
            Descripcion = paso.Descripcion,
            Fase = paso.Fase
        };
    }

    public static Paso ToPaso(this PasoEntity entity)
    {
        return new Paso
        {
            Id = entity.Id,
            Orden = entity.Orden,
            Descripcion = entity.Descripcion,
            Fase = entity.Fase
        };
    }
}
