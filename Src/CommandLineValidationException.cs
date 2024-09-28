using RT.Util.Consoles;

namespace RT.CommandLine;

/// <summary>Specifies that the arguments specified by the user on the command-line do not pass the custom validation checks.</summary>
[Serializable]
public sealed class CommandLineValidationException(ConsoleColoredString message, Func<int, ConsoleColoredString> helpGenerator)
    : CommandLineParseException(message, helpGenerator)
{
}
