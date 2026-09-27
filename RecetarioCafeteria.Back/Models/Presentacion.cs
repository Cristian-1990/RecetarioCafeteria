namespace RecetarioCafeteria.Back.Models;

public record Presentacion
{
    public int Id { get; set; }
    public int RecetaId { get; set; }
    public string FotoUrl { get; set; } = string.Empty;
    public List<PasoPresentacion> Pasos { get; set; } = [];
}
