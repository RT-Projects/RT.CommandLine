using RT.Util.Consoles;
using RT.Util.ExtensionMethods;

namespace RT.CommandLine;

/// <summary>Indicates that the user supplied one of the standard options we recognize as a help request.</summary>
[Serializable]
public sealed class CommandLineHelpRequestedException(Func<int, ConsoleColoredString> helpGenerator)
    : CommandLineParseException("The user has requested help using one of the help options.".Color(ConsoleColor.Gray), helpGenerator)
{
    /// <summary>Prints usage information.</summary>
    public override void WriteUsageInfoToConsole() => ConsoleUtil.Write(GenerateHelp(ConsoleUtil.WrapToWidth()));
    /// <inheritdoc/>
    protected internal override bool WriteErrorText => false;
}
