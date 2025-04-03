namespace BLOQUESONE;

class Program
{
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
        Console.WriteLine("=== PLANIFICACIÓN ===");
        var resultado = agente.Planificar(
            estadoObjetivo: lector.EstadoObjetivo,
            bloques: lector.Bloques
        );

        // 4. Mostrar plan
        Console.WriteLine("\n=== PLAN GENERADO ===");
        foreach (var accion in resultado.Plan)
        {
            Console.WriteLine(accion);
        }

        // 5. Ejecutar en mundo real
        mundo.EjecutarPlan(resultado.Plan);
    }
        
      
}