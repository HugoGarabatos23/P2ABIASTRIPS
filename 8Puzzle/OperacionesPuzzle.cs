using System;
namespace PUZZLE
{
    public static class OperacionesPuzzle
        {
            // Método que aplica una acción sobre el estado actual del puzzle.
            // Recibe el estado actual (matriz int[,]) y una instancia de AccionPuzzle,
            // y retorna un nuevo estado resultante de mover el espacio en blanco.
            public static int[,] AplicarAccion(int[,] estadoActual, AccionPuzzle accion)
            {
                // Clonar el estado actual para no modificar la matriz original.
                int[,] nuevoEstado = (int[,])estadoActual.Clone();
                int filas = nuevoEstado.GetLength(0);
                int columnas = nuevoEstado.GetLength(1);

                // Buscar la posición del espacio en blanco (representado por 0).
                int filaBlanco = -1, colBlanco = -1;
                for (int i = 0; i < filas; i++)
                {
                    for (int j = 0; j < columnas; j++)
                    {
                        if (nuevoEstado[i, j] == 0)
                        {
                            filaBlanco = i;
                            colBlanco = j;
                            break;
                        }
                    }
                    if (filaBlanco != -1) break;
                }

                // Calcular la nueva posición del espacio en blanco según la dirección indicada.
                int nuevaFila = filaBlanco, nuevaCol = colBlanco;
                switch (accion.Direccion)
                {
                    case "arriba":
                        nuevaFila = filaBlanco - 1;
                        break;
                    case "abajo":
                        nuevaFila = filaBlanco + 1;
                        break;
                    case "izquierda":
                        nuevaCol = colBlanco - 1;
                        break;
                    case "derecha":
                        nuevaCol = colBlanco + 1;
                        break;
                    default:
                        throw new ArgumentException("Dirección no válida en la acción");
                }

                // Verificar que la nueva posición esté dentro de los límites del tablero.
                if (nuevaFila < 0 || nuevaFila >= filas || nuevaCol < 0 || nuevaCol >= columnas)
                    throw new InvalidOperationException("Movimiento no válido: se sale del tablero");

                // Intercambiar la posición del espacio en blanco con el valor de la celda destino.
                int temp = nuevoEstado[nuevaFila, nuevaCol];
                nuevoEstado[nuevaFila, nuevaCol] = 0;
                nuevoEstado[filaBlanco, colBlanco] = temp;

                return nuevoEstado;
            }
        }
}