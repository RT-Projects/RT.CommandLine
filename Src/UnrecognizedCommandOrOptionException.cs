using RT.Util.Consoles;
using RT.Util.ExtensionMethods;

namespace RT.CommandLine;

/// <summary>
///     Specifies that the command-line parser encountered a command or option that was not recognised (there was no <see
///     cref="OptionAttribute"/> or <see cref="CommandNameAttribute"/> attribute with a matching option or command name).</summary>
[Serializable]
public sealed class UnrecognizedCommandOrOptionException(string commandOrOptionName, Func<int, ConsoleColoredString> helpGenerator, Exception inner)
    : CommandLineParseException("The specified command or option, {0}, is not recognized.".ToConsoleColoredString().Fmt(commandOrOptionName.Color(ConsoleColor.White)), helpGenerator, inner)
{
    /// <summary>The unrecognized command name or option name.</summary>
    public string CommandOrOptionName { get; private set; } = commandOrOptionName;
    /// <summary>Constructor.</summary>
    public UnrecognizedCommandOrOptionException(string commandOrOptionName, Func<int, ConsoleColoredString> helpGenerator) : this(commandOrOptionName, helpGenerator, null) { }
}
