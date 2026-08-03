namespace ScrumBoard.Domain.Enums;

public sealed class ProjectStatus(string name, int value)
{
    public static readonly ProjectStatus Pending = new("Pending",1);
    public static readonly ProjectStatus InProgres = new("InProgres",2);
    public static readonly ProjectStatus Completed = new("Completed",3);
    public static readonly ProjectStatus Cancelled = new("Cancelled",4);
    
    public string Name { get; } = name;
    public int Value { get; } = value;

    public static ProjectStatus FromValue(int value)
    {
        return value switch
        {
            1 => Pending,
            2 => InProgres,
            3 => Completed,
            4 => Cancelled,
            _ => throw new ArgumentOutOfRangeException(nameof(value), value,
                "El valor proporcionado no corresponde a ningún estado de proyecto válido.")
        };
    }

    public static ProjectStatus FromName(string name)
    {
        return name switch
        {
            nameof(Pending) => Pending,
            nameof(InProgres) => InProgres,
            nameof(Completed) => Completed,
            nameof(Cancelled) => Cancelled,
            _ => throw new ArgumentException($"El nombre '{name}' no es un estado de proyecto válido.", nameof(name))
        };
    }
}