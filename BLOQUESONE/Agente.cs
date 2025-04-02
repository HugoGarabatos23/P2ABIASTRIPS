namespace BLOQUESONE;
public class Agente
{
    public string[] Bloques ;
    public string[] Posiciones;
    public List<Accion> PlanActual { get; private set; }
    public int CostoPlan { get; private set; }
    private Dictionary<Predicado, bool> _estadoSimulado;

    public Agente(string[] bloques, string[] posiciones, Dictionary<Predicado, bool> estadoInicial)
    {
        
        _estadoSimulado = CopiarEstado(estadoInicial);
        Bloques = bloques;
        Posiciones = posiciones;
        PlanActual = new List<Accion>();
        CostoPlan = 0;
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

    public ResultadoBusqueda Planificar(Dictionary<Predicado, bool> estadoObjetivo)
    {
        ResultadoBusqueda resultado = BusquedaAEstrella.EncontrarSolucion(
            _estadoSimulado,
            estadoObjetivo,
            estado => GenerarSucesores(estado)
        );

        PlanActual = resultado.Plan;
        CostoPlan = resultado.CostoTotal;
        return resultado;
    }

    private Dictionary<Predicado, bool> AplicarAccionSimulada(Accion accion)
    {
        return OperacionesBloques.AplicarAccion(_estadoSimulado, accion);
    }
   
    // Versión optimizada y coherente que primero valida precondiciones
    private List<Sucesor> GenerarSucesores(Dictionary<Predicado, bool> estado)
    {
        List<Sucesor> sucesores = new List<Sucesor>();
        
        // 1. Filtrar bloques que pueden moverse (clear=true)
        foreach (string bloque in Bloques)
        {
            if (!estado.GetValueOrDefault(new Predicado("clear", bloque), false))
                continue;

            // 2. Encontrar posición actual del bloque
            string posicionActual = null;
            foreach (string posicion in Posiciones)
            {
                if (estado.GetValueOrDefault(new Predicado("on", bloque, posicion), false))
                {
                    posicionActual = posicion;
                    break;
                }
            }
            if (posicionActual == null) continue;

            // 3. Generar destinos válidos
            foreach (string destino in Posiciones)
            {
                if (destino == posicionActual) continue;
                
                // Verificar precondiciones específicas para este movimiento
                if (EsMovimientoValido(estado, bloque, posicionActual, destino))
                {
                    Accion accion = new Accion(bloque, posicionActual, destino);
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
}