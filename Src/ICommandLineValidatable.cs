using RT.Util.Consoles;

namespace RT.CommandLine;

/// <summary>
///     Contains methods to validate and post-process a class representing command-line options as populated by <see
///     cref="CommandLineParser"/>.</summary>
public interface ICommandLineValidatable
{
    /// <summary>
    ///     When implemented in a class, returns an error message if the contents of the class are invalid, otherwise returns
    ///     null.</summary>
    /// <remarks>
    ///     <para>
    ///         When <see cref="CommandLineParser"/> invokes this method, all parsed commands and options have already been
    ///         populated. This method can thus alter the class in application-specific ways, perform further parsing, or
    ///         further validate the options for constraints such as mutual exclusivity.</para>
    ///     <para>
    ///         To report a validation error, this method can either return the text of the error message, or throw a <see
    ///         cref="CommandLineValidationException"/>. The message is passed to the user in the same way as other parse
    ///         errors (for example, see <see cref="CommandLineParser.ParseOrWriteUsageToConsole"/> which prints it to the
    ///         console).</para></remarks>
    ConsoleColoredString Validate();
}
