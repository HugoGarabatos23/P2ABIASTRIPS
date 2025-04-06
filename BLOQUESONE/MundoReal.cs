// Hugo Garabatos Díaz y Pedro Agrelo Márquez

namespace BLOQUESONE;

/// <summary>
/// Representa el mundo físico donde se ejecutan las acciones del problema de bloques.
/// Responsabilidades clave:
/// 1. Mantener el estado actual (single source of truth)
/// 2. Validar y aplicar acciones físicas
/// 3. Mostrar el estado de forma legible
/// </summary>
public class MundoReal
{
    /// <summary>
    /// Estado actual del mundo. Privado para garantizar modificaciones controladas.
    /// </summary>
    public Dictionary<Predicado, bool> Estado { get; set; } // para solo mutar el mundo desde aquí
    
    /// <summary>
    /// Nombres de todos los bloques existentes en el mundo.
    /// </summary>
    public string[] Bloques { get; }

    
    /// <summary>
    /// Constructor: Inicializa el mundo con bloques y un estado inicial válido.
    /// </summary>
    /// <param name="bloques">Array con los nombres de los bloques (ej: ["A", "B"]).</param>
    /// <param name="estadoInicial">Predicados iniciales (ej: "on(A,mesa)"=true).</param>
    /// <exception cref="ArgumentException">Si el estado inicial es físicamente inválido.</exception>
    public MundoReal(string[] bloques, Dictionary<Predicado, bool> estadoInicial)
    {
        Estado = new Dictionary<Predicado, bool>(estadoInicial);
        Bloques = bloques;
        ValidarEstadoInicial();
    }

    /// <summary>
    /// Muestra el estado actual detallado (torres y bloques libres).
    /// </summary>
    public void MostrarEstado()
    {
        bool hayTorres = false;
        Console.WriteLine("\nTorres de bloques:");
        foreach (string bloque in Bloques)
        {
            if (Estado.GetValueOrDefault(new Predicado("on",bloque,"mesa"), false))
            {
                hayTorres = true;
                Console.Write($"- Torre {bloque}");
                string actual = bloque;
                
                string bloqueArriba;
                while ((bloqueArriba = BuscarBloqueArriba(actual)) != null)
                {
                    Console.Write($" → {bloqueArriba}");
                    actual = bloqueArriba;
                }
                Console.WriteLine();
            }
        }
        if (!hayTorres) Console.WriteLine("No hay torres (todos los bloques están directamente sobre la mesa)");

        Console.WriteLine("\nBloques libres (clear=true):");
        bool hayLibres = false;
        foreach (string bloque in Bloques)
        {
            if (Estado.GetValueOrDefault(new Predicado("clear", bloque), false))
                {
                    hayLibres = true;
                    Console.WriteLine($"- {bloque}");
                }
            }
            if (!hayLibres) Console.WriteLine("No hay bloques libres");
        }

    /// <summary>
    /// Valida que todos los bloques "on" tengan soporte hasta la mesa.
    /// </summary>
     private void ValidarEstadoInicial()
    {
        foreach (KeyValuePair<Predicado, bool> kvp in Estado)
        {
            if (kvp.Key.Nombre == "on" && kvp.Value)
            {
                string bloqueSuperior = kvp.Key.Argumentos[0];
                string baseActual = kvp.Key.Argumentos[1];
                if (baseActual != "mesa")
                {
                    //Verifica que la base (otro bloque) esté soportada directa o indirectamente por la mesa
    
                    if (!EstadoSobreMesa(baseActual))
                    {
                        throw new ArgumentException($"El bloque {bloqueSuperior} está sobre {baseActual}, que no tiene soporte válido hasta la mesa");
                    }
                }
            }
        }
    }

    /// <summary>
    /// Verifica recursivamente si un bloque está directa/indirectamente sobre la mesa.
    /// </summary>
    private bool EstadoSobreMesa(string bloque)
    {
        if (bloque == "mesa") return true; 

        // Busca que bloque o mesa esten debajo
        KeyValuePair<Predicado, bool> soporte = Estado.FirstOrDefault(
            kvp  => kvp.Key.Nombre == "on" && 
               kvp.Key.Argumentos[0] == bloque && 
               kvp.Value);
        
        if (soporte.Key == null) return false ; // No tiene soporte 

        string baseDelBloque = soporte.Key.Argumentos[1];
        return EstadoSobreMesa(baseDelBloque); // LLamada recursiva 

    }

    /// <summary>
    /// Busca el bloque inmediatamente superior a otro (null si no existe).
    /// </summary>
        private string BuscarBloqueArriba(string bloqueBase)
    {
        foreach (KeyValuePair<Predicado, bool> kvp in Estado)
        {
            if (kvp.Key.Nombre == "on" && 
                kvp.Key.Argumentos[1] == bloqueBase && 
                kvp.Value)
            {
                return kvp.Key.Argumentos[0];
            }
        }
        return null;
    }
}