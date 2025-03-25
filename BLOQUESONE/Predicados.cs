namespace BLOQUESONE;

public class Predicado
{
    public string Nombre { get; }
    public string[] Argumentos { get; }
    
    public Predicado(string nombre, params string[] argumentos)
    {
        if (argumentos.Length < 1 || argumentos.Length > 2)
            throw new ArgumentException("Los predicados deben tener 1 o 2 argumentos");
        
        Nombre = nombre;
        Argumentos = argumentos;
    }

    public override string ToString()
    {
        return $"{Nombre}({string.Join(", ", Argumentos)})";
    }

    public static Predicado Parse(string predicadoStr)
    {
        string[] partes = predicadoStr.Split(new char[] { '(', ',', ')' }, StringSplitOptions.RemoveEmptyEntries);
        return new Predicado(partes[0].Trim(), partes.Skip(1).Select(p => p.Trim()).ToArray());
    }

    public override bool Equals(object obj)
    {
        return obj is Predicado otro &&
               Nombre == otro.Nombre &&
               Argumentos.SequenceEqual(otro.Argumentos);
    }

    public override int GetHashCode()
    {
        return HashCode.Combine(Nombre, Argumentos);
    }
}
