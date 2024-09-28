using RT.Util;

namespace RT.CommandLine;

/// <summary>
///     Use this to specify that a command-line parameter is positional, i.e. is not invoked by an option that starts with
///     "-".</summary>
[AttributeUsage(AttributeTargets.Field, Inherited = false, AllowMultiple = false), RummageKeepUsersReflectionSafe]
public sealed class IsPositionalAttribute : Attribute
{
    /// <summary>Constructor.</summary>
    public IsPositionalAttribute() { }
}
