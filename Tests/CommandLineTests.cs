using RT.Util.Consoles;
using Xunit;

namespace RT.CommandLine.Tests;

#pragma warning disable 0649 // Field is never assigned to, and will always have its default value null

public sealed class CmdLineTests
{
    private class CommandLineWithOption
    {
        [IsPositional, IsMandatory]
        public string Arg;
        [Option("-o")]
        public string Option;
    }
    private class CommandLineWithArray
    {
        [Option("--stuff")]
        public string Stuff;
        [IsPositional]
        public string[] Args;
    }

    [Fact]
    public static void TestMinusMinus()
    {
        try
        {
            CommandLineParser.Parse<CommandLineWithOption>([]);
            Assert.Fail();
        }
        catch (CommandLineParseException e)
        {
            Assert.Equal("The parameter <Arg> is mandatory and must be specified.", e.Message);
        }
        try
        {
            CommandLineParser.Parse<CommandLineWithOption>(["-o", "Option"]);
            Assert.Fail();
        }
        catch (CommandLineParseException e)
        {
            Assert.Equal("The parameter <Arg> is mandatory and must be specified.", e.Message);
        }

        var c = CommandLineParser.Parse<CommandLineWithOption>(["Arg"]);
        Assert.Equal("Arg", c.Arg);
        Assert.Null(c.Option);

        c = CommandLineParser.Parse<CommandLineWithOption>(["-o", "Arg1", "Arg2"]);
        Assert.Equal("Arg1", c.Option);
        Assert.Equal("Arg2", c.Arg);

        c = CommandLineParser.Parse<CommandLineWithOption>(["Arg1", "-o", "Arg2"]);
        Assert.Equal("Arg1", c.Arg);
        Assert.Equal("Arg2", c.Option);

        c = CommandLineParser.Parse<CommandLineWithOption>(["--", "-o"]);
        Assert.Equal("-o", c.Arg);
        Assert.Null(c.Option);
    }

    [Fact]
    public static void TestArrays()
    {
        var c = CommandLineParser.Parse<CommandLineWithArray>("--stuff blah abc def".Split(' '));
        Assert.Equal("blah", c.Stuff);
        Assert.True(c.Args.SequenceEqual(["abc", "def"]));

        c = CommandLineParser.Parse<CommandLineWithArray>("def --stuff thingy abc".Split(' '));
        Assert.Equal("thingy", c.Stuff);
        Assert.True(c.Args.SequenceEqual(["def", "abc"]));

        c = CommandLineParser.Parse<CommandLineWithArray>("--stuff stuff -- abc --stuff blah -- def".Split(' '));
        Assert.Equal("stuff", c.Stuff);
        Assert.True(c.Args.SequenceEqual(["abc", "--stuff", "blah", "--", "def"]));
    }

    [Fact]
    public static void TestInvalidOption()
    {
        try
        {
            CommandLineParser.Parse<CommandLineWithArray>("--blah stuff".Split(' '));
            Assert.Fail();
        }
        catch (CommandLineParseException e)
        {
            Assert.Equal("The specified command or option, --blah, is not recognized.", e.Message);
            Assert.Equal("The specified command or option, --blah, is not recognized.", e.GetColoredMessage().ToString());
        }
    }


    class Test1Cmd : ICommandLineValidatable
    {
        [IsPositional, IsMandatory]
        public string Base;

        [IsPositional, IsMandatory]
        public Test1SubcommandBase Subcommand;

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

    [Fact]
    public static void TestSubcommandValidation()
    {
        Test1Cmd.ValidateCalled = 0;
        Test1SubcommandBase.ValidateCalled = 0;
        var c = CommandLineParser.Parse<Test1Cmd>(["base", "sub1", "item"]);
        Assert.Equal("base", c.Base);
        Assert.IsType<Test1Subcommand1>(c.Subcommand);
        var cs1 = (Test1Subcommand1) c.Subcommand;
        Assert.Equal("item", cs1.ItemName);
        Assert.Equal(1, Test1Cmd.ValidateCalled);
        Assert.Equal(1, Test1SubcommandBase.ValidateCalled);

        Test1Cmd.ValidateCalled = 0;
        Test1SubcommandBase.ValidateCalled = 0;
        var c2 = CommandLineParser.Parse<Test1Cmd>(["base", "sub2"]);
        Assert.Equal("base", c2.Base);
        Assert.IsType<Test1Subcommand2>(c2.Subcommand);
        Assert.Equal(1, Test1Cmd.ValidateCalled);
        Assert.Equal(0, Test1SubcommandBase.ValidateCalled);
    }
}
