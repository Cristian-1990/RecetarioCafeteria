namespace RecetarioCafeteria.Back.Models;

public record Receta
{
    public int Id { get; set; }
    public string Titulo { get; set; } = string.Empty;
    public CategoriaReceta Categoria { get; set; }
    public int TiempoMinutos { get; set; }
    public string FotoUrl { get; set; } = string.Empty;
    public List<Ingrediente> Ingredientes { get; set; } = [];
    public List<Paso> Pasos { get; set; } = [];
}
