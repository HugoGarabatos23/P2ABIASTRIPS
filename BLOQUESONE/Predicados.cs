namespace BLOQUESONE;

public class Predicado
{
    public string Nombre { get; }
    public string[] Argumentos { get; }
    
    public Predicado(string nombre, params string[] argumentos)
    {
        if (nombre == "on" && argumentos.Length != 2)
            throw new ArgumentException("El predicado 'on' requiere exactamente 2 argumentos");
        if (nombre == "clear" && argumentos.Length != 1)
            throw new ArgumentException("El predicado 'clear' requiere exactamente 1 argumento");
     
        Nombre = nombre;
        Argumentos = argumentos;

    }

    public override string ToString()
    {
        return $"{Nombre}({string.Join(", ", Argumentos)})";
    }


    //<docstrin>
    // Trim() elimina espacios en blanco alrededor (útil si el input es "on (A, mesa)").
    //Select(p => p.Trim()): Aplica Trim() a cada argumento (por si hay espacios).
    // ToArray(): Convierte el resultado en un array de strings.
    public static Predicado Parse(string input)
    {
        string[] partes = input.Split(new char[] { '(', ',', ')' }, StringSplitOptions.RemoveEmptyEntries);
        return new Predicado(
            partes[0].Trim(), // Nombre del predicado ("on")
            partes.Skip(1).Select(p => p.Trim()).ToArray() // Argumentos (["A", "mesa"])
        );
    }


    public override bool Equals(object obj)
    {
        return obj is Predicado otro &&
            Nombre == otro.Nombre &&
            Argumentos.SequenceEqual(otro.Argumentos);
    }

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
