// Hugo Garabatos Díaz y Pedro Agrelo Márquez

namespace BLOQUESONE;

/// <summary>
/// Representa una acción que puede ser ejecutada en el mundo de bloques,
/// siguiendo la estructura STRIPS (Precondiciones -> Efectos).
/// </summary>
public class Accion
{
    /// <summary>
    /// Nombre del bloque que se está moviendo.
    /// Ejemplo: "A".
    /// </summary>
    public string Bloque { get; }
    
    /// <summary>
    /// Ubicación actual del bloque (puede ser "mesa" u otro bloque).
    /// Ejemplo: "mesa" o "B".
    /// </summary>
    public string Desde { get; }
    
    /// <summary>
    /// Destino del movimiento (puede ser "mesa" u otro bloque).
    /// Ejemplo: "C" o "mesa".
    /// </summary>
    public string Hacia { get; }


    /// <summary>
    /// Constructor para crear una nueva acción de movimiento.
    /// </summary>
    /// <param name="bloque">Bloque a mover.</param>
    /// <param name="desde">Ubicación actual del bloque.</param>
    /// <param name="hacia">Nueva ubicación del bloque.</param>    
    public Accion(string bloque, string desde, string hacia)
    {
        Bloque = bloque;
        Desde = desde;
        Hacia = hacia;
    }

    /// <summary>
    /// Representación en cadena de la acción en formato legible.
    /// </summary>
    /// <returns>Cadena en formato "Mover(Bloque, Desde, Hacia)".</returns>
    public override string ToString() => $"Mover({Bloque}, {Desde}, {Hacia})";

}

