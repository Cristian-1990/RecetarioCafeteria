using RecetarioCafeteria.Back.Models;

namespace RecetarioCafeteria.Back.Entity;

public class PasoEntity
{
    public int Id { get; set; }
    public int Orden { get; set; }
    public string Descripcion { get; set; } = string.Empty;
    public FasePaso Fase { get; set; }

    public int RecetaId { get; set; }
    public RecetaEntity Receta { get; set; } = null!;
}
