using System;

namespace BLOQUESONE

public class MundoPuzzle
{
    // Propiedad que guarda el estado actual del puzzle en forma de matriz bidimensional.
    // Cada celda contiene un número; 0 representa el espacio en blanco.
    public int[,] Tablero { get; private set; }

    // Constructor: recibe el estado inicial (una matriz) y lo clona para trabajar sin modificar el original.
    public MundoPuzzle(int[,] estadoInicial)
    {
        Tablero = (int[,])estadoInicial.Clone();
    }

    // Método para mostrar el estado actual del puzzle en consola, en forma de rejilla.
    public void MostrarEstado()
    {
        int filas = Tablero.GetLength(0);
        int columnas = Tablero.GetLength(1);
        for (int i = 0; i < filas; i++)
        {
            for (int j = 0; j < columnas; j++)
            {
                // Se imprime 0 como un espacio en blanco para mayor claridad.
                Console.Write(Tablero[i, j] == 0 ? "   " : $"{Tablero[i, j],3}");
            }
            Console.WriteLine();
        }
        Console.WriteLine();
    }

    // Método que verifica si el estado actual es el estado objetivo.
    // Por ejemplo, para un 8-puzzle, se espera que los números estén en orden ascendente
    // y que el espacio en blanco (0) se encuentre en la última posición.
    public bool EsEstadoObjetivo()
    {
        int filas = Tablero.GetLength(0);
        int columnas = Tablero.GetLength(1);
        int valorEsperado = 1;
        for (int i = 0; i < filas; i++)
        {
            for (int j = 0; j < columnas; j++)
            {
                // La última celda debe ser 0 (espacio en blanco)
                if (i == filas - 1 && j == columnas - 1)
                {
                    if (Tablero[i, j] != 0)
                        return false;
                }
                else
                {
                    if (Tablero[i, j] != valorEsperado)
                        return false;
                    valorEsperado++;
                }
            }
        }
        return true;
    }

    // Método que aplica una acción al estado actual y devuelve un nuevo estado (sin modificar el actual).
    // Recibe una instancia de AccionPuzzle que indica la dirección en la que se moverá el espacio en blanco.
    public int[,] AplicarAccion(AccionPuzzle accion)
    {
        int filas = Tablero.GetLength(0);
        int columnas = Tablero.GetLength(1);
        
        // Se clona el estado actual para no modificar la instancia original.
        int[,] nuevoTablero = (int[,])Tablero.Clone();

        // Buscar la posición del espacio en blanco (valor 0) en el tablero.
        int filaBlanco = -1, colBlanco = -1;
        for (int i = 0; i < filas; i++)
        {
            for (int j = 0; j < columnas; j++)
            {
                if (nuevoTablero[i, j] == 0)
                {
                    filaBlanco = i;
                    colBlanco = j;
                    break;
                }
            }
            if (filaBlanco != -1) break;
        }

        // Determinar la nueva posición del espacio en blanco en función de la dirección indicada en la acción.
        int nuevaFila = filaBlanco, nuevaCol = colBlanco;
        switch (accion.Direccion.ToLower())
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
                throw new ArgumentException("Dirección no válida");
        }

        // Verificar que la nueva posición esté dentro de los límites del tablero.
        if (nuevaFila >= 0 && nuevaFila < filas && nuevaCol >= 0 && nuevaCol < columnas)
        {
            // Intercambiar la posición del espacio en blanco con el número que se mueve.
            int temp = nuevoTablero[nuevaFila, nuevaCol];
            nuevoTablero[nuevaFila, nuevaCol] = 0;
            nuevoTablero[filaBlanco, colBlanco] = temp;
        }
        else
        {
            // Si el movimiento no es válido (se sale del tablero), se lanza una excepción.
            throw new InvalidOperationException("Movimiento no válido");
        }

        return nuevoTablero;
    }
}

