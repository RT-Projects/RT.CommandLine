using RT.Util.Consoles;

namespace RT.CommandLine.Tests;

#pragma warning disable CS0649 // Field is never assigned to and will always have its default value

class Test1Cmd : ICommandLineProcessed
{
    [IsPositional, IsMandatory]
    public string Base;

    [IsPositional, IsMandatory]
    public Test1SubcommandBase Subcommand;

    public static int ValidateCalled = 0;

    public void Process()
    {
        ValidateCalled++;
    }
}

[CommandGroup]
abstract class Test1SubcommandBase : ICommandLineProcessed
{
    public static int ValidateCalled = 0;
    public abstract void Process();
}

[CommandName("sub1")]
sealed class Test1Subcommand1 : Test1SubcommandBase
{
    [IsPositional, IsMandatory]
    public string ItemName;

    public override void Process()
    {
        ValidateCalled++;
    }
}

[CommandName("sub2")]
sealed class Test1Subcommand2 : Test1SubcommandBase
{
    public override void Process() { }
}
