using System.Reflection;
using RT.Util.Consoles;
using RT.Util.ExtensionMethods;

namespace RT.CommandLine;

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
