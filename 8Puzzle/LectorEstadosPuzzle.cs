
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace PUZZLE
{
/// <summary>
/// Clase para leer y parsear un archivo de configuración para el 8-puzzle.
/// Lee dos secciones: [EstadoInicial] y [EstadoObjetivo] y las convierte en matrices (int[,]).
/// </summary>
    public class LectorEstadosPuzzle
    {
        /// <summary>
        /// Estado inicial del puzzle (matriz 3x3).
        /// </summary>
        public int[,] EstadoInicial { get; }

        /// <summary>
        /// Estado objetivo del puzzle (matriz 3x3).
        /// </summary>
        public int[,] EstadoObjetivo { get; }

        /// <summary>
        /// Constructor que lee el archivo ubicado en 'rutaArchivo' y parsea los estados.
        /// </summary>
        /// <param name="rutaArchivo">Ruta del archivo de configuración.</param>
        public LectorEstadosPuzzle(string rutaArchivo)
        {
            try
            {
                string[] lineas = File.ReadAllLines(rutaArchivo)
                                        .Where(l => !string.IsNullOrWhiteSpace(l))
                                        .ToArray();

                // Leer las secciones correspondientes.
                List<string> estadoInicialLineas = LeerSeccion(lineas, "[EstadoInicial]");
                List<string> estadoObjetivoLineas = LeerSeccion(lineas, "[EstadoObjetivo]");

                // Parsear cada sección a una matriz.
                EstadoInicial = ParsearMatriz(estadoInicialLineas);
                EstadoObjetivo = ParsearMatriz(estadoObjetivoLineas);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error al leer el archivo: {ex.Message}");
                throw;
            }
        }

        /// <summary>
        /// Extrae las líneas de una sección específica del archivo.
        /// </summary>
        /// <param name="lineas">Todas las líneas del archivo.</param>
        /// <param name="seccion">Nombre de la sección a buscar (ejemplo: "[EstadoInicial]").</param>
        /// <returns>Lista de líneas correspondientes a la sección.</returns>
        /// <exception cref="InvalidDataException">Si la sección no se encuentra o está vacía.</exception>
        private List<string> LeerSeccion(string[] lineas, string seccion)
        {
            List<string> resultado = new List<string>();
            bool enSeccion = false;

            foreach (string linea in lineas)
            {
                string lineaTrim = linea.Trim();

                if (lineaTrim == seccion)
                {
                    enSeccion = true;
                    continue;
                }

                if (enSeccion)
                {
                    // Si encontramos otra sección, terminamos.
                    if (lineaTrim.StartsWith("["))
                        break;

                    if (!string.IsNullOrWhiteSpace(lineaTrim))
                        resultado.Add(lineaTrim);
                }
            }

            if (resultado.Count == 0)
                throw new InvalidDataException($"Sección '{seccion}' no encontrada o vacía.");

            return resultado;
        }

        /// <summary>
        /// Convierte una lista de líneas en una matriz (int[,]) usando comas o espacios como separador.
        /// </summary>
        /// <param name="lineas">Líneas que representan las filas del estado.</param>
        /// <returns>Matriz que representa el estado.</returns>
        /// <exception cref="InvalidDataException">Si las filas tienen un número de columnas inconsistente.</exception>
        private int[,] ParsearMatriz(List<string> lineas)
        {
            int filas = lineas.Count;
            // Se asume que la primera línea define el número de columnas.
            string[] elementosPrimeraLinea = lineas[0].Split(new char[] { ',', ' ' }, StringSplitOptions.RemoveEmptyEntries);
            int columnas = elementosPrimeraLinea.Length;

            int[,] matriz = new int[filas, columnas];

            for (int i = 0; i < filas; i++)
            {
                string[] elementos = lineas[i].Split(new char[] { ',', ' ' }, StringSplitOptions.RemoveEmptyEntries);
                if (elementos.Length != columnas)
                    throw new InvalidDataException("Número de columnas inconsistente en la matriz.");

                for (int j = 0; j < columnas; j++)
                {
                    matriz[i, j] = int.Parse(elementos[j]);
                }
            }

            return matriz;
        }
    }
}
