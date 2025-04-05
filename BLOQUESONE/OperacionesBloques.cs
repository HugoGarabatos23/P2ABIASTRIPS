namespace BLOQUESONE;

/// <summary>
/// Clase estática con las operaciones básicas para manipular el estado del mundo de bloques
/// según las reglas STRIPS (añadir/eliminar predicados).
/// </summary>
public static class OperacionesBloques
{
    /// <summary>
    /// Aplica una acción al estado actual del mundo, devolviendo un NUEVO estado modificado.
    /// </summary>
    /// <param name="estadoActual">Estado antes de la acción (diccionario de predicados).</param>
    /// <param name="accion">Acción a ejecutar (ej: mover "A" desde "B" a "mesa").</param>
    /// <returns>Copia del estado actual con los cambios aplicados (inmutable).</returns>
    /// <remarks>
    /// Sigue el formato STRIPS:
    /// 1. Elimina el predicado antiguo (ej: "on(A, B)").
    /// 2. Añade el nuevo predicado (ej: "on(A, mesa)").
    /// 3. Actualiza los predicados "clear" afectados.
    /// </remarks>
        public static Dictionary<Predicado, bool> AplicarAccion(
        Dictionary<Predicado, bool> estadoActual, 
        Accion accion)
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