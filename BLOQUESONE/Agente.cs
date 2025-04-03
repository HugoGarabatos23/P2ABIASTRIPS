namespace BLOQUESONE;

public class Agente
{
 
 
    public Dictionary<Predicado, bool> _estadoSimulado;
    public Agente(Dictionary<Predicado, bool> estadoInicial)
    {
        
        _estadoSimulado = CopiarEstado(estadoInicial);
    }

     // Método para generar copias seguras del estado
    private Dictionary<Predicado, bool> CopiarEstado(Dictionary<Predicado, bool> original)
    {
        Dictionary<Predicado, bool> copia = new Dictionary<Predicado, bool>();
        foreach (KeyValuePair<Predicado, bool> kvp in original)
        {
            copia.Add(new Predicado(kvp.Key.Nombre, kvp.Key.Argumentos), kvp.Value);
        }
        return copia;
    }

    public ResultadoBusqueda Planificar(Dictionary<Predicado, bool> estadoObjetivo,
    string[]bloques)
    {
       return BusquedaAEstrella.EncontrarSolucion(_estadoSimulado, estadoObjetivo,
        estado=> GenerarSucesores(estado,bloques)
        );
    }

    private Dictionary<Predicado, bool> AplicarAccionSimulada(Dictionary<Predicado,bool>estado, Accion accion)
    {
        return OperacionesBloques.AplicarAccion(estado, accion);
    }
   
    // Versión optimizada y coherente que primero valida precondiciones
    private List<Sucesor> GenerarSucesores(Dictionary<Predicado, bool> estado, string[]bloques)
    {
        List<Sucesor> sucesores = new List<Sucesor>();
        
        // 1. Filtrar bloques que pueden moverse (clear=true)
        foreach (string bloque in bloques)
        {
            if (!estado.GetValueOrDefault(new Predicado("clear", bloque), false))
                continue;

            // 2. Encontrar base actual del bloque (puede ser mesa o otro bloque)
            string baseActual = null;
            foreach (string posibleBase in bloques.Concat(new[] {"mesa"}))
            {
                if (estado.GetValueOrDefault(new Predicado("on", bloque, posibleBase), false))
                {
                    baseActual = posibleBase;
                    break;
                }
            }
            if (baseActual == null) continue;

            // 3. Generar destinos válidos
            foreach (string destino in bloques.Concat(new[] { "mesa" }))
            {
                if (destino == baseActual) continue;
                
                // Verificar precondiciones específicas para este movimiento
                if (EsMovimientoValido(estado, bloque, baseActual, destino))
                {
                    Accion accion = new Accion(bloque, baseActual, destino);
                    Dictionary<Predicado, bool> nuevoEstado = AplicarAccionSimulada(estado, accion);
                    sucesores.Add(new Sucesor {
                        Accion = accion,
                        Estado = nuevoEstado,
                        Costo = 1
                    });
                }
            }
        }
        return sucesores;
    }

    // Método específico para validar movimientos
    private bool EsMovimientoValido(Dictionary<Predicado, bool> estado, 
                                  string bloque, string desde, string hacia)
    {
        // 1. El bloque debe estar en la posición de origen
        if (!estado.GetValueOrDefault(new Predicado("on", bloque, desde), false))
            return false;

        // 2. El bloque debe estar libre (nada encima)
        if (!estado.GetValueOrDefault(new Predicado("clear", bloque), false))
            return false;

        // 3. Si el destino no es la mesa, debe estar libre
        if (hacia != "mesa" && !estado.GetValueOrDefault(new Predicado("clear", hacia), false))
            return false;

        return true;
    }
        // Muestra el estado actual del mundo
}