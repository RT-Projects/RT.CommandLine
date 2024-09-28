using RT.Util.Consoles;
using RT.Util.ExtensionMethods;

namespace RT.CommandLine;

/// <summary>
///     Specifies that the command-line parser encountered the end of the command line when it expected an argument to an
///     option.</summary>
[Serializable]
public sealed class IncompleteOptionException(string optionName, Func<int, ConsoleColoredString> helpGenerator, Exception inner)
    : CommandLineParseException("The {0} option must be followed by an additional parameter.".ToConsoleColoredString().Fmt(optionName.Color(ConsoleColor.White)), helpGenerator, inner)
{
    /// <summary>The name of the option that was missing an argument.</summary>
    public string OptionName { get; private set; } = optionName;
    /// <summary>Constructor.</summary>
    public IncompleteOptionException(string optionName, Func<int, ConsoleColoredString> helpGenerator) : this(optionName, helpGenerator, null) { }
}
