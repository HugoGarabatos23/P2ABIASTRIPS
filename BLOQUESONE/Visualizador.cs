using System;
using System.Collections.Generic;
using System.Linq;

namespace BLOQUESONE
{
    public class VisualizadorEstado
    {
        public static void DibujarEstado(Dictionary<Predicado, bool> estado)
        {
            // Encontrar bases: bloques que están sobre "mesa"
            var bases = estado
                .Where(kvp => kvp.Value && kvp.Key.Nombre == "on" && kvp.Key.Argumentos[1] == "mesa")
                .Select(kvp => kvp.Key.Argumentos[0])
                .ToList();

            // Para cada base, construir la pila hacia arriba
            var columnas = new List<List<string>>();
            foreach (var baseBloque in bases)
            {
                var pila = new List<string> { baseBloque };
                var cima = baseBloque;

                while (true)
                {
                    var encima = estado
                        .Where(kvp => kvp.Value && kvp.Key.Nombre == "on" && kvp.Key.Argumentos[1] == cima)
                        .Select(kvp => kvp.Key.Argumentos[0])
                        .FirstOrDefault();

                    if (encima == null) break;

                    pila.Add(encima);
                    cima = encima;
                }

                columnas.Add(pila);
            }

            // Calcular la altura máxima
            int altura = columnas.Max(c => c.Count);

            // Imprimir verticalmente
            for (int nivel = altura - 1; nivel >= 0; nivel--)
            {
                foreach (var col in columnas)
                {
                    if (col.Count > nivel)
                        Console.Write($"  {col[nivel]}  ");
                    else
                        Console.Write("     ");
                }
                Console.WriteLine();
            }

            // Línea base
            Console.WriteLine(new string('-', columnas.Count * 5));
        }
    }
}
