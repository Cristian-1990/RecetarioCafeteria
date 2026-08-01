using RecetarioCafeteria.Back.Models;

namespace RecetarioCafeteria.Back.Entity;

public class RecetaEntity
{
    public int Id { get; set; }
    public string Titulo { get; set; } = string.Empty;
    public CategoriaReceta Categoria { get; set; }
    public int TiempoMinutos { get; set; }
    public string FotoUrl { get; set; } = string.Empty;
    public List<IngredienteEntity> Ingredientes { get; set; } = [];
    public List<PasoEntity> Pasos { get; set; } = [];
    public List<Alergeno> Alergenos { get; set; } = [];
    public List<string> Utensilios { get; set; } = [];
    public List<string> MiseEnPlace { get; set; } = [];
    public string? NotaFinal { get; set; }
}
