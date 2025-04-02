namespace BLOQUESONE;
public class Agente
{
    public string[] Bloques ;
    public string[] Posiciones;
    public List<Accion> PlanActual { get; private set; }
    public int CostoPlan { get; private set; }
    public Dictionary<Predicado, bool> EstadoActual => CopiarEstado(_estadoSimulado);
    private Dictionary<Predicado, bool> _estadoSimulado;

    public Agente(string[] bloques, string[] posiciones, Dictionary<Predicado, bool> estadoInicial)
    {
        
        _estadoSimulado = CopiarEstado(estadoInicial);
        Bloques = bloques;
        Posiciones = posiciones;
        PlanActual = new List<Accion>();
        CostoPlan = 0;
    }

     // Método para generar copias seguras del estado
    private Dictionary<Predicado, bool> CopiarEstado(Dictionary<Predicado, bool> original)
    {
        Dictionary<Predicado, bool> copia = new Dictionary<Predicado, bool>();
        foreach (KeyValuePair<Predicado, bool> kvp in original)
        {
            copia.Add(new Predicado(kvp.Key.Nombre, kvp.Key.Argumentos), kvp.Value);
        }
        return copia;
    }

    public ResultadoBusqueda Planificar(Dictionary<Predicado, bool> estadoObjetivo)
    {
        ResultadoBusqueda resultado = BusquedaAEstrella.EncontrarSolucion(
            _estadoSimulado,
            estadoObjetivo,
            estado => GenerarSucesores(estado)
        );

        PlanActual = resultado.Plan;
        CostoPlan = resultado.CostoTotal;
        return resultado;
    }

    private Dictionary<Predicado, bool> AplicarAccionSimulada(Accion accion)
    {
        return OperacionesBloques.AplicarAccion(estado, accion);
    }
   
    // Versión optimizada y coherente que primero valida precondiciones
    private List<Sucesor> GenerarSucesores(Dictionary<Predicado, bool> estado)
    {
        List<Sucesor> sucesores = new List<Sucesor>();
        
        // 1. Filtrar bloques que pueden moverse (clear=true)
        foreach (string bloque in Bloques)
        {
            if (!estado.GetValueOrDefault(new Predicado("clear", bloque), false))
                continue;

            // 2. Encontrar posición actual del bloque
            string posicionActual = null;
            foreach (string posicion in Posiciones)
            {
                if (estado.GetValueOrDefault(new Predicado("on", bloque, posicion), false))
                {
                    posicionActual = posicion;
                    break;
                }
            }
            if (posicionActual == null) continue;

            // 3. Generar destinos válidos
            foreach (string destino in Posiciones)
            {
                if (destino == posicionActual) continue;
                
                // Verificar precondiciones específicas para este movimiento
                if (EsMovimientoValido(estado, bloque, posicionActual, destino))
                {
                    Accion accion = new Accion(bloque, posicionActual, destino);
                    Dictionary<Predicado, bool> nuevoEstado = AplicarAccionSimulada(estado, accion);
                    sucesores.Add(new Sucesor {
                        Accion = accion,
                        Estado = nuevoEstado,
                        Costo = 1
                    });
                }
            }
        }
        return sucesores;
    }

    // Método específico para validar movimientos
    private bool EsMovimientoValido(Dictionary<Predicado, bool> estado, 
                                  string bloque, string desde, string hacia)
    {
        // 1. El bloque debe estar en la posición de origen
        if (!estado.GetValueOrDefault(new Predicado("on", bloque, desde), false))
            return false;

        // 2. El bloque debe estar libre (nada encima)
        if (!estado.GetValueOrDefault(new Predicado("clear", bloque), false))
            return false;

        // 3. Si el destino no es la mesa, debe estar libre
        if (hacia != "mesa" && !estado.GetValueOrDefault(new Predicado("clear", hacia), false))
            return false;

        return true;
    }

    // Ejecuta el siguiente paso del plan
        public bool EjecutarSiguienteAccion()
        {
            if (PlanActual.Count == 0) return false;

            Accion accion = PlanActual[0];
            PlanActual.RemoveAt(0);
            _estadoSimulado = AplicarAccionSimulada(accion);
            return true;
        }

        // Muestra el plan actual por consola
        public void MostrarPlan()
        {
            Console.WriteLine($"=== PLAN (Costo: {CostoPlan}) ===");
            for (int i = 0; i < PlanActual.Count; i++)
            {
                Console.WriteLine($"{i+1}. {PlanActual[i]}");
            }
        }

        // Muestra el estado actual del mundo
        public void MostrarEstadoActual()
        {
            Console.WriteLine("\n=== ESTADO ACTUAL ===");
            
            // Mostrar torres de bloques
            foreach (string posicion in Posiciones)
            {
                if (posicion == "mesa") continue;
                
                if (_estadoSimulado.TryGetValue(new Predicado("on", posicion, "mesa"), out bool sobreMesa) && sobreMesa)
                {
                    Console.Write($"- Torre: {posicion}");
                    string actual = posicion;
                    while (_estadoSimulado.TryGetValue(new Predicado("on", BuscarBloqueArriba(actual), actual), out bool tieneArriba) && tieneArriba)
                    {
                        string bloqueArriba = BuscarBloqueArriba(actual);
                        Console.Write($" → {bloqueArriba}");
                        actual = bloqueArriba;
                    }
                    Console.WriteLine();
                }
            }

            // Mostrar bloques libres
            Console.WriteLine("\nBloques libres:");
            foreach (string bloque in Bloques)
            {
                if (_estadoSimulado.GetValueOrDefault(new Predicado("clear", bloque), false))
                {
                    Console.WriteLine($"- {bloque}");
                }
            }
        }

        // Métodos auxiliares
        private string BuscarBloqueArriba(string bloqueBase)
        {
            foreach (KeyValuePair<Predicado, bool> kvp in _estadoSimulado)
            {
                if (kvp.Key.Nombre == "on" && kvp.Key.Argumentos[1] == bloqueBase && kvp.Value)
                {
                    return kvp.Key.Argumentos[0];
                }
            }
            return null;
        }

    public void EjecutarPlanConVisualizacion(bool pausarEntrePasos = true)
        {
            if (PlanActual.Count == 0)
            {
                Console.WriteLine("No hay plan para ejecutar");
                return;
            }

            MostrarPlan(); // Muestra el plan completo primero

            for (int i = 0; i < PlanActual.Count; i++)
            {
                Console.WriteLine($"\n=== PASO {i+1}/{PlanActual.Count} ===");
                
                // Mostrar acción actual
                Console.WriteLine($"\nAcción a ejecutar: {PlanActual[i]}");
                
                // Mostrar estado antes
                Console.WriteLine("\nEstado ANTES:");
                MostrarEstadoActual();
                
                // Ejecutar acción
                _estadoSimulado = AplicarAccionSimulada(PlanActual[i]);
                PlanActual.RemoveAt(0);
                
                // Mostrar estado después
                Console.WriteLine("\nEstado DESPUÉS:");
                MostrarEstadoActual();
                
                if (pausarEntrePasos && i < PlanActual.Count)
                {
                    Console.WriteLine("\nPresione Enter para continuar...");
                    Console.ReadLine();
                }
            }
        }

        // Método mejorado para mostrar estado actual (versión unificada)
        public void MostrarEstadoActual()
        {
            Console.WriteLine("\n=== ESTADO ACTUAL ===");
            
            bool hayTorres = false;
            bool hayBloquesLibres = false;

            // Mostrar torres de bloques
            Console.WriteLine("\nTorres de bloques:");
            foreach (string posicion in Posiciones)
            {
                if (posicion == "mesa") continue;
                
                if (_estadoSimulado.TryGetValue(new Predicado("on", posicion, "mesa"), out bool sobreMesa) && sobreMesa)
                {
                    hayTorres = true;
                    Console.Write($"- {posicion}");
                    string actual = posicion;
                    
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

            // Mostrar bloques libres
            Console.WriteLine("\nBloques libres (clear=true):");
            foreach (string bloque in Bloques)
            {
                if (_estadoSimulado.GetValueOrDefault(new Predicado("clear", bloque), false))
                {
                    hayBloquesLibres = true;
                    Console.WriteLine($"- {bloque}");
                }
            }
            if (!hayBloquesLibres) Console.WriteLine("No hay bloques libres");
        }

}