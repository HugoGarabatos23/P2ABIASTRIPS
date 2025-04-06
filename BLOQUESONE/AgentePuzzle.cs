using System;
using System.Collections.Generic;

namespace BLOQUESONE

public class AgentePuzzle
{
    // Estado actual representado como una matriz 3x3, donde 0 es el espacio en blanco.
    private int[,] estadoActual;

    public AgentePuzzle(int[,] estadoInicial)
    {
        // Clonamos el estado inicial para trabajar sin modificar el original.
        estadoActual = (int[,])estadoInicial.Clone();
    }

    // Genera todos los sucesores (nuevos estados) a partir del estado actual.
    public List<SucesorPuzzle> GenerarSucesores()
    {
        List<SucesorPuzzle> sucesores = new List<SucesorPuzzle>();

        // Encontrar la posición del espacio en blanco (0).
        int filaBlanco = -1, colBlanco = -1;
        for (int i = 0; i < 3; i++)
        {
            for (int j = 0; j < 3; j++)
            {
                if (estadoActual[i, j] == 0)
                {
                    filaBlanco = i;
                    colBlanco = j;
                    break;
                }
            }
            if (filaBlanco != -1) break;
        }

        // Definir los posibles movimientos para el espacio en blanco:
        // Cada movimiento se representa con un desplazamiento (dFila, dColumna).
        var movimientos = new Dictionary<string, (int dFila, int dColumna)>
        {
            { "arriba",    (-1, 0) },
            { "abajo",     (1, 0) },
            { "izquierda", (0, -1) },
            { "derecha",   (0, 1) }
        };

        // Para cada movimiento, verificamos si es válido (dentro de los límites del tablero)
        // y, de serlo, generamos un nuevo estado intercambiando la posición del espacio en blanco.
        foreach (var mov in movimientos)
        {
            int nuevaFila = filaBlanco + mov.Value.dFila;
            int nuevaCol = colBlanco + mov.Value.dColumna;

            // Comprobamos límites del tablero.
            if (nuevaFila >= 0 && nuevaFila < 3 && nuevaCol >= 0 && nuevaCol < 3)
            {
                // Clonar el estado actual.
                int[,] nuevoEstado = (int[,])estadoActual.Clone();

                // Realizar el intercambio: mover la pieza en la dirección indicada al espacio en blanco.
                nuevoEstado[filaBlanco, colBlanco] = nuevoEstado[nuevaFila, nuevaCol];
                nuevoEstado[nuevaFila, nuevaCol] = 0;

                // Crear la acción que representa este movimiento.
                AccionPuzzle accion = new AccionPuzzle(mov.Key);

                // Crear un sucesor que contiene la acción y el nuevo estado.
                SucesorPuzzle sucesor = new SucesorPuzzle
                {
                    Accion = accion,
                    Estado = nuevoEstado
                };

                sucesores.Add(sucesor);
            }
        }

        return sucesores;
    }
}

// Representa una acción en el 8-puzzle (mover el espacio en blanco en una dirección).
public class AccionPuzzle
{
    public string Direccion { get; }

    public AccionPuzzle(string direccion)
    {
        Direccion = direccion;
    }

    public override string ToString() => $"Mover espacio en blanco {Direccion}";
}

// Representa un sucesor en el 8-puzzle: la acción aplicada y el estado resultante.
public class SucesorPuzzle
{
    public AccionPuzzle Accion { get; set; }
    public int[,] Estado { get; set; }
}

