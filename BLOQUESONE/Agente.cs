namespace BLOQUESONE;
public class Agente
{

    public string[] Bloques ;
    public string[] Posiciones;
    private Dictionary<Predicado, bool> _estadoSimulado;

    public Agente(string[] bloques, string[] posiciones, Dictionary<Predicado, bool> estadoInicial)
    {
        _estadoSimulado = CopiarEstado(estadoInicial);
        Bloques = bloques;
        Posiciones = posiciones;
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
                    Dictionary<Predicado, bool> nuevoEstado = AplicarAccion(estado, accion);
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

    // ... (resto de métodos permanece igual)}
    private Dictionary<Predicado, bool> AplicarAccion(Dictionary<Predicado, bool> estadoActual, Accion accion)
    {
        // Crear copia profunda del estado
        Dictionary<Predicado, bool> nuevoEstado = new Dictionary<Predicado, bool>(estadoActual);

        // 1. Eliminar el predicado antiguo
        Predicado onAntiguo = new Predicado("on", accion.Bloque, accion.Desde);
        nuevoEstado[onAntiguo] = false;

        // 2. Añadir nuevo predicado
        Predicado onNuevo = new Predicado("on", accion.Bloque, accion.Hacia);
        nuevoEstado[onNuevo] = true;

        // 3. Actualizar clears
        Predicado clearOrigen = new Predicado("clear", accion.Desde);
        nuevoEstado[clearOrigen] = true;

        if (accion.Hacia != "mesa")
        {
            Predicado clearDestino = new Predicado("clear", accion.Hacia);
            nuevoEstado[clearDestino] = false;
        }

        return nuevoEstado;
    }
}