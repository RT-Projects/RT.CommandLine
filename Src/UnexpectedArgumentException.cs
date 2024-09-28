using RT.Internal;
using RT.Util.Consoles;
using RT.Util.ExtensionMethods;

namespace RT.CommandLine;

/// <summary>
///     Specifies that the command-line parser encountered additional command-line arguments when it expected the end of the
///     command line.</summary>
[Serializable]
public sealed class UnexpectedArgumentException(string[] unexpectedArgs, Func<int, ConsoleColoredString> helpGenerator, Exception inner)
    : CommandLineParseException("Unexpected parameter: {0}".ToConsoleColoredString().Fmt(unexpectedArgs.Select(prm => prm.Length > 50 ? $"{prm.Substring(0, 47)}..." : prm).FirstOrDefault().Color(CmdLineColor.UnexpectedArgument)), helpGenerator, inner)
{
    /// <summary>Contains the first unexpected argument and all of the subsequent arguments.</summary>
    public string[] UnexpectedParameters { get; private set; } = unexpectedArgs;
    /// <summary>Constructor.</summary>
    public UnexpectedArgumentException(string[] unexpectedArgs, Func<int, ConsoleColoredString> helpGenerator) : this(unexpectedArgs, helpGenerator, null) { }
}
