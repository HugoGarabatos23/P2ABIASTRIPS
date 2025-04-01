namespace BLOQUESONE;

public class MundoReal
{
    public Dictionary<Predicado, bool> Estado { get; private set; }

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

        string bloque = accion.Item1;
        string desde = accion.Item2;
        string hacia = accion.Item3;

        Predicado onAntes = new Predicado("on", bloque, desde);
        Predicado onDespues = new Predicado("on", bloque, hacia);
        Predicado clearDesde = new Predicado("clear", desde);
        Predicado clearHacia = hacia != "mesa" ? new Predicado("clear", hacia) : null;

        Dictionary<Predicado, bool> nuevoEstado = new Dictionary<Predicado, bool>(Estado)
        {
            [onAntes] = false,
            [onDespues] = true,
            [clearDesde] = true
        };

        if (clearHacia != null)
        {
            nuevoEstado[clearHacia] = false;
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