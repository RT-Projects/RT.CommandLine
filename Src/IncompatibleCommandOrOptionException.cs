using RT.Util.Consoles;
using RT.Util.ExtensionMethods;

namespace RT.CommandLine;

/// <summary>
///     Specifies that the command-line parser encountered a command or option that is not allowed in conjunction with a
///     previously-encountered command or option.</summary>
[Serializable]
public sealed class IncompatibleCommandOrOptionException(string earlier, string later, Func<int, ConsoleColoredString> helpGenerator, Exception inner)
    : CommandLineParseException("The command or option, {0}, cannot be used in conjunction with {1}. Please specify only one of the two.".ToConsoleColoredString().Fmt(later.Color(ConsoleColor.White), earlier.Color(ConsoleColor.White)), helpGenerator, inner)
{
    /// <summary>
    ///     The earlier option or command, which by itself is valid, but conflicts with the <see
    ///     cref="LaterCommandOrOption"/>.</summary>
    public string EarlierCommandOrOption { get; private set; } = earlier;
    /// <summary>The later option or command, which conflicts with the <see cref="EarlierCommandOrOption"/>.</summary>
    public string LaterCommandOrOption { get; private set; } = later;
    /// <summary>Constructor.</summary>
    public IncompatibleCommandOrOptionException(string earlier, string later, Func<int, ConsoleColoredString> helpGenerator) : this(earlier, later, helpGenerator, null) { }
}
