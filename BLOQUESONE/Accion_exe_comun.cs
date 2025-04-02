namespace BLOQUESONE;

namespace BLOQUESONE
{
    public static class OperacionesBloques
    {
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
}