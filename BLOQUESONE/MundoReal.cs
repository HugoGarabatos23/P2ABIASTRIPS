namespace BLOQUESONE;

public class MundoReal
{
    public Dictionary<Predicado, bool> Estado { get; private set; } // para solo mutar el mundo desde aquí

    public MundoReal(Dictionary<Predicado, bool> estadoInicial)
    {
        Estado = new Dictionary<Predicado, bool>(estadoInicial);
        ValidarEstadoInicial();
    }

    private void ValidarEstadoInicial()
    {
        foreach (KeyValuePair<Predicado, bool> kvp in Estado)
        {
            if (kvp.Key.Nombre == "on" && kvp.Value)
            {
                if (kvp.Key.Argumentos[1] != "mesa")
                {
                    Predicado predicadoBase = new Predicado("on", kvp.Key.Argumentos[1], "mesa");
                    if (!Estado.ContainsKey(predicadoBase) || !Estado[predicadoBase])
                    {
                        throw new ArgumentException($"El bloque {kvp.Key.Argumentos[1]} no tiene base válida");
                    }
                }
            }
        }
    }

    public Dictionary<Predicado, bool> AplicarAccion(Accion accion)
    {

        string bloque = accion.Bloque; //Aprovechamos los ya definidos
        string desde = accion.Desde; 
        string hacia = accion.Hacia;

        Predicado onAntes = new Predicado("on", bloque, desde);
        Predicado onDespues = new Predicado("on", bloque, hacia);
        Predicado clearDesde = new Predicado("clear", desde);
        Predicado clearHacia = hacia != "mesa" ? new Predicado("clear", hacia) : null; //mesa especial no requiere estar libre 

        Dictionary<Predicado, bool> nuevoEstado = new Dictionary<Predicado, bool>(Estado)
        {
            [onAntes] = false,
            [onDespues] = true,
            [clearDesde] = true
        };

        if (clearHacia != null) // Solo actualiza si no es la mesa
        {
            nuevoEstado[clearHacia] = false;  // El destino ya no está libre
        }

        return nuevoEstado;
    }

    public void MostrarEstado()
    {
        Console.WriteLine("Estado actual:");
        foreach (KeyValuePair<Predicado, bool> kvp in Estado.Where(p => p.Value))
        {
            Console.WriteLine($"- {kvp.Key}");
        }
    }
}