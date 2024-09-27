namespace RT.CommandLine.Tests;

#pragma warning disable CS0649 // Field is never assigned to and will always have its default value

class Test2Cmd
{
    [Option("-b")]
    public bool Boolean;
    [IsPositional]
    public Test2Subcommand Subcommand;
}

[CommandGroup]
abstract class Test2Subcommand
{
    [Option("-k")]
    public string SharedString;
}

[CommandName("add")]
class Test2SubcommandAdd : Test2Subcommand
{
    [IsPositional, IsMandatory]
    public string Name;
}

[CommandName("del")]
class Test2SubcommandDelete : Test2Subcommand
{
    [IsPositional, IsMandatory]
    public string Id;
}
