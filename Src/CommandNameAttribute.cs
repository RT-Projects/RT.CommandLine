using RT.Util;

namespace RT.CommandLine;

/// <summary>
///     Use this on a sub-class of an abstract class or on an enum value to specify the command the user must use to invoke
///     that class or enum value.</summary>
/// <param name="names">
///     The command(s) the user can specify to invoke this class or enum value.</param>
[AttributeUsage(AttributeTargets.Class | AttributeTargets.Field, Inherited = false, AllowMultiple = false), RummageKeepUsersReflectionSafe]
public sealed class CommandNameAttribute(params string[] names) : Attribute
{
    /// <summary>The command the user can specify to invoke this class.</summary>
    public string[] Names { get; private set; } = names;
}
