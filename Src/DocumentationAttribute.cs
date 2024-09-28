using RT.Util;
using RT.Util.Consoles;

namespace RT.CommandLine;

/// <summary>
///     Use this attribute to link a command-line option or command with the help text that describes (documents) it. Suitable
///     for single-language applications only. See Remarks.</summary>
/// <remarks>
///     This attribute specifies the documentation in plain text. All characters are printed exactly as specified. You may
///     wish to use <see cref="DocumentationRhoMLAttribute"/> to specify documentation with special markup for
///     command-line-related concepts, as well as <see cref="DocumentationEggsMLAttribute"/> for an alternative markup
///     language without command-line specific concepts.</remarks>
[AttributeUsage(AttributeTargets.Field | AttributeTargets.Class | AttributeTargets.Method, Inherited = false, AllowMultiple = false), RummageKeepUsersReflectionSafe]
public class DocumentationAttribute(string documentation) : Attribute
{
    /// <summary>
    ///     Gets the console-colored documentation string. Note that this property may throw if the text couldn't be parsed
    ///     where applicable.</summary>
    public virtual ConsoleColoredString Text => OriginalText;
    /// <summary>Gets a string describing the documentation format to the programmer (not seen by the users).</summary>
    public virtual string OriginalFormat => "Plain text";
    /// <summary>Gets the original documentation string exactly as specified in the attribute.</summary>
    public string OriginalText { get; private set; } = documentation;
}
