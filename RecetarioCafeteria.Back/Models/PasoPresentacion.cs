namespace RecetarioCafeteria.Back.Models;

public record PasoPresentacion
{
    public int Id { get; set; }
    public int Orden { get; set; }
    public string Descripcion { get; set; } = string.Empty;
    public string? Consejo { get; set; }
}
