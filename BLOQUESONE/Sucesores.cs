namespace BLOQUESONE;

public class Sucesor
{
    public Accion Accion { get; set; }  // Acción que generó este estado
    public Dictionary<Predicado, bool> Estado { get; set; }  // Nuevo estado resultante
    public int Costo { get; set; } 
}