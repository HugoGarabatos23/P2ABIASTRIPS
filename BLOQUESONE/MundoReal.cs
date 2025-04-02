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

    public void EjecutarAccion(Accion accion)
    {
        // Usa la versión estática
        Estado = OperacionesBloques.AplicarAccion(Estado, accion);
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