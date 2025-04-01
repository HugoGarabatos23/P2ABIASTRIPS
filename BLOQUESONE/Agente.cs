namespace BLOQUESONE;


class Sucesor
{
    public Accion Accion { get; set; }
    public Dictionary< Predicado , bool> Estado { get; set; }
}


public class Agente
{   
    private MundoReal copiaMundo;

    public Agente(MundoReal mundo)
    {
        copiaMundo = new MundoReal(mundo); // Copia profunda
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

                // 3. Verificar precondiciones usando los objetos Predicado
                    if (estado.ContainsKey(onBX) && estado[onBX] &&    // ¿'b' está realmente sobre 'x'?
                        estado.ContainsKey(clearB) && estado[clearB] && // ¿'b' está libre para mover?
                        estado.ContainsKey(clearY) && estado[clearY])   // ¿'y' está libre para recibir?
                {
                    Accion accion = new Accion(b, x, y);
                    Dictionary<string, bool> nuevoEstado = new MundoReal(estado).AplicarAccion(accion);
                    sucesores.Add(new Sucesor { Accion = accion, Estado = nuevoEstado });
                }
            }
        }
    }

    return sucesores;
    }
}
