namespace BLOQUESONE;

public class LectorEstados
{
    public Dictionary<Predicado, bool> EstadoInicial { get; }
    public Dictionary<Predicado, bool> EstadoObjetivo { get; }
    public string[] Bloques { get; }
    public string[] Posiciones { get; }

    public LectorEstados(string rutaArchivo)
    {
        var lineas = File.ReadAllLines(rutaArchivo);
        Bloques = LeerSeccion(lineas, "[Bloques]")[0].Split(',');
        Posiciones = LeerSeccion(lineas, "[Posiciones]")[0].Split(',');
        EstadoInicial = ParsearEstado(LeerSeccion(lineas, "[EstadoInicial]"));
        EstadoObjetivo = ParsearEstado(LeerSeccion(lineas, "[EstadoObjetivo]"));
    }

    private List<string> LeerSeccion(string[] lineas, string seccion)
    {
        var resultado = new List<string>();
        bool enSeccion = false;
        
        foreach (var linea in lineas)
        {
            if (linea.Trim() == seccion)
            {
                enSeccion = true;
                continue;
            }

            if (enSeccion)
            {
                if (linea.Trim().StartsWith("[")) break;
                if (!string.IsNullOrWhiteSpace(linea))
                    resultado.Add(linea.Trim());
            }
        }
        return resultado;
    }

    private Dictionary<Predicado, bool> ParsearEstado(List<string> lineas)
    {
        var estado = new Dictionary<Predicado, bool>();
        foreach (var linea in lineas)
        {
            var partes = linea.Split('=');
            var predicado = Predicado.Parse(partes[0]);
            bool valor = bool.Parse(partes[1]);
            estado.Add(predicado, valor);
        }
        return estado;
    }
}