namespace ScrumBoard.Domain.Services;


/// <summary>
/// Estrategia de posición fraccionaria: al insertar una tarea entre dos existentes,
/// el nuevo Order es el punto medio entre ambas. Evita renumerar toda la columna 
/// en cada movimiento, algo crítico porque el tablero se actualiza en tiempo real
/// con múltiples usuarios moviendo tareas simultáneamente.
/// </summary>
public static class TaskOrderCalculator
{
    private const decimal DefaultGap = 1024m;

    public static decimal CalculateNewOrder(decimal? previousOrder, decimal? nextOrder)
    {
        if (previousOrder is null && nextOrder is null)
            return DefaultGap;

        if (previousOrder is null)
            return nextOrder!.Value / 2m;

        if (nextOrder is null)
            return previousOrder.Value + DefaultGap;

        return (previousOrder.Value + nextOrder.Value) / 2m;
    }
}