namespace BLOQUESONE;

public class LectorEstados
{
    public Dictionary<Predicado, bool> EstadoInicial { get; }
    public Dictionary<Predicado, bool> EstadoObjetivo { get; }
    public string[] Bloques { get; }
    public string[] Posiciones { get; }

    public LectorEstados(string rutaArchivo)
    {
        try
        {
            var lineas = File.ReadAllLines(rutaArchivo).Where(l => !string.IsNullOrWhiteSpace(l)).ToArray();
            Bloques = LeerSeccion(lineas, "[Bloques]")[0].Split(',').Select(b => b.Trim()).ToArray();
            Posiciones = LeerSeccion(lineas, "[Posiciones]")[0].Split(',').Select(p => p.Trim()).ToArray();
            EstadoInicial = ParsearEstado(LeerSeccion(lineas, "[EstadoInicial]"));
            EstadoObjetivo = ParsearEstado(LeerSeccion(lineas, "[EstadoObjetivo]"));

        }
        catch (Exception ex)
        {
            Console.WriteLine($"El error al leer el archivo : {ex.Message}" );
            throw;
        }
       
    }

    private List<string> LeerSeccion(string[] lineas, string seccion)
    {
        var resultado = new List<string>();
        bool enSeccion = false;
        
        foreach (var linea in lineas)
        {
            
            string lineaTrim = linea.Trim();
            
            if (lineaTrim == seccion)
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

        if (resultado.Count == 0)
            throw new InvalidDataException($"Sección '{seccion}' no encontrada o vacía");


        return resultado;
    }

    private Dictionary<Predicado, bool> ParsearEstado(List<string> lineas)
    {
        var estado = new Dictionary<Predicado, bool>();
        foreach (var linea in lineas)
        {
            var partes = linea.Replace(" ", "").Split('=');
            var predicado = Predicado.Parse(partes[0]);
            bool valor = bool.Parse(partes[1]);
            estado.Add(predicado, valor);
        }
        return estado;
    }
}