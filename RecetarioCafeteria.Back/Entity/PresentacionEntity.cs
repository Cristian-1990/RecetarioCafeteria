namespace RecetarioCafeteria.Back.Entity;

public class PresentacionEntity
{
    public int Id { get; set; }
    public string FotoUrl { get; set; } = string.Empty;
    public List<PasoPresentacionEntity> Pasos { get; set; } = [];

    public int RecetaId { get; set; }
    public RecetaEntity Receta { get; set; } = null!;
}
