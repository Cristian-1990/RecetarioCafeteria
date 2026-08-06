namespace RecetarioCafeteria.Back.Models;

public record Paso
{
    public int Id { get; set; }
    public int Orden { get; set; } //Orden relativo a su propia Fase, no global
    public string Descripcion { get; set; } = string.Empty;
    public FasePaso Fase { get; set; }
    public string? Truco { get; set; }
}
