using RecetarioCafeteria.Back.Entity;
using RecetarioCafeteria.Back.Models;
namespace RecetarioCafeteria.Back.Mappers;

public static class PresentacionMapper
{
    public static PresentacionEntity ToEntity(this Presentacion presentacion)
    {
        return new PresentacionEntity
        {
            Id = presentacion.Id,
            RecetaId = presentacion.RecetaId,
            FotoUrl = presentacion.FotoUrl,
            Pasos = presentacion.Pasos.Select(p => p.ToEntity()).ToList()
        };
    }

    public static Presentacion ToPresentacion(this PresentacionEntity entity)
    {
        return new Presentacion
        {
            Id = entity.Id,
            RecetaId = entity.RecetaId,
            FotoUrl = entity.FotoUrl,
            Pasos = entity.Pasos.Select(p => p.ToPasoPresentacion()).ToList()
        };
    }

    public static PasoPresentacionEntity ToEntity(this PasoPresentacion paso)
    {
        return new PasoPresentacionEntity
        {
            Id = paso.Id,
            Orden = paso.Orden,
            Descripcion = paso.Descripcion,
            Consejo = paso.Consejo
        };
    }

    public static PasoPresentacion ToPasoPresentacion(this PasoPresentacionEntity entity)
    {
        return new PasoPresentacion
        {
            Id = entity.Id,
            Orden = entity.Orden,
            Descripcion = entity.Descripcion,
            Consejo = entity.Consejo
        };
    }
}
