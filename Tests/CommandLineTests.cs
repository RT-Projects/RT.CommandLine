using RT.Util.Consoles;
using Xunit;

namespace RT.CommandLine.Tests;

public sealed class CmdLineTests
{
#pragma warning disable 0649 // Field is never assigned to, and will always have its default value null
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

    [CommandLine]
    abstract class CmdBase1 : ICommandLineValidatable
    {
        [IsPositional, IsMandatory]
        public string Base;

        public static int ValidateCalled = 0;

        public ConsoleColoredString Validate()
        {
            ValidateCalled++;
            return null;
        }
    }

    [CommandName("sub")]
    sealed class CmdSubcmd1 : CmdBase1
    {
        [IsPositional, IsMandatory]
        public string ItemName;
    }

    [CommandLine]
    abstract class CmdBase2
    {
        [IsPositional, IsMandatory]
        public string Base;
    }

    [CommandName("sub")]
    sealed class CmdSubcmd2 : CmdBase2, ICommandLineValidatable
    {
        [IsPositional, IsMandatory]
        public string ItemName;

        public static int ValidateCalled = 0;

        public ConsoleColoredString Validate()
        {
            ValidateCalled++;
            return null;
        }
    }

    [CommandLine]
    abstract class CmdBase3
    {
        [IsPositional, IsMandatory]
        public string Base;
    }

    abstract class CmdSubBase3 : CmdBase3
    {
        [IsPositional, IsMandatory]
        public string SubBase;
    }

    [CommandName("sub")]
    sealed class CmdSubcmd3 : CmdSubBase3
    {
        [IsPositional, IsMandatory]
        public string ItemName;
    }
#pragma warning restore 0649 // Field is never assigned to, and will always have its default value null

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
    public static void TestInvalidOptionAndLingo()
    {
        try
        {
            CommandLineParser.Parse<CommandLineWithArray>("--blah stuff".Split(' '));
            Assert.Fail();
        }
        catch (CommandLineParseException e)
        {
            Assert.Equal("The specified command or option, --blah, is not recognized.", e.Message);
            Assert.Equal("The specified command or option, --blah, is not recognized.", e.GetColoredMessage(new Translation()).ToString());
            var tr = new Translation();
            tr.UnrecognizedCommandOrOption = "Неизвестная опция или команда: {0}.";
            Assert.Equal("Неизвестная опция или команда: --blah.", e.GetColoredMessage(tr).ToString());
        }
    }

    [Fact]
    public static void TestSubcommandValidation()
    {
        CmdBase1.ValidateCalled = 0;
        var c = CommandLineParser.Parse<CmdBase1>(["base", "sub", "item"]);
        Assert.IsType<CmdSubcmd1>(c);
        var cs1 = (CmdSubcmd1) c;
        Assert.Equal("base", cs1.Base);
        Assert.Equal("item", cs1.ItemName);
        Assert.Equal(1, CmdBase1.ValidateCalled);

        CmdSubcmd2.ValidateCalled = 0;
        var c2 = CommandLineParser.Parse<CmdBase2>(["base", "sub", "item"]);
        Assert.IsType<CmdSubcmd2>(c2);
        var cs2 = (CmdSubcmd2) c2;
        Assert.Equal("base", cs2.Base);
        Assert.Equal("item", cs2.ItemName);
        Assert.Equal(1, CmdSubcmd2.ValidateCalled);
    }

    [Fact]
    public static void TestSubcommandIntermediate()
    {
        var c3 = CommandLineParser.Parse<CmdBase3>(["base", "sub", "subbase", "item"]);
        Assert.IsType<CmdSubcmd3>(c3);
        var cs3 = (CmdSubcmd3) c3;
        Assert.Equal("base", cs3.Base);
        Assert.Equal("subbase", cs3.SubBase);
        Assert.Equal("item", cs3.ItemName);
    }
}
