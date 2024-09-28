namespace RT.CommandLine;

/// <summary>Use this on an abstract class to specify that its subclasses represent various commands.</summary>
[AttributeUsage(AttributeTargets.Class, Inherited = false, AllowMultiple = false)]
public sealed class CommandGroupAttribute : Attribute
{
    /// <summary>Constructor.</summary>
    public CommandGroupAttribute() { }
}
