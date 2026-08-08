namespace RecetarioCafeteria.Back.Entity;

public class PasoPresentacionEntity
{
    public int Id { get; set; }
    public int Orden { get; set; }
    public string Descripcion { get; set; } = string.Empty;
    public string? Consejo { get; set; }

    public int PresentacionId { get; set; }
    public PresentacionEntity Presentacion { get; set; } = null!;
}
