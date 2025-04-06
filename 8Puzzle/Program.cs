using System;
using System.Collections.Generic;
using System.Threading;
using PUZZLE; 

namespace PUZZLE
{
    class Program
    {
        static void Main()
        {
            // Leer estados desde el archivo de configuración (por ejemplo, "puzzle.txt")
            LectorEstadosPuzzle lector = new LectorEstadosPuzzle("estadopuzzle.txt");
            int[,] estadoInicial = lector.EstadoInicial;
            int[,] estadoObjetivo = lector.EstadoObjetivo;

            // Crear el mundo y mostrar el estado inicial
            MundoPuzzle mundo = new MundoPuzzle(estadoInicial);
            Console.WriteLine("Estado Inicial:");
            VisualizadorPuzzle.Dibujar(mundo.Tablero);

            // Planificar usando la búsqueda A* definida en BusquedaPuzzle
            ResultadoBusquedaPuzzle resultado = BusquedaPuzzle.EncontrarSolucion(
                estadoInicial,
                estadoObjetivo,
                (estado) => new AgentePuzzle(estado).GenerarSucesores()
            );

            // Mostrar el plan completo
            Console.WriteLine("Plan encontrado:");
            foreach (AccionPuzzle accion in resultado.Plan)
            {
                Console.WriteLine(accion);
            }
            Console.WriteLine("Presione una tecla para ejecutar el plan...");
            Console.ReadKey();

            // Ejecutar el plan en el mundo real (mostrando el estado actualizado en cada acción)
            int[,] estadoActual = (int[,])estadoInicial.Clone();
            foreach (AccionPuzzle accion in resultado.Plan)
            {
                estadoActual = OperacionesPuzzle.AplicarAccion(estadoActual, accion);
                Console.Clear();
                Console.WriteLine($"Ejecutando: {accion}");
                VisualizadorPuzzle.Dibujar(estadoActual);
                Thread.Sleep(1000);
            }
            Console.WriteLine("¡Objetivo alcanzado!");
        }
    }
}
