namespace BLOQUESONE
{
    public class ResultadoBusqueda
    {
        public List<Accion> Plan { get; }
        public int CostoTotal { get; }

        public ResultadoBusqueda(List<Accion> plan, int costo)
        {
            Plan = plan;
            CostoTotal = costo;
        }
    }

    public class BusquedaAEstrella
    {
        public static ResultadoBusqueda EncontrarSolucion(
            Dictionary<Predicado, bool> estadoInicial,
            Dictionary<Predicado, bool> estadoObjetivo,
            Func<Dictionary<Predicado, bool>, List<Sucesor>> generadorSucesores)
        {
            PriorityQueue<Nodo, int> openSet = new PriorityQueue<Nodo, int>();
            HashSet<string> closedSet = new HashSet<string>();
            Dictionary<string, Nodo> cameFrom = new Dictionary<string, Nodo>();

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
                
                if (EsEstadoObjetivo(current.Estado, estadoObjetivo))
                {
                    return new ResultadoBusqueda(
                        plan: ReconstruirCamino(current),
                        costo: current.G
                    );
                }

                closedSet.Add(GenerarHashEstado(current.Estado));

                List<Sucesor> sucesores = generadorSucesores(current.Estado);
                foreach (Sucesor sucesor in sucesores)
                {
                    string hashSucesor = GenerarHashEstado(sucesor.Estado);
                    if (closedSet.Contains(hashSucesor))
                        continue;

                    int g = current.G + sucesor.Costo;
                    int h = CalcularHeuristica(sucesor.Estado, estadoObjetivo);
                    Nodo nodoSucesor = new Nodo(
                        estado: sucesor.Estado,
                        accion: sucesor.Accion,
                        padre: current,
                        costoAcumulado: g,
                        heuristica: h
                    );

                    if (!cameFrom.ContainsKey(hashSucesor) || g < cameFrom[hashSucesor].G)
                    {
                        cameFrom[hashSucesor] = nodoSucesor;
                        openSet.Enqueue(nodoSucesor, nodoSucesor.F);
                    }
                }
            }

            throw new InvalidOperationException("No se encontró solución");
        }

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

    public class Nodo
    {
        public Dictionary<Predicado, bool> Estado { get; }
        public Accion Accion { get; }
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
}