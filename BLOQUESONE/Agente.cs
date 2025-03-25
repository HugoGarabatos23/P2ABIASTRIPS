namespace BLOQUESONE;

public class Agente
{   
    private MundoReal copiaMundo;

    public Agente(MundoReal mundo)
    {
        copiaMundo = new MundoReal(mundoReal); // Copia profunda
    }

    private List<Sucesor> GenerarSucesores(Dictionary<string, bool> estado)
{
    List<Sucesor> sucesores = new List<Sucesor>();

    string[] bloques = { "A", "B", "C", "D", "E", "F" };
    string[] posiciones = { "mesa", "A", "B", "C", "D", "E", "F" };

    foreach (string b in bloques)
    {
        foreach (string x in posiciones)
        {
            foreach (string y in posiciones)
            {
                if (x == y) continue;

                    Predicado onBX = new Predicado("on", b, x);
                    Predicado clearB = new Predicado("libre", b);
                    Predicado clearY = new Predicado("libre", y);

                if (estado.ContainsKey($"on({b}, {x})") && estado[$"on({b}, {x})"] &&
                    estado.ContainsKey($"libre({b})") && estado[$"libre({b})"] &&
                    estado.ContainsKey($"c({y})") && estado[$"libre({y})"])
                {
                    Tuple<string, string, string> accion = Tuple.Create(b, x, y);
                    Dictionary<string, bool> nuevoEstado = new MundoReal(estado).AplicarAccion(accion);
                    sucesores.Add(new Sucesor { Accion = accion, Estado = nuevoEstado });
                }
            }
        }
    }

    return sucesores;
    }
}
