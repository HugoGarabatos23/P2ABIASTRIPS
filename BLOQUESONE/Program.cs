namespace BLOQUESONE;

class Program
{
    static void Main()
    {
        // 1. Cargar estados desde el archivo
        string ruta_archivo = "escenarios_bloques.txt";
        LectorEstados lector = new LectorEstados(ruta_archivo);
        
        // 2. Crear el agente con los datos leídos
        Agente agente = new Agente(
            bloques: lector.Bloques,
            posiciones: lector.Posiciones,
            estadoInicial: lector.EstadoInicial
        );

        // 3. Mostrar estado inicial
        Console.WriteLine("=== ESTADO INICIAL ===");
        agente.MostrarEstadoActual();

        // 4. Planificar para alcanzar el estado objetivo
        Console.WriteLine("\nPlanificando...");
        var resultado = agente.Planificar(lector.EstadoObjetivo);

        if (resultado.Plan.Count == 0)
        {
            Console.WriteLine("No se encontró un plan válido.");
            return;
        }

        // 5. Ejecutar el plan con visualización paso a paso
        Console.WriteLine("\n=== EJECUTANDO PLAN ===");
        agente.EjecutarPlanConVisualizacion(pausarEntrePasos: true);

        // 6. Mostrar estado final
        Console.WriteLine("\n=== ESTADO FINAL ===");
        agente.MostrarEstadoActual();
    }
}