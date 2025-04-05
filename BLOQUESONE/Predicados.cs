namespace BLOQUESONE;

/// <summary>
/// Representa un predicado en el sistema de planificación STRIPS.
/// Ejemplos: "on(A,B)", "clear(C)".
/// </summary>
public class Predicado
{
    /// <summary>
    /// Nombre del predicado (ej: "on", "clear").
    /// </summary>   
    public string Nombre { get; }

    /// <summary>
    /// Argumentos del predicado (ej: ["A", "B"] para "on(A,B)").
    /// </summary>
    public string[] Argumentos { get; }
    
    /// <summary>
    /// Crea un nuevo predicado con validación básica de argumentos.
    /// </summary>
    /// <param name="nombre">Tipo de predicado ("on", "clear", etc.).</param>
    /// <param name="argumentos">Argumentos específicos del predicado.</param>
    /// <exception cref="ArgumentException">
    /// Si no se cumplen las reglas: 
    /// - "on" requiere 2 argumentos
    /// - "clear" requiere 1 argumento
    /// </exception>
    public Predicado(string nombre, params string[] argumentos)
    {
        if (nombre == "on" && argumentos.Length != 2)
            throw new ArgumentException("El predicado 'on' requiere exactamente 2 argumentos");
        if (nombre == "clear" && argumentos.Length != 1)
            throw new ArgumentException("El predicado 'clear' requiere exactamente 1 argumento");
     
        Nombre = nombre;
        Argumentos = argumentos;

    }

    /// <summary>
    /// Convierte el predicado a su representación en cadena (ej: "on(A,B)").
    /// </summary>
    public override string ToString()
    {
        return $"{Nombre}({string.Join(", ", Argumentos)})";
    }


    /// <summary>
    /// Parsea una cadena en un objeto Predicado.
    /// </summary>
    /// <param name="input">Cadena con formato "nombre(arg1, arg2)".</param>
    /// <returns>Nueva instancia de Predicado.</returns>
    /// <example>
    /// var p = Predicado.Parse("on(A, B)"); // Devuelve Predicado("on", "A", "B")
    /// </example>
    public static Predicado Parse(string input)
    {
        string[] partes = input.Split(new char[] { '(', ',', ')' }, StringSplitOptions.RemoveEmptyEntries);
        return new Predicado(
            partes[0].Trim(), // Nombre del predicado ("on")
            partes.Skip(1).Select(p => p.Trim()).ToArray() // Argumentos (["A", "mesa"])
        );
    }


    /// <summary>
    /// Compara dos predicados por nombre y argumentos.
    /// </summary>

    public override bool Equals(object obj)
    {
        return obj is Predicado otro &&
            Nombre == otro.Nombre &&
            Argumentos.SequenceEqual(otro.Argumentos);
    }

    /// <summary>
    /// Genera un código hash único para el predicado.
    /// Fundamental para usarlo como clave en Dictionary.
    /// </summary>
    public override int GetHashCode()
    {
        int hash = 17;
        hash = hash * 23 + Nombre.GetHashCode();
        foreach (string arg in Argumentos)
        {
            hash = hash * 23 + arg.GetHashCode();
        }
        return hash;
    }
}
