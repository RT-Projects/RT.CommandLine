using System.Reflection;
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

/// <summary>
///     Specifies that the command-line parser encountered the end of the command line when it expected additional mandatory
///     options.</summary>
[Serializable]
public sealed class MissingParameterException(FieldInfo paramField, FieldInfo beforeField, bool isOption, Func<int, ConsoleColoredString> helpGenerator, Exception inner)
    : CommandLineParseException(getMessage(paramField, beforeField, isOption), helpGenerator, inner)
{
    /// <summary>Contains the field pertaining to the parameter that was missing.</summary>
    public FieldInfo Field { get; private set; } = paramField;

    /// <summary>Contains an optional reference to a field which the missing parameter must precede.</summary>
    public FieldInfo BeforeField { get; private set; } = beforeField;

    /// <summary>
    ///     Specifies whether the missing parameter was a missing option (true) or a missing positional parameter (false).</summary>
    public bool IsOption { get; private set; } = isOption;

    /// <summary>Constructor.</summary>
    public MissingParameterException(FieldInfo paramField, FieldInfo beforeField, bool isOption, Func<int, ConsoleColoredString> helpGenerator) : this(paramField, beforeField, isOption, helpGenerator, null) { }

    private static ConsoleColoredString getMessage(FieldInfo field, FieldInfo beforeField, bool isOption)
    {
        if (beforeField == null)
            return (isOption ? "The option {0} is mandatory and must be specified." : "The parameter {0} is mandatory and must be specified.").ToConsoleColoredString().Fmt(field.FormatParameterUsage(true));

        return (isOption ? "The option {0} is mandatory and must be specified before the {1} parameter." : "The parameter {0} is mandatory and must be specified before the {1} parameter.").ToConsoleColoredString().Fmt(
            field.FormatParameterUsage(true),
            "<".Color(CmdLineColor.FieldBrackets) + beforeField.Name.Color(CmdLineColor.Field) + ">".Color(CmdLineColor.FieldBrackets));
    }
}

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

/// <summary>
///     Specifies that a parameter that expected a numerical value was passed a string by the user that doesn’t parse as a
///     number.</summary>
[Serializable]
public sealed class InvalidNumericParameterException(string fieldName, Func<int, ConsoleColoredString> helpGenerator, Exception inner)
    : CommandLineParseException("The {0} option expects a number. The specified parameter does not constitute a valid number.".ToConsoleColoredString().Fmt("<".Color(CmdLineColor.FieldBrackets) + fieldName.Color(CmdLineColor.Field) + ">".Color(CmdLineColor.FieldBrackets)), helpGenerator, inner)
{
    /// <summary>Contains the name of the field pertaining to the parameter that was passed an invalid value.</summary>
    public string FieldName { get; private set; } = fieldName;
    /// <summary>Constructor.</summary>
    public InvalidNumericParameterException(string fieldName, Func<int, ConsoleColoredString> helpGenerator) : this(fieldName, helpGenerator, null) { }
}

/// <summary>
///     Indicates that the arguments specified by the user on the command-line do not pass the custom validation check.</summary>
/// <param name="message">
///     Provide a helpful, descriptive message for the user to determine how to provide a valid command-line parameter.</param>
/// <param name="helpGenerator">
///     Used internally to generate the help screen; omit if throwing from a validation check.</param>
[Serializable]
public sealed class CommandLineValidationException(ConsoleColoredString message, Func<int, ConsoleColoredString> helpGenerator = null)
    : CommandLineParseException(message, helpGenerator)
{
}
