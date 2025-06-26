using RT.Util;
using RT.Util.Consoles;

namespace RT.CommandLine;

/// <summary>
///     Use this to specify that a field in a class can be specified on the command line using an option, for example
///     <c>-a</c> or <c>--option-name</c>. The option name(s) MUST begin with a dash (<c>-</c>).</summary>
/// <param name="names">
///     The name of the option. Specify several names as synonyms if required.</param>
[AttributeUsage(AttributeTargets.Field, Inherited = false, AllowMultiple = false), RummageKeepUsersReflectionSafe]
public sealed class OptionAttribute(params string[] names) : Attribute
{
    /// <summary>All of the names of the option.</summary>
    public string[] Names { get; private set; } = names;
}

/// <summary>
///     Use this to specify that a command-line parameter is positional, i.e. is not invoked by an option that starts with
///     "-".</summary>
/// <param name="order">
///     Optionally use this to re-order positional arguments differently from their declaration order.</param>
[AttributeUsage(AttributeTargets.Field, Inherited = false, AllowMultiple = false), RummageKeepUsersReflectionSafe]
public sealed class IsPositionalAttribute(double order = 0) : Attribute
{
    /// <summary>Optionally use this to re-order positional arguments differently from their declaration order.</summary>
    public double Order { get; private set; } = order;
}

/// <summary>Use this to specify that a command-line parameter is mandatory.</summary>
[AttributeUsage(AttributeTargets.Field, Inherited = false, AllowMultiple = false), RummageKeepUsersReflectionSafe]
public sealed class IsMandatoryAttribute() : Attribute
{
}

/// <summary>
///     Specifies that a field of an enum type should be interpreted as multiple possible options, each specified by an <see
///     cref="OptionAttribute"/> on the enum values in the enum type.</summary>
[AttributeUsage(AttributeTargets.Field, Inherited = false, AllowMultiple = false)]
public sealed class EnumOptionsAttribute(EnumBehavior behavior) : Attribute
{
    /// <summary>
    ///     Specifies whether the enum is considered to represent a single value or a bitfield containing multiple values.</summary>
    public EnumBehavior Behavior { get; private set; } = behavior;
}

/// <summary>
///     Use this on a sub-class of an abstract class or on an enum value to specify the command the user must use to invoke
///     that class or enum value.</summary>
/// <param name="names">
///     The command(s) the user can specify to invoke this class or enum value.</param>
[AttributeUsage(AttributeTargets.Class | AttributeTargets.Field, Inherited = false, AllowMultiple = false), RummageKeepUsersReflectionSafe]
public sealed class CommandNameAttribute(params string[] names) : Attribute
{
    /// <summary>The command the user can specify to invoke this class.</summary>
    public string[] Names { get; private set; } = names;
}

/// <summary>Use this on an abstract class to specify that its subclasses represent various commands.</summary>
[AttributeUsage(AttributeTargets.Class, Inherited = false, AllowMultiple = false)]
public sealed class CommandGroupAttribute : Attribute
{
    /// <summary>Constructor.</summary>
    public CommandGroupAttribute() { }
}

/// <summary>
///     Use this attribute to write the help text that describes (documents) a command-line option or command. Specifying
///     multiple strings will create multiple columns in the table. See Remarks.</summary>
/// <remarks>
///     This attribute specifies the documentation in plain text. All characters are printed exactly as specified. You may
///     wish to use <see cref="DocumentationRhoMLAttribute"/> to specify documentation with special markup for
///     command-line-related concepts, as well as <see cref="DocumentationEggsMLAttribute"/> for an alternative markup
///     language without command-line specific concepts.</remarks>
[AttributeUsage(AttributeTargets.Field | AttributeTargets.Class | AttributeTargets.Method, Inherited = false, AllowMultiple = true), RummageKeepUsersReflectionSafe]
public class DocumentationAttribute(params string[] documentation) : Attribute
{
    /// <summary>
    ///     Gets the console-colored documentation string. Multiple strings may be returned to create multiple columns. Note
    ///     that this property may throw if the text couldn't be parsed where applicable.</summary>
    public virtual ConsoleColoredString[] Texts => _converted ??= OriginalTexts.Select(s => (ConsoleColoredString) s).ToArray();
    private ConsoleColoredString[] _converted;
    /// <summary>Gets a string describing the documentation format to the programmer (not seen by the users).</summary>
    public virtual string OriginalFormat => "Plain text";
    /// <summary>Gets the original documentation string exactly as specified in the attribute.</summary>
    public string[] OriginalTexts { get; private set; } = documentation;
}

/// <summary>
///     This is a legacy attribute. Do not use in new programs. This attribute is equivalent to <see
///     cref="DocumentationEggsMLAttribute"/>.</summary>
[AttributeUsage(AttributeTargets.Field | AttributeTargets.Class | AttributeTargets.Method, Inherited = false, AllowMultiple = false), RummageKeepUsersReflectionSafe]
public class DocumentationLiteralAttribute(string documentation) : DocumentationEggsMLAttribute(documentation)
{
}

/// <summary>
///     Use this attribute to write the help text that describes (documents) a command-line option or command. Specifying
///     multiple strings will create multiple columns in the table. The documentation is to be specified in <see
///     cref="EggsML"/>, which is interpreted as described in <see cref="CommandLineParser.Colorize(EggsNode)"/>.</summary>
/// <seealso cref="DocumentationAttribute"/>
/// <seealso cref="DocumentationRhoMLAttribute"/>
[AttributeUsage(AttributeTargets.Field | AttributeTargets.Class | AttributeTargets.Method, Inherited = false, AllowMultiple = true), RummageKeepUsersReflectionSafe]
public class DocumentationEggsMLAttribute(params string[] documentation) : DocumentationAttribute(documentation)
{
    /// <summary>Gets a string describing the documentation format to the programmer (not seen by the users).</summary>
    public override string OriginalFormat => "EggsML";
    /// <summary>
    ///     Gets the console-colored documentation string. Note that this property may throw if the text couldn't be parsed
    ///     where applicable.</summary>
    public override ConsoleColoredString[] Texts => _parsed ??= OriginalTexts.Select(text => CommandLineParser.Colorize(EggsML.Parse(text))).ToArray();
    private ConsoleColoredString[] _parsed;
}

/// <summary>
///     Use this attribute to write the help text that describes (documents) a command-line option or command. Specifying
///     multiple strings will create multiple columns in the table. The documentation is to be specified in <see
///     cref="RhoML"/>, which is interpreted as described in <see cref="CommandLineParser.Colorize(RhoElement)"/>.</summary>
/// <seealso cref="DocumentationAttribute"/>
/// <seealso cref="DocumentationEggsMLAttribute"/>
[AttributeUsage(AttributeTargets.Field | AttributeTargets.Class | AttributeTargets.Method, Inherited = false, AllowMultiple = false), RummageKeepUsersReflectionSafe]
public class DocumentationRhoMLAttribute(string documentation) : DocumentationAttribute(documentation)
{
    /// <summary>Gets a string describing the documentation format to the programmer (not seen by the users).</summary>
    public override string OriginalFormat => "RhoML";
    /// <summary>
    ///     Gets the console-colored documentation string. Note that this property may throw if the text couldn't be parsed
    ///     where applicable.</summary>
    public override ConsoleColoredString[] Texts => _parsed ??= OriginalTexts.Select(text => CommandLineParser.Colorize(RhoML.Parse(text))).ToArray();
    private ConsoleColoredString[] _parsed;
}

/// <summary>
///     Specifies that a specific command-line option should not be printed in help pages, i.e. the option should explicitly
///     be undocumented.</summary>
[AttributeUsage(AttributeTargets.Field | AttributeTargets.Class, Inherited = false, AllowMultiple = true)]
public sealed class UndocumentedAttribute() : Attribute
{
}

/// <summary>Adds a section header on the help screen above the option or parameter that has this attribute.</summary>
[AttributeUsage(AttributeTargets.Field, Inherited = false, AllowMultiple = true)]
public sealed class SectionAttribute(string heading) : Attribute
{
    /// <summary>Specifies the section heading.</summary>
    public string Heading { get; private set; } = heading;
}

/// <summary>
///     Optionally use this on a class containing command-line options (either the main class or a class representing a
///     subcommand) to specify some formatting parameters for the help screen.</summary>
[AttributeUsage(AttributeTargets.Class, Inherited = false, AllowMultiple = false), RummageKeepUsersReflectionSafe]
public sealed class HelpScreenFormattingAttribute : Attribute
{
    /// <summary>Specifies the amount of horizontal separation between the columns in the table(s).</summary>
    public int ColumnSpacing { get; set; } = 3;
    /// <summary>Specifies the amount of blank lines between the rows in the table(s).</summary>
    public int RowSpacing { get; set; } = 1;
    /// <summary>Specifies the amount of blank lines before a section header.</summary>
    public int BlankLinesBeforeSection { get; set; } = 1;
    /// <summary>Specifies the amount of blank lines after a section header.</summary>
    public int BlankLinesAfterSection { get; set; } = 1;
    /// <summary>Specifies the amount of margin to the left of the table(s).</summary>
    public int LeftMargin { get; set; } = 3;
}
