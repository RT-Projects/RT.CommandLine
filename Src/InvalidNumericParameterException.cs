using RT.Util.Consoles;
using RT.Util.ExtensionMethods;

namespace RT.CommandLine;

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
