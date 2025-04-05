// Hugo Garabatos Díaz y Pedro Agrelo Márquez

namespace BLOQUESONE
{
    /// <summary>
    /// Representa el resultado de una búsqueda de planificación, conteniendo
    /// el plan de acciones y el costo total para alcanzar el objetivo.
    /// </summary>
    public class ResultadoBusqueda
    {
        /// <summary>
        /// Secuencia de acciones que llevan del estado inicial al objetivo.
        /// </summary>    
        public List<Accion> Plan { get; }
        
        /// <summary>
        /// Costo total acumulado del plan (suma de costos individuales).
        /// </summary>
        public int CostoTotal { get; }

        /// <summary>
        /// Crea un nuevo resultado de búsqueda.
        /// </summary>
        /// <param name="plan">Lista ordenada de acciones.</param>
        /// <param name="costo">Costo total del plan.</param>
        public ResultadoBusqueda(List<Accion> plan, int costo)
        {
            Plan = plan;
            CostoTotal = costo;
        }
    }

    /// <summary>
    /// Implementación del algoritmo A* para resolver problemas de planificación STRIPS.
    /// </summary>
    public class BusquedaAEstrella
    {
        /// <summary>
        /// Encuentra una solución óptima usando el algoritmo A*.
        /// </summary>
        /// <param name="estadoInicial">Estado inicial del mundo.</param>
        /// <param name="estadoObjetivo">Predicados que deben ser verdaderos al final.</param>
        /// <param name="generadorSucesores">Función que genera estados sucesores válidos.</param>
        /// <returns>Resultado con el plan y costo total.</returns>
        /// <exception cref="InvalidOperationException">Si no se encuentra solución.</exception>
        public static ResultadoBusqueda EncontrarSolucion(
            Dictionary<Predicado, bool> estadoInicial,
            Dictionary<Predicado, bool> estadoObjetivo,
            Func<Dictionary<Predicado, bool>, List<Agente.Sucesor>> generadorSucesores)
        {
            // Estructuras para la búsqueda:
            PriorityQueue<Nodo, int> openSet = new PriorityQueue<Nodo, int>();
            HashSet<string> closedSet = new HashSet<string>();
            Dictionary<string, Nodo> cameFrom = new Dictionary<string, Nodo>();

            // Inicializar el nodo inicial
            Nodo nodoInicial = new Nodo(
                estado: estadoInicial,
                accion: null,
                padre: null,
                costoAcumulado: 0,
                heuristica: CalcularHeuristica(estadoInicial, estadoObjetivo)
            );

            openSet.Enqueue(nodoInicial, nodoInicial.F);

            while (openSet.Count > 0)
            {
                Nodo current = openSet.Dequeue();

                // Verificar si alcanzamos el objetivo
                if (EsEstadoObjetivo(current.Estado, estadoObjetivo))
                {
                    return new ResultadoBusqueda(
                        plan: ReconstruirCamino(current),
                        costo: current.G
                    );
                }

                closedSet.Add(GenerarHashEstado(current.Estado));


                // Generar y procesar sucesores
                List<Agente.Sucesor> sucesores = generadorSucesores(current.Estado);
                foreach (Agente.Sucesor sucesor in sucesores)
                {
                    string hashSucesor = GenerarHashEstado(sucesor.Estado);
                    // Saltar si ya fue explorado
                    if (closedSet.Contains(hashSucesor))
                        continue;

                    // Calcular valores para el nuevo nodo
                    int g = current.G + sucesor.Costo;
                    int h = CalcularHeuristica(sucesor.Estado, estadoObjetivo);
                    Nodo nodoSucesor = new Nodo(
                        estado: sucesor.Estado,
                        accion: sucesor.Accion,
                        padre: current,
                        costoAcumulado: g,
                        heuristica: h
                    );

                    // Actualizar si encontramos un mejor camino
                    if (!cameFrom.ContainsKey(hashSucesor) || g < cameFrom[hashSucesor].G)
                    {
                        cameFrom[hashSucesor] = nodoSucesor;
                        openSet.Enqueue(nodoSucesor, nodoSucesor.F);
                    }
                }
            }

            throw new InvalidOperationException("No se encontró solución");
        }

        
    /// <summary>
    /// Representa un nodo en el espacio de búsqueda del algoritmo A*.
    /// </summary>
    public class Nodo
    {
        /// <summary>
        /// Estado del mundo en este nodo.
        /// </summary>
        public Dictionary<Predicado, bool> Estado { get; }

        /// <summary>
        /// Acción que llevó a este estado (null para el nodo inicial).
        /// </summary>
        public Accion Accion { get; }

        /// <summary>
        /// Nodo padre en el árbol de búsqueda.
        /// </summary>
        public Nodo Padre { get; }
        
        public int G { get; } // Costo acumulado
        public int H { get; } // Heurística
        public int F => G + H; // Función de evaluación

        public Nodo(
            Dictionary<Predicado, bool> estado,
            Accion accion,
            Nodo padre,
            int costoAcumulado,
            int heuristica)
        {
            Estado = estado;
            Accion = accion;
            Padre = padre;
            G = costoAcumulado;
            H = heuristica;
        }
    }

        /// <summary>
        /// Calcula la heurística (número de predicados objetivo no satisfechos).
        /// </summary>
        private static int CalcularHeuristica(Dictionary<Predicado, bool> estado, Dictionary<Predicado, bool> objetivo)
        {
            int distancia = 0;
            foreach (KeyValuePair<Predicado, bool> pred in objetivo)
            {
                if (pred.Value && (!estado.ContainsKey(pred.Key) || !estado[pred.Key]))
                {
                    distancia++;
                }
            }
            return distancia;
        }

        /// <summary>
        /// Verifica si un estado cumple todos los predicados objetivo.
        /// </summary>
        private static bool EsEstadoObjetivo(Dictionary<Predicado, bool> estado, Dictionary<Predicado, bool> objetivo)
        {
            foreach (KeyValuePair<Predicado, bool> pred in objetivo)
            {
                if (pred.Value)
                {
                    if (!estado.TryGetValue(pred.Key, out bool valor) || !valor)
                    {
                        return false;
                    }
                }
            }
            return true;
        }

        /// <summary>
        /// Reconstruye el plan desde el nodo final hasta el inicial.
        /// </summary>
        private static List<Accion> ReconstruirCamino(Nodo nodoFinal)
        {
            List<Accion> camino = new List<Accion>();
            Nodo current = nodoFinal;
            
            while (current.Accion != null)
            {
                camino.Insert(0, current.Accion);
                current = current.Padre;
            }
            
            return camino;
        }

        /// <summary>
        /// Genera un identificador único para un estado (para evitar duplicados).
        /// </summary>
        private static string GenerarHashEstado(Dictionary<Predicado, bool> estado)
        {
            List<string> predicados = new List<string>();
            foreach (KeyValuePair<Predicado, bool> kvp in estado)
            {
                if (kvp.Value)
                {
                    predicados.Add(kvp.Key.ToString());
                }
            }
            predicados.Sort();
            return string.Join(";", predicados);
        }
    }
}