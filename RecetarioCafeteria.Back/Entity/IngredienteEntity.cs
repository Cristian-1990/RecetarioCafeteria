using RecetarioCafeteria.Back.Models;

namespace RecetarioCafeteria.Back.Entity;

public class IngredienteEntity
{
    public int Id { get; set; }
    public string Nombre { get; set; } = string.Empty;
    public decimal Cantidad { get; set; }
    public UnidadMedida Unidad { get; set; }

    public int RecetaId { get; set; }
    public RecetaEntity Receta { get; set; } = null!;
}
