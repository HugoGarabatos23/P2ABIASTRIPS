namespace BLOQUESONE;

public class MundoReal
{
    public Dictionary<Predicado, bool> Estado { get; private set; } // para solo mutar el mundo desde aquí
    public string[] Bloques { get; }
    

    public MundoReal(string[] bloques, Dictionary<Predicado, bool> estadoInicial)
    {
        Estado = new Dictionary<Predicado, bool>(estadoInicial);
        Bloques = bloques;
        ValidarEstadoInicial();
    }

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
            MostrarEstadoDetallado();

            EjecutarAccion(plan[i]);

            Console.WriteLine("\nEstado DESPUÉS:");
            MostrarEstadoDetallado();

            if (pausarEntrePasos && i < plan.Count - 1)
            {
                Console.WriteLine("\nPresione Enter para continuar...");
                Console.ReadLine();
            }
        }
    }

    public void EjecutarAccion(Accion accion)
    {
        // Usa la versión estática
        Estado = OperacionesBloques.AplicarAccion(Estado, accion);
    }

     // Versión detallada (similar a tu implementación mejorada)
    public void MostrarEstadoDetallado()
    {
        //Console.WriteLine("\n--- ESTADO ACTUAL ---");
        
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

    public void MostrarEstadoSimple()
    {
        Console.WriteLine("Estado actual:");
        foreach (KeyValuePair<Predicado, bool> kvp in Estado)
        {
            Console.WriteLine($"- {kvp.Key}");
        }
    }
}