namespace RT.CommandLine;

internal static class CmdLineColor
{
    public const ConsoleColor Option = ConsoleColor.Yellow;
    public const ConsoleColor FieldBrackets = ConsoleColor.DarkCyan;
    public const ConsoleColor Field = ConsoleColor.Cyan;
    public const ConsoleColor Command = ConsoleColor.Green;
    public const ConsoleColor EnumValue = ConsoleColor.Green;
    public const ConsoleColor UsageLinePrefix = ConsoleColor.Green;
    public const ConsoleColor OptionalityDelimiters = ConsoleColor.DarkGray; // e.g. [foo|bar] has [, ] and | in this color
    public const ConsoleColor SubcommandsPresentAsterisk = ConsoleColor.DarkYellow;
    public const ConsoleColor UnexpectedArgument = ConsoleColor.Magenta;
    public const ConsoleColor Error = ConsoleColor.Red;
    public const ConsoleColor HelpHeading = ConsoleColor.White;
    public const ConsoleColor Highlight = ConsoleColor.White;
}
