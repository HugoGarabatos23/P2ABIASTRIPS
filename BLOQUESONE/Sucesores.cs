namespace BLOQUESONE;

/// <summary>
/// Representa un estado sucesor en el espacio de búsqueda del planificador,
/// incluyendo la acción que lo generó, el nuevo estado y su costo asociado.
/// </summary>
public class Sucesor
{
    public Accion Accion { get; set; }  // Acción que generó este estado
    public Dictionary<Predicado, bool> Estado { get; set; }  // Nuevo estado resultante
    public int Costo { get; set; } 
}