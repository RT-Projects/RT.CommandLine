namespace RT.CommandLine.Tests;

class Test3Cmd
{
    [IsPositional, IsMandatory]
    public Test3CmdBase Cmd;
}

[CommandGroup]
abstract class Test3CmdBase
{
    [IsPositional, IsMandatory]
    public string Base;
}

abstract class Test3CmdSubBase : Test3CmdBase
{
    [IsPositional, IsMandatory]
    public string SubBase;
}

[CommandName("sub")]
sealed class Test3SubCmd : Test3CmdSubBase
{
    [IsPositional, IsMandatory]
    public string ItemName;
}
