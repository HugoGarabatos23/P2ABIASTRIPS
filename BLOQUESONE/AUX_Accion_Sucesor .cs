namespace BLOQUESONE;

public class Sucesor
{
    public Accion Accion { get; set; }  // Acción que generó este estado
    public Dictionary<Predicado, bool> Estado { get; set; }  // Nuevo estado resultante
    public int Costo { get; set; } 
}


public class Accion
{
    public string Bloque { get; }
    public string Desde { get; }
    public string Hacia { get; }
    
    public Accion(string bloque, string desde, string hacia)
    {
        Bloque = bloque;
        Desde = desde;
        Hacia = hacia;
    }
    public override string ToString() => $"Mover({Bloque}, {Desde}, {Hacia})";

}

// Uso: