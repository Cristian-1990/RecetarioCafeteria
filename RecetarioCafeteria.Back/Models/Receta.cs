namespace RecetarioCafeteria.Back.Models;

public record Receta
{
    public int Id { get; set; }
    public string Titulo { get; set; } = string.Empty;
    public CategoriaReceta Categoria { get; set; }
    public int TiempoMinutos { get; set; }
    public int Raciones { get; set; } = 4;
    public string FotoUrl { get; set; } = string.Empty;
    public List<Ingrediente> Ingredientes { get; set; } = [];
    public List<Paso> Pasos { get; set; } = [];
    public List<Alergeno> Alergenos { get; set; } = [];
    public List<string> Utensilios { get; set; } = [];
    public List<string> MiseEnPlace { get; set; } = [];
    public string? NotaFinal { get; set; }
}
