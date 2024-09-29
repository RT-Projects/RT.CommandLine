﻿using RT.Util;
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
[AttributeUsage(AttributeTargets.Field, Inherited = false, AllowMultiple = false), RummageKeepUsersReflectionSafe]
public sealed class IsPositionalAttribute : Attribute
{
    /// <summary>Constructor.</summary>
    public IsPositionalAttribute() { }
}

/// <summary>Use this to specify that a command-line parameter is mandatory.</summary>
[AttributeUsage(AttributeTargets.Field, Inherited = false, AllowMultiple = false), RummageKeepUsersReflectionSafe]
public sealed class IsMandatoryAttribute() : Attribute
{
}

/// <summary>Specifies that the command-line parser should ignore a field.</summary>
[AttributeUsage(AttributeTargets.Field, Inherited = false, AllowMultiple = false)]
public sealed class IgnoreAttribute : Attribute
{
    /// <summary>Constructor.</summary>
    public IgnoreAttribute() { }
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

/// <summary>
///     This is a legacy attribute. Do not use in new programs. This attribute is equivalent to <see
///     cref="DocumentationEggsMLAttribute"/>.</summary>
[AttributeUsage(AttributeTargets.Field | AttributeTargets.Class | AttributeTargets.Method, Inherited = false, AllowMultiple = false), RummageKeepUsersReflectionSafe]
public class DocumentationLiteralAttribute(string documentation) : DocumentationEggsMLAttribute(documentation)
{
}

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

/// <summary>
///     Use this attribute to link a command-line option or command with the help text that describes (documents) it. Suitable
///     for single-language applications only. The documentation is to be specified in <see cref="RhoML"/>, which is
///     interpreted as described in <see cref="CommandLineParser.Colorize(RhoElement)"/>. See also <see
///     cref="DocumentationAttribute"/>.</summary>
[AttributeUsage(AttributeTargets.Field | AttributeTargets.Class | AttributeTargets.Method, Inherited = false, AllowMultiple = false), RummageKeepUsersReflectionSafe]
public class DocumentationRhoMLAttribute(string documentation) : DocumentationAttribute(documentation)
{
    /// <summary>Gets a string describing the documentation format to the programmer (not seen by the users).</summary>
    public override string OriginalFormat { get { return "RhoML"; } }
    /// <summary>
    ///     Gets the console-colored documentation string. Note that this property may throw if the text couldn't be parsed
    ///     where applicable.</summary>
    public override ConsoleColoredString Text => _parsed ??= CommandLineParser.Colorize(RhoML.Parse(OriginalText));
    private ConsoleColoredString _parsed;
}

/// <summary>
///     Specifies that a specific command-line option should not be printed in help pages, i.e. the option should explicitly
///     be undocumented.</summary>
[AttributeUsage(AttributeTargets.Field | AttributeTargets.Class, Inherited = false, AllowMultiple = true)]
public sealed class UndocumentedAttribute : Attribute
{
    /// <summary>Constructor.</summary>
    public UndocumentedAttribute() { }
}