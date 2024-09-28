using RT.Util;
using RT.Util.Consoles;

namespace RT.CommandLine;

/// <summary>
///     Use this attribute to link a command-line option or command with the help text that describes (documents) it. Suitable
///     for single-language applications only. The documentation is to be specified in <see cref="EggsML"/>, which is
///     interpreted as described in <see cref="CommandLineParser.Colorize(EggsNode)"/>. See also <see
///     cref="DocumentationRhoMLAttribute"/> and <see cref="DocumentationAttribute"/>.</summary>
[AttributeUsage(AttributeTargets.Field | AttributeTargets.Class | AttributeTargets.Method, Inherited = false, AllowMultiple = false), RummageKeepUsersReflectionSafe]
public class DocumentationEggsMLAttribute(string documentation) : DocumentationAttribute(documentation)
{
    /// <summary>Gets a string describing the documentation format to the programmer (not seen by the users).</summary>
    public override string OriginalFormat { get { return "EggsML"; } }
    /// <summary>
    ///     Gets the console-colored documentation string. Note that this property may throw if the text couldn't be parsed
    ///     where applicable.</summary>
    public override ConsoleColoredString Text => _parsed ??= CommandLineParser.Colorize(EggsML.Parse(OriginalText));
    private ConsoleColoredString _parsed;
}
