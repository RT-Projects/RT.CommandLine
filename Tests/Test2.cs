namespace RT.CommandLine.Tests;

class Test2Cmd
{
    [Option("-b")]
    public bool Boolean;

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
