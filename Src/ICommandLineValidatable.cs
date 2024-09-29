using RT.Util.Consoles;

namespace RT.CommandLine;

/// <summary>
///     Contains methods to validate a set of parameters passed by the user on the command-line and parsed by <see
///     cref="CommandLineParser"/>.</summary>
/// <remarks>
///     If an input doesn’t parse correctly, throw <see cref="CommandLineValidationException"/> with a helpful, descriptive
///     message that is displayed to the user.</remarks>
public interface ICommandLineValidatable
{
    /// <summary>
    ///     When overridden in a derived class, returns an error message if the contents of the class are invalid, otherwise
    ///     returns null.</summary>
    void Validate();
}
