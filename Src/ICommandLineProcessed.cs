namespace RT.CommandLine;

/// <summary>
///     Contains methods to post-process a class representing command-line options as populated by <see
///     cref="CommandLineParser"/>.</summary>
/// <remarks>
///     If an input doesn’t parse correctly, throw <see cref="CommandLineValidationException"/> with a helpful, descriptive
///     message that is displayed to the user.</remarks>
public interface ICommandLineProcessed
{
    /// <summary>
    ///     When implemented in a class, performs application-specific post-processing of the options class.</summary>
    /// <remarks>
    ///     When <see cref="CommandLineParser"/> invokes this method, all parsed commands and options have already been
    ///     populated. This method can thus alter the class in application-specific ways, perform further parsing, or further
    ///     validate the options for constraints such as mutual exclusivity. To report a validation error, this method should
    ///     throw a <see cref="CommandLineValidationException"/> with a helpful, descriptive message that is displayed to the
    ///     user.</remarks>
    void Process();
}
