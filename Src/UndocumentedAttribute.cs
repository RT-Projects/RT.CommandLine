namespace RT.CommandLine;

/// <summary>
///     Specifies that a specific command-line option should not be printed in help pages, i.e. the option should explicitly
///     be undocumented.</summary>
[AttributeUsage(AttributeTargets.Field | AttributeTargets.Class, Inherited = false, AllowMultiple = true)]
public sealed class UndocumentedAttribute : Attribute
{
    /// <summary>Constructor.</summary>
    public UndocumentedAttribute() { }
}
