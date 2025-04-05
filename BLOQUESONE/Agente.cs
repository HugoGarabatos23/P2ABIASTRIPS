// Hugo Garabatos Díaz y Pedro Agrelo Márquez

namespace BLOQUESONE;

/// <summary>
/// Clase que representa al agente planificador, capaz de simular estados y generar planes
/// para alcanzar un objetivo utilizando el algoritmo A*.
/// </summary>
public class Agente
{   
    private readonly MundoReal _mundo;
    public Dictionary<Predicado, bool> _estadoSimulado;

    // --- Constructor ---
    /// <summary>
    /// Inicializa el agente con referencia al mundo real.
    /// </summary>
    /// <param name="estadoInicial">Estado inicial del mundo (se copia para evitar modificaciones externas).</param>
    public Agente(MundoReal mundo)
    {
        _mundo = mundo;
        _estadoSimulado = CopiarEstado(mundo.Estado);
    }

        /// <summary>
    /// Genera y ejecuta un plan completo, mostrando cada paso por pantalla.
    /// </summary>
    public void GenerarYEjecutarPlan(Dictionary<Predicado, bool> estadoObjetivo, string[] bloques)
    {
        // 1. Mostrar estado inicial
        Console.WriteLine("\n=== ESTADO INICIAL ===");
        _mundo.MostrarEstado();

        // 2. Generar plan (Simulación)
        ResultadoBusqueda resultado = Planificar(estadoObjetivo, bloques);
        
        // 3. Mostrar plan completo
        Console.WriteLine("\n=== PLAN GENERADO ===");
        for (int i = 0; i < resultado.Plan.Count; i++)
        {
            Console.WriteLine($"{i + 1}. {resultado.Plan[i]}");
        }

         // 4. Ejecutar paso a paso
        Console.WriteLine("\n=== EJECUCIÓN en el Mundo Real ===");
        for (int i = 0; i < resultado.Plan.Count; i++) 
        {
            Accion accion = resultado.Plan[i];
            Console.WriteLine($"\n[Paso {i + 1}/{resultado.Plan.Count}] Acción: {accion}");
            Console.WriteLine( i == 0 ? "\nEstado INICIAL:" : "\nEstado ANTES:");
            _mundo.MostrarEstado();
            _mundo.EjecutarAccion(accion);
            Console.WriteLine("\nEstado DESPUÉS:");
            _mundo.MostrarEstado();

            // Opcional: Pausa entre acciones
            if (i < resultado.Plan.Count - 1) {
                Console.WriteLine("Presione Enter para continuar...");
                Console.ReadLine();
            }
        }
    }
    /// <summary>
    /// Clase interna que representa un estado sucesor en el espacio de búsqueda.
    /// Solo el Agente la utiliza para generar y evaluar planes.
    /// </summary>
    public class Sucesor
    {
        /// <summary>
        /// Acción que generó este estado (ej: "Mover(A, mesa, B)").
        /// </summary>
        public Accion Accion { get; set; }

        /// <summary>
        /// Estado resultante después de aplicar la acción.
        /// </summary>
        public Dictionary<Predicado, bool> Estado { get; set; }

        /// <summary>
        /// Costo acumulado para alcanzar este estado (en STRIPS, normalmente 1 por acción).
        /// </summary>
        public int Costo { get; set; }

        public override string ToString() => 
            $"Acción: {Accion}, Costo: {Costo}";
    }

    /// <summary>
    /// Genera un plan para alcanzar el estado objetivo utilizando búsqueda A*.
    /// </summary>
    /// <param name="estadoObjetivo">Predicados que deben ser verdaderos al final.</param>
    /// <param name="bloques">Lista de bloques disponibles en el mundo.</param>
    /// <returns>Resultado de la búsqueda (éxito/fracaso + plan).</returns>
    public ResultadoBusqueda Planificar(Dictionary<Predicado, bool> estadoObjetivo,
    string[]bloques)
    {
       return BusquedaAEstrella.EncontrarSolucion(_estadoSimulado, estadoObjetivo,
        estado=> GenerarSucesores(estado,bloques)
        );
    }

    /// <summary>
    /// Crea una copia profunda del estado para evitar aliasing.
    /// </summary>
    private Dictionary<Predicado, bool> CopiarEstado(Dictionary<Predicado, bool> original)
    {
        Dictionary<Predicado, bool> copia = new Dictionary<Predicado, bool>();
        foreach (KeyValuePair<Predicado, bool> kvp in original)
        {
            copia.Add(new Predicado(kvp.Key.Nombre, kvp.Key.Argumentos), kvp.Value);
        }
        return copia;
    }

    /// <summary>
    /// Aplica una acción a un estado simulado (sin modificar el estado real).
    /// </summary>
    private Dictionary<Predicado, bool> AplicarAccionSimulada(Dictionary<Predicado,bool>estado, Accion accion)
    {
        return OperacionesBloques.AplicarAccion(estado, accion);
    }

    /// <summary>
    /// Genera todos los estados sucesores válidos a partir de un estado dado.
    /// </summary>
    /// <param name="estado">Estado actual de simulación.</param>
    /// <param name="bloques">Bloques disponibles para mover.</param>
    /// <returns>Lista de sucesores con sus acciones y costos asociados.</returns>   
    private List<Sucesor> GenerarSucesores(Dictionary<Predicado, bool> estado, string[]bloques)
    {
        List<Sucesor> sucesores = new List<Sucesor>();
        
        // 1. Filtrar bloques que pueden moverse (clear=true)
        foreach (string bloque in bloques)
        {
            if (!estado.GetValueOrDefault(new Predicado("clear", bloque), false))
                continue;

            // 2. Encontrar base actual del bloque (puede ser mesa o otro bloque)
            string baseActual = null;
            foreach (string posibleBase in bloques.Concat(new[] {"mesa"}))
            {
                if (estado.GetValueOrDefault(new Predicado("on", bloque, posibleBase), false))
                {
                    baseActual = posibleBase;
                    break;
                }
            }
            if (baseActual == null) continue;

            // 3. Generar destinos válidos
            foreach (string destino in bloques.Concat(new[] { "mesa" }))
            {
                if (destino == baseActual) continue;
                
                // Verificar precondiciones específicas para este movimiento
                if (EsMovimientoValido(estado, bloque, baseActual, destino))
                {
                    Accion accion = new Accion(bloque, baseActual, destino);
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

    /// <summary>
    /// Valida si un movimiento cumple las precondiciones STRIPS.
    /// </summary>
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
        // Muestra el estado actual del mundo
}