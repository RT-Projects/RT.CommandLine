using RT.Util.Consoles;

namespace RT.CommandLine.Tests;

#pragma warning disable CS0649 // Field is never assigned to and will always have its default value

class Test1Cmd : ICommandLineValidatable
{
    [IsPositional, IsMandatory]
    public string Base;

    [IsPositional, IsMandatory]
    public Test1SubcommandBase Subcommand;

    [Ignore]
    public static int ValidateCalled = 0;

    public ConsoleColoredString Validate()
    {
        ValidateCalled++;
        return null;
    }
}

[CommandGroup]
abstract class Test1SubcommandBase : ICommandLineValidatable
{
    public static int ValidateCalled = 0;
    public abstract ConsoleColoredString Validate();
}

[CommandName("sub1")]
sealed class Test1Subcommand1 : Test1SubcommandBase
{
    [IsPositional, IsMandatory]
    public string ItemName;

    public override ConsoleColoredString Validate()
    {
        ValidateCalled++;
        return null;
    }
}

[CommandName("sub2")]
sealed class Test1Subcommand2 : Test1SubcommandBase
{
    public override ConsoleColoredString Validate() { return null; }
}
