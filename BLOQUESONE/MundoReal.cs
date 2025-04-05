// Hugo Garabatos Díaz y Pedro Agrelo Márquez

namespace BLOQUESONE;

/// <summary>
/// Representa el mundo físico donde se ejecutan las acciones del problema de bloques.
/// Valida estados y aplica cambios según el plan generado por STRIPS.
/// </summary>
public class MundoReal
{
    public Dictionary<Predicado, bool> Estado { get; private set; } // para solo mutar el mundo desde aquí
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
    /// Ejecuta un plan paso a paso, mostrando el estado antes/después de cada acción.
    /// </summary>
    /// <param name="plan">Lista de acciones (ej: "mover(A,B)").</param>
    /// <param name="pausarEntrePasos">Si true, pausa para visualización interactiva.</param>
    public void EjecutarPlan(List<Accion> plan, bool pausarEntrePasos = true)
    {
        if (plan.Count == 0)
        {
            Console.WriteLine("Plan vacío - No hay acciones a ejecutar");
            return;
        }

        Console.WriteLine("\n=== EJECUCIÓN EN MUNDO REAL ===");
        for (int i = 0; i < plan.Count; i++)
        {
            Console.WriteLine($"\n[Paso {i + 1}/{plan.Count}] Acción: {plan[i]}");
            
            Console.WriteLine("Estado ANTES:");
            MostrarEstado();

            EjecutarAccion(plan[i]);

            Console.WriteLine("\nEstado DESPUÉS:");
            MostrarEstado();

            if (pausarEntrePasos && i < plan.Count - 1)
            {
                Console.WriteLine("\nPresione Enter para continuar...");
                Console.ReadLine();
            }
        }
    }

    /// <summary>
    /// Aplica una acción al estado actual (delega a OperacionesBloques).
    /// </summary>
    public void EjecutarAccion(Accion accion)
    {
        // Usa la versión estática
        Estado = OperacionesBloques.AplicarAccion(Estado, accion);
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