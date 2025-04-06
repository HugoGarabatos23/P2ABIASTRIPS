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
    /// Verifica si esta acción es aplicable a un estado dado.
    /// </summary>
    public bool EsAplicable(Dictionary<Predicado, bool> estado)
    {
        // 1. El bloque debe estar en la posición de origen
        bool enPosicion = estado.GetValueOrDefault(new Predicado("on", Bloque, Desde), false);
        
        // 2. El bloque debe estar libre (nada encima)
        bool bloqueLibre = estado.GetValueOrDefault(new Predicado("clear", Bloque), false);
        
        // 3. Si el destino no es mesa, debe estar libre
        bool destinoLibre = (Hacia == "mesa") || 
                          estado.GetValueOrDefault(new Predicado("clear", Hacia), false);
        
        return enPosicion && bloqueLibre && destinoLibre;
    }

    /// <summary>
    /// Aplica los efectos de esta acción a un estado.
    /// </summary>
    public Dictionary<Predicado, bool> AplicarEfectos(Dictionary<Predicado, bool> estado)
    {
        Dictionary<Predicado, bool> nuevoEstado = new Dictionary<Predicado, bool>(estado);
        
        
        // 1. Eliminar el predicado antiguo
        nuevoEstado[new Predicado("on", Bloque, Desde)] = false;
        
         // 2. Añadir nuevo predicado
        nuevoEstado[new Predicado("on", Bloque, Hacia)] = true;

        // 3. Actualizar clears
        nuevoEstado[new Predicado("clear", Desde)] = true;
        
        if (Hacia != "mesa")
        {
            nuevoEstado[new Predicado("clear", Hacia)] = false;
        }
        
        return nuevoEstado;
    }



    /// <summary>
    /// Representación en cadena de la acción en formato legible.
    /// </summary>
    /// <returns>Cadena en formato "Mover(Bloque, Desde, Hacia)".</returns>
    public override string ToString() => $"Mover({Bloque}, {Desde}, {Hacia})";

}

