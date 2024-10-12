using RT.Json;
using RT.PostBuild;
using RT.Serialization;
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
            Assert.Equal("The specified command or option, --blah, is not recognized.", e.ColoredMessage.ToString());
        }
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

    [Fact]
    public static void TestMore()
    {
        Assert.True(
            JsonValue.Parse(@"{""Boolean"":true,""Subcommand"":{""SharedString"":null,""Name"":""this"","":type"":""Test2SubcommandAdd""}}")
            == ClassifyJson.Serialize(CommandLineParser.Parse<Test2Cmd>(["-b", "add", "this"])));

        Assert.True(
            JsonValue.Parse(@"{""Boolean"":false,""Subcommand"":{""SharedString"":null,""Id"":""this"","":type"":""Test2SubcommandDelete""}}")
            == ClassifyJson.Serialize(CommandLineParser.Parse<Test2Cmd>(["del", "this"])));
    }

    [Fact]
    public static void TestSubcommandIntermediate()
    {
        var c3 = CommandLineParser.Parse<Test3Cmd>(["sub", "base", "subbase", "item"]);
        Assert.IsType<Test3SubCmd>(c3.Cmd);
        var cs3 = (Test3SubCmd) c3.Cmd;
        Assert.Equal("base", cs3.Base);
        Assert.Equal("subbase", cs3.SubBase);
        Assert.Equal("item", cs3.ItemName);
    }

    [Fact]
    public static void TestPostBuild()
    {
        var reporter = new Reporter();
        CommandLineParser.PostBuildStep<CommandLineWithOption>(reporter);
        CommandLineParser.PostBuildStep<CommandLineWithArray>(reporter);
        CommandLineParser.PostBuildStep<Test1Cmd>(reporter);
        CommandLineParser.PostBuildStep<Test2Cmd>(reporter);
        CommandLineParser.PostBuildStep<Test3Cmd>(reporter);
        CommandLineParser.PostBuildStep<Test5Cmd3>(reporter);
    }

    [Fact]
    public static void TestPositionalOrder()
    {
        static void Test<T>(string helpPart, int[] oneTwoThree)
        {
            try { CommandLineParser.Parse<T>([]); }
            catch (CommandLineParseException e) { Assert.Matches($@"\AUsage: .* {helpPart}", e.GenerateHelp().ToString()); }
            dynamic cmd = CommandLineParser.Parse<T>(["1", "2", "3"]);
            Assert.Equal(oneTwoThree[0], (int) cmd.One);
            Assert.Equal(oneTwoThree[1], (int) cmd.Two);
            Assert.Equal(oneTwoThree[2], (int) cmd.Three);
        }

        Test<Test4Cmd1>("<One> <Two> <Three>", [1, 2, 3]);
        Test<Test4Cmd2>("<Three> <One> <Two>", [2, 3, 1]);
        Test<Test4Cmd3>("<Two> <One> <Three>", [2, 1, 3]);
    }

    [Fact]
    public static void TestPositionalMandatory()
    {
        // Mandatory, then optional — allowed
        var cmd = CommandLineParser.Parse<Test5Cmd1>(["1", "2"]);
        Assert.Equal(2, cmd.One);
        Assert.Equal(1, cmd.Two);
        var cmd2 = CommandLineParser.Parse<Test5Cmd1>(["8472"]);
        Assert.Equal(47, cmd2.One);
        Assert.Equal(8472, cmd2.Two);

        // Optional, then mandatory — expect exception
        var exc = Assert.Throws<InvalidOrderOfPositionalParametersException>(() => CommandLineParser.Parse<Test5Cmd2>(["1", "2"]));
        Assert.Equal("The positional parameter <Test5Cmd2.Two> is optional, but is followed by positional parameter <Test5Cmd2.One> which is mandatory. Either mark <Test5Cmd2.Two> as mandatory or <Test5Cmd2.One> as optional.", exc.Message);
    }

    class Reporter : IPostBuildReporter
    {
        public void Error(string message, params string[] tokens) => throw new Exception(message);
        public void Error(string message, string filename, int lineNumber, int? columnNumber = null) => throw new Exception(message);
        public void Warning(string message, params string[] tokens) { }
        public void Warning(string message, string filename, int lineNumber, int? columnNumber = null) { }
    }
}
