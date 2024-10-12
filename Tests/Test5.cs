namespace RT.CommandLine.Tests;

#pragma warning disable CS0649 // Field is never assigned to and will always have its default value

class Test5Cmd1
{
    // Mandatory before optional should be allowed
    [IsPositional(1)] public int One = 47;
    [IsPositional(0), IsMandatory] public int Two;
}

class Test5Cmd2
{
    // Optional before mandatory should trigger an error
    [IsPositional(1), IsMandatory] public int One;
    [IsPositional(0)] public int Two;
}

class Test5Cmd3
{
    [IsPositional, IsMandatory] public Test5Cmd3_CmdBase Cmd;
}

[CommandGroup]
abstract class Test5Cmd3_CmdBase
{
    [IsPositional(1), IsMandatory] public string One;
    [IsPositional(1), IsMandatory] public Test5Cmd3_SubCmdBase Sub;
}

[CommandName("cmd")]
class Test5Cmd3_Cmd : Test5Cmd3_CmdBase
{
    [IsPositional(0), IsMandatory] public string Two;
}

[CommandGroup]
abstract class Test5Cmd3_SubCmdBase { }
[CommandName("sub")]
class Test5Cmd3_SubCmd : Test5Cmd3_SubCmdBase { }
