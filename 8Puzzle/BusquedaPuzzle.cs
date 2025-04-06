using System;
using System.Collections.Generic;
using System.Linq;

namespace PUZZLE
{
    /// <summary>
    /// Representa el resultado de una búsqueda en el 8-puzzle, con el plan de acciones y el costo total.
    /// </summary>
    public class ResultadoBusquedaPuzzle
    {
        public List<AccionPuzzle> Plan { get; }
        public int CostoTotal { get; }

        public ResultadoBusquedaPuzzle(List<AccionPuzzle> plan, int costo)
        {
            Plan = plan;
            CostoTotal = costo;
        }
    }

    /// <summary>
    /// Implementación del algoritmo A* para resolver el 8-puzzle.
    /// </summary>
    public class BusquedaPuzzle
    {
        /// <summary>
        /// Clase interna que representa un nodo en el espacio de búsqueda.
        /// </summary>
        public class NodoPuzzle
        {
            public int[,] Tablero { get; }
            public List<AccionPuzzle> Plan { get; }
            public int G { get; } // Costo acumulado
            public int H { get; } // Heurística
            public int F => G + H; // Función de evaluación

            public NodoPuzzle(int[,] tablero, List<AccionPuzzle> plan, int g, int h)
            {
                Tablero = tablero;
                Plan = plan;
                G = g;
                H = h;
            }
        }

        /// <summary>
        /// Encuentra una solución óptima usando A*.
        /// </summary>
        /// <param name="estadoInicial">Estado inicial del puzzle (matriz int[,]).</param>
        /// <param name="estadoObjetivo">Estado objetivo del puzzle (matriz int[,]).</param>
        /// <param name="generadorSucesores">Función que, dado un estado, genera los sucesores (nuevos estados y la acción aplicada).</param>
        /// <returns>Resultado con el plan de acciones y el costo total.</returns>
        /// <exception cref="InvalidOperationException">Si no se encuentra solución.</exception>
        public static ResultadoBusquedaPuzzle EncontrarSolucion(
            int[,] estadoInicial,
            int[,] estadoObjetivo,
            Func<int[,], List<SucesorPuzzle>> generadorSucesores)
        {
            // Usamos la PriorityQueue disponible en .NET 6 (si usas una versión anterior, puedes usar otra estructura).
            PriorityQueue<NodoPuzzle, int> openSet = new PriorityQueue<NodoPuzzle, int>();
            HashSet<string> closedSet = new HashSet<string>();

            NodoPuzzle nodoInicial = new NodoPuzzle(
                estadoInicial,
                new List<AccionPuzzle>(),
                0,
                CalcularHeuristica(estadoInicial, estadoObjetivo)
            );
            openSet.Enqueue(nodoInicial, nodoInicial.F);

            while (openSet.Count > 0)
            {
                NodoPuzzle current = openSet.Dequeue();

                if (TablerosIguales(current.Tablero, estadoObjetivo))
                {
                    return new ResultadoBusquedaPuzzle(current.Plan, current.G);
                }

                closedSet.Add(TableroToString(current.Tablero));

                List<SucesorPuzzle> sucesores = generadorSucesores(current.Tablero);
                foreach (var suc in sucesores)
                {
                    string hash = TableroToString(suc.Estado);
                    if (closedSet.Contains(hash))
                        continue;

                    int g = current.G + 1;
                    int h = CalcularHeuristica(suc.Estado, estadoObjetivo);
                    NodoPuzzle nodoSucesor = new NodoPuzzle(
                        suc.Estado,
                        new List<AccionPuzzle>(current.Plan) { suc.Accion },
                        g,
                        h
                    );

                    openSet.Enqueue(nodoSucesor, nodoSucesor.F);
                }
            }

            throw new InvalidOperationException("No se encontró solución");
        }

        /// <summary>
        /// Heurística: número de piezas fuera de lugar (excluyendo el espacio 0).
        /// </summary>
        public static int CalcularHeuristica(int[,] estado, int[,] objetivo)
        {
            int filas = estado.GetLength(0);
            int columnas = estado.GetLength(1);
            int h = 0;
            for (int i = 0; i < filas; i++)
            {
                for (int j = 0; j < columnas; j++)
                {
                    if (estado[i, j] != 0 && estado[i, j] != objetivo[i, j])
                        h++;
                }
            }
            return h;
        }

        /// <summary>
        /// Compara dos tableros para ver si son iguales.
        /// </summary>
        public static bool TablerosIguales(int[,] a, int[,] b)
        {
            int filas = a.GetLength(0);
            int columnas = a.GetLength(1);
            if (filas != b.GetLength(0) || columnas != b.GetLength(1))
                return false;
            for (int i = 0; i < filas; i++)
            {
                for (int j = 0; j < columnas; j++)
                {
                    if (a[i, j] != b[i, j])
                        return false;
                }
            }
            return true;
        }

        /// <summary>
        /// Convierte un tablero a una cadena única para identificar estados ya visitados.
        /// </summary>
        public static string TableroToString(int[,] tablero)
        {
            int filas = tablero.GetLength(0);
            int columnas = tablero.GetLength(1);
            List<string> elems = new List<string>();
            for (int i = 0; i < filas; i++)
            {
                for (int j = 0; j < columnas; j++)
                {
                    elems.Add(tablero[i, j].ToString());
                }
            }
            return string.Join(",", elems);
        }
    }
}
