// Hugo Garabatos Díaz y Pedro Agrelo Márquez

namespace BLOQUESONE;

/// <summary>
/// Clase principal que ejecuta el flujo completo de planificación y ejecución.
/// </summary>
class Program
{
    /// Punto de entrada del programa. Coordina:
    /// 1. Carga de escenario
    /// 2. Creación de mundo y agente
    /// 3. Planificación
    /// 4. Ejecución
    /// </summary>
    static void Main()
    {
        // 1. Cargar estados desde el archivo
        string ruta_archivo = "escenario2.txt";
        LectorEstados lector = new LectorEstados(ruta_archivo);


        // 2. Crear mundo real y agente
        MundoReal mundo = new MundoReal(
            lector.Bloques,
            lector.EstadoInicial
        );

        Agente agente = new Agente(lector.EstadoInicial);
        

        // 3. Planificar

        ResultadoBusqueda resultado = agente.Planificar(
            estadoObjetivo: lector.EstadoObjetivo,
            bloques: lector.Bloques
        );
    

        // 4. Mostrar plan
        Console.WriteLine("\n=== PLAN GENERADO ===");
        foreach (Accion accion in resultado.Plan)
        {
            Console.WriteLine(accion);
        }

        // 5. Ejecutar en mundo real
        mundo.EjecutarPlan(resultado.Plan);
        
    }
          
}