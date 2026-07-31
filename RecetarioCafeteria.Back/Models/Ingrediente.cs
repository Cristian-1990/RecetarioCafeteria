namespace RecetarioCafeteria.Back.Models;

public record Ingrediente
{
    public int Id { get; set; }
    public string Nombre { get; set; } = string.Empty;
    public decimal Cantidad { get; set; }
    public UnidadMedida Unidad { get; set; }
}
