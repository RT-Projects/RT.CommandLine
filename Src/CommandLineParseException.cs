using RT.Util.Consoles;
using RT.Util.ExtensionMethods;

namespace RT.CommandLine;

/// <summary>Represents any error encountered while parsing a command line. This class is abstract.</summary>
[Serializable]
public abstract class CommandLineParseException(ConsoleColoredString message, Func<int, ConsoleColoredString> helpGenerator, Exception inner)
    : Exception(message.ToString(), inner)
{
    /// <summary>
    ///     Generates the help screen to be output to the user on the console. For non-internationalised (single-language)
    ///     applications, pass null for the Translation parameter.</summary>
    internal Func<int, ConsoleColoredString> GenerateHelpFunc { get; private set; } = helpGenerator;
    /// <summary>Contains the error message that describes the cause of this exception.</summary>
    public ConsoleColoredString ColoredMessage { get; private set; } = message;
    /// <summary>
    ///     Generates the help screen to be output to the user on the console.</summary>
    /// <param name="wrapWidth">
    ///     The character width at which the output should be word-wrapped. The default (<c>null</c>) uses <see
    ///     cref="ConsoleUtil.WrapToWidth"/>.</param>
    public ConsoleColoredString GenerateHelp(int? wrapWidth = null) { return GenerateHelpFunc(wrapWidth ?? ConsoleUtil.WrapToWidth()); }
    /// <summary>
    ///     Generates a printable description of the error represented by this exception, typically used to tell the user what
    ///     they did wrong.</summary>
    /// <param name="wrapWidth">
    ///     The character width at which the output should be word-wrapped. The default (<c>null</c>) uses <see
    ///     cref="ConsoleUtil.WrapToWidth"/>.</param>
    public ConsoleColoredString GenerateErrorText(int? wrapWidth = null)
    {
        var strings = new List<ConsoleColoredString>();
        var message = "Error:".Color(CmdLineColor.Error) + " " + ColoredMessage;
        foreach (var line in message.WordWrap(wrapWidth ?? ConsoleUtil.WrapToWidth(), "Error:".Length + 1))
        {
            strings.Add(line);
            strings.Add(Environment.NewLine);
        }
        return new ConsoleColoredString(strings);
    }

    /// <summary>Constructor.</summary>
    public CommandLineParseException(ConsoleColoredString message, Func<int, ConsoleColoredString> helpGenerator) : this(message, helpGenerator, null) { }

    /// <summary>
    ///     Prints usage information, followed by an error message describing to the user what it was that the parser didn't
    ///     understand.</summary>
    public virtual void WriteUsageInfoToConsole()
    {
        ConsoleUtil.Write(GetUsageInfo());
    }

    /// <summary>
    ///     Generates and returns usage information, followed by an error message describing to the user what it was that the
    ///     parser didn't understand.</summary>
    public ConsoleColoredString GetUsageInfo()
    {
        var s = GenerateHelp(ConsoleUtil.WrapToWidth());
        if (WriteErrorText)
            s += Environment.NewLine + GenerateErrorText(ConsoleUtil.WrapToWidth());
        return s;
    }

    /// <summary>
    ///     Determines whether <see cref="WriteUsageInfoToConsole"/> and <see cref="GetUsageInfo"/> should call <see
    ///     cref="GenerateErrorText"/> and output it to the console. Default is <c>true</c>.</summary>
    /// <remarks>
    ///     Only set this to <c>false</c> if the user explicitly asked to see the help screen. Otherwise its appearance
    ///     without explanation is confusing.</remarks>
    protected internal virtual bool WriteErrorText => true;
}
