// Hugo Garabatos Díaz y Pedro Agrelo Márquez

namespace BLOQUESONE;

/// <summary>
/// Clase principal que ejecuta el flujo completo del programa.
/// </summary>
class Program
{
    /// <summary>
    /// Punto de entrada del programa.
    /// Flujo: Carga → Creación → Planificación+Ejecución
    /// </summary>
    static void Main()
    {
        try
        {
             // 1. Cargar estados desde el archivo
            string ruta_archivo = "escenario2.txt";
            LectorEstados lector = new LectorEstados(ruta_archivo);


            // 2. Crear mundo real y agente
            MundoReal mundo = new MundoReal(
                lector.Bloques,
                lector.EstadoInicial
            );

            Agente agente = new Agente(mundo); // Inyecto mundo real en el agente
            
            // 3. Ejecutar flujo completo (planificación simulada + ejecución real )
                agente.GenerarYEjecutarPlan(lector.EstadoObjetivo, lector.Bloques);
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error: {ex.Message}");
        }
        
        Console.WriteLine("\nPresione cualquier tecla para salir...");
        Console.ReadKey();
    }
}

