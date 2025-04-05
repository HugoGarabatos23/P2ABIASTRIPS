// Hugo Garabatos Díaz y Pedro Agrelo Márquez

namespace BLOQUESONE;

/// <summary>
/// Clase para leer y parsear archivos de configuración que definen:
/// - Bloques disponibles
/// - Estado inicial del mundo
/// - Estado objetivo deseado
/// </summary>
public class LectorEstados
{
    /// <summary>
    /// Estado inicial del mundo (diccionario de predicados y sus valores true/false).
    /// Ejemplo: "on(A,B)"=true significa "el bloque A está sobre el bloque B".
    /// </summary>
    public Dictionary<Predicado, bool> EstadoInicial { get; }

    /// <summary>
    /// Estado objetivo que el planificador debe alcanzar.
    /// </summary>
    public Dictionary<Predicado, bool> EstadoObjetivo { get; }

    /// <summary>
    /// Nombres de todos los bloques disponibles en el problema.
    /// Ejemplo: ["A", "B", "C"].
    /// </summary>
    public string[] Bloques { get; }


    
    /// <summary>
    /// Constructor: Lee y parsea un archivo de configuración.
    /// </summary>
    /// <param name="rutaArchivo">Ruta del archivo de configuración.</param>
    /// <exception cref="FileNotFoundException">Si el archivo no existe.</exception>
    /// <exception cref="InvalidDataException">Si el formato del archivo es incorrecto.</exception>
    public LectorEstados(string rutaArchivo)
    {
        try
        {
            string [] lineas = File.ReadAllLines(rutaArchivo).Where(l => !string.IsNullOrWhiteSpace(l)).ToArray();

            // Leer secciones del archivo
            List<string> bloquesLinea = LeerSeccion(lineas, "[Bloques]");
            List<string> estadoInicialLineas = LeerSeccion(lineas, "[EstadoInicial]");
            List<string> estadoObjetivoLineas = LeerSeccion(lineas, "[EstadoObjetivo]");

            // Parsear datos
            Bloques = bloquesLinea[0].Split(',')
                      .Select(bloque => bloque.Trim())
                      .ToArray();

            EstadoInicial = ParsearEstado(estadoInicialLineas);
            EstadoObjetivo = ParsearEstado(estadoObjetivoLineas);
        }
        catch (Exception ex)
        {
            Console.WriteLine($"El error al leer el archivo : {ex.Message}" );
            throw;
        }
       
    }

    /// <summary>
    /// Extrae las líneas de una sección específica del archivo.
    /// </summary>
    /// <param name="lineas">Todas las líneas del archivo.</param>
    /// <param name="seccion">Nombre de sección a buscar (ej: "[EstadoInicial]").</param>
    /// <returns>Lista de líneas de la sección (sin espacios).</returns>
    /// <exception cref="InvalidDataException">Si la sección no existe o está vacía.</exception>
    private List<string> LeerSeccion(string[] lineas, string seccion)
    {
        List<string> resultado = new List<string>();
        bool enSeccion = false;
        
        foreach (string linea in lineas)
        {
            
            string lineaTrim = linea.Trim();
            
            if (lineaTrim == seccion)
            {
                enSeccion = true;
                continue;
            }

            if (enSeccion)
            {
                if (linea.Trim().StartsWith("[")) break; // Nueva seccion encontrada
                if (linea.Trim() == "") continue; // Ignora lineas vacías
                if (!string.IsNullOrWhiteSpace(linea))
                    resultado.Add(linea.Trim());
            }
        }

        if (resultado.Count == 0)
            throw new InvalidDataException($"Sección '{seccion}' no encontrada o vacía");


        return resultado;
    }

    /// <summary>
    /// Convierte líneas de texto en un diccionario de predicados.
    /// </summary>
    /// <param name="lineas">Líneas en formato "predicado=valor" (ej: "on(A,B)=true").</param>
    /// <returns>Diccionario de predicados.</returns>
    /// <exception cref="FormatException">Si el formato de una línea es inválido.</exception>
    private Dictionary<Predicado, bool> ParsearEstado(List<string> lineas)
    {
        var estado = new Dictionary<Predicado, bool>();
        foreach (var linea in lineas)
        {
            string[] partes = linea.Replace(" ", "").Split('=');
            Predicado predicado = Predicado.Parse(partes[0]);
            bool valor = bool.Parse(partes[1]);
            estado.Add(predicado, valor);
        }
        return estado;
    }
}