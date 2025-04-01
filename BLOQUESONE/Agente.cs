public class Agente
{
    private MundoReal copiaMundo;
    public string[] Bloques { get; }
    public string[] Posiciones { get; }

    public Agente(MundoReal mundo, string[] bloques, string[] posiciones)
    {
        copiaMundo = new MundoReal(mundo);
        Bloques = bloques;
        Posiciones = posiciones;
    }

    private List<Sucesor> GenerarSucesores(Dictionary<Predicado, bool> estado)
    {
        List<Sucesor> sucesores = new List<Sucesor>();
        
        foreach (string b in Bloques)
        {
            foreach (string x in Posiciones)
            {
                foreach (string y in Posiciones)
                {
                    if (x == y) continue;

                    Predicado onBX = new Predicado("on", b, x);
                    Predicado clearB = new Predicado("clear", b);
                    Predicado clearY = new Predicado("clear", y);

                    if (estado.TryGetValue(onBX, out bool on) && on &&
                        estado.TryGetValue(clearB, out bool clear1) && clear1 &&
                        estado.TryGetValue(clearY, out bool clear2) && clear2)
                    {
                        Accion accion = new Accion(b, x, y);
                        Dictionary<Predicado, bool> nuevoEstado = new MundoReal(estado).AplicarAccion(accion);
                        sucesores.Add(new Sucesor { 
                            Accion = accion, 
                            Estado = nuevoEstado 
                        });
                    }
                }
            }
        }
        return sucesores;
    }

    private List<Accion> GenerarAccionesPosibles(Dictionary<Predicado, bool> estado)
    {
        List<Accion> accionesValidas = new List<Accion>();

        foreach (string bloque in Bloques)
        {
            foreach (string origen in Posiciones)
            {
                foreach (string destino in Posiciones)
                {
                    if (origen == destino) continue; // No mover al mismo lugar

                    // Verificar precondiciones
                    if (EsAccionValida(estado, bloque, origen, destino))
                    {
                        accionesValidas.Add(new Accion(bloque, origen, destino));
                    }
                }
            }
        }
        return accionesValidas;
    }

    private bool EsAccionValida(Dictionary<Predicado, bool> estado, string bloque, string origen, string destino)
    {
        Predicado onBloqueOrigen = new Predicado("on", bloque, origen);
        Predicado clearBloque = new Predicado("clear", bloque);
        Predicado clearDestino = new Predicado("clear", destino);

        return estado.ContainsKey(onBloqueOrigen) && estado[onBloqueOrigen] && // Bloque está en el origen
               estado.ContainsKey(clearBloque) && estado[clearBloque] &&       // Bloque no tiene nada encima
               estado.ContainsKey(clearDestino) && estado[clearDestino];       // Destino está libre
    }

    private Dictionary<Predicado, bool> AplicarAccionGenerandoEstado(Dictionary<Predicado, bool> estadoActual, Accion accion)
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