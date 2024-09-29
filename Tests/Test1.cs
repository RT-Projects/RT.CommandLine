using RT.Util.Consoles;

namespace RT.CommandLine.Tests;

#pragma warning disable CS0649 // Field is never assigned to and will always have its default value

class Test1Cmd : ICommandLineValidatable
{
    [IsPositional, IsMandatory]
    public string Base;

    [IsPositional, IsMandatory]
    public Test1SubcommandBase Subcommand;

    public static int ValidateCalled = 0;

    public void Validate()
    {
        ValidateCalled++;
    }
}

[CommandGroup]
abstract class Test1SubcommandBase : ICommandLineValidatable
{
    public static int ValidateCalled = 0;
    public abstract void Validate();
}

[CommandName("sub1")]
sealed class Test1Subcommand1 : Test1SubcommandBase
{
    [IsPositional, IsMandatory]
    public string ItemName;

    public override void Validate()
    {
        ValidateCalled++;
    }
}

[CommandName("sub2")]
sealed class Test1Subcommand2 : Test1SubcommandBase
{
    public override void Validate() { }
}
