
namespace BLOQUESONE;

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
}

// Uso: