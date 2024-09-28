namespace RT.CommandLine;

/// <summary>Specifies that the command-line parser should ignore a field.</summary>
[AttributeUsage(AttributeTargets.Field, Inherited = false, AllowMultiple = false)]
public sealed class IgnoreAttribute : Attribute
{
    /// <summary>Constructor.</summary>
    public IgnoreAttribute() { }
}
