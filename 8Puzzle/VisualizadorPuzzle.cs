using System;

namespace PUZZLE
{
    public static class VisualizadorPuzzle
    {
        // Método para dibujar el estado del puzzle (matriz 2D) en la consola.
        public static void Dibujar(int[,] tablero)
        {
            int filas = tablero.GetLength(0);
            int columnas = tablero.GetLength(1);
            
            for (int i = 0; i < filas; i++)
            {
                for (int j = 0; j < columnas; j++)
                {
                    // Imprime 0 como un espacio en blanco para mayor claridad.
                    Console.Write(tablero[i, j] == 0 ? "   " : $"{tablero[i, j],3}");
                }
                Console.WriteLine();
            }
            // Línea separadora para delimitar el tablero.
            Console.WriteLine(new string('-', columnas * 3));
        }
    }
}
