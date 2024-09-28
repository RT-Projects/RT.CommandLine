using RT.Util;

namespace RT.CommandLine;

/// <summary>
///     Use this to specify that a field in a class can be specified on the command line using an option, for example
///     <c>-a</c> or <c>--option-name</c>. The option name(s) MUST begin with a dash (<c>-</c>).</summary>
/// <param name="names">
///     The name of the option. Specify several names as synonyms if required.</param>
[AttributeUsage(AttributeTargets.Field, Inherited = false, AllowMultiple = false), RummageKeepUsersReflectionSafe]
public sealed class OptionAttribute(params string[] names) : Attribute
{
    /// <summary>All of the names of the option.</summary>
    public string[] Names { get; private set; } = names;
}
