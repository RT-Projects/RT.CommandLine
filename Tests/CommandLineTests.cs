using Xunit;

namespace RT.CommandLine.Tests;

public sealed class CmdLineTests
{
#pragma warning disable 0649 // Field is never assigned to, and will always have its default value null
    private class CommandLine1
    {
        [IsPositional, IsMandatory]
        public string Arg;
        [Option("-o")]
        public string Option;
    }
    private class CommandLine2
    {
        [Option("--stuff")]
        public string Stuff;
        [IsPositional]
        public string[] Args;
    }
#pragma warning restore 0649 // Field is never assigned to, and will always have its default value null

    [Fact]
    public static void Test()
    {
        try
        {
            CommandLineParser.Parse<CommandLine1>([]);
            Assert.Fail();
        }
        catch (CommandLineParseException e)
        {
            Assert.Equal("The parameter <Arg> is mandatory and must be specified.", e.Message);
        }
        try
        {
            CommandLineParser.Parse<CommandLine1>(["-o", "Option"]);
            Assert.Fail();
        }
        catch (CommandLineParseException e)
        {
            Assert.Equal("The parameter <Arg> is mandatory and must be specified.", e.Message);
        }

        var c1 = CommandLineParser.Parse<CommandLine1>(["Arg"]);
        Assert.Equal("Arg", c1.Arg);
        Assert.Null(c1.Option);

        c1 = CommandLineParser.Parse<CommandLine1>(["-o", "Arg1", "Arg2"]);
        Assert.Equal("Arg1", c1.Option);
        Assert.Equal("Arg2", c1.Arg);

        c1 = CommandLineParser.Parse<CommandLine1>(["Arg1", "-o", "Arg2"]);
        Assert.Equal("Arg1", c1.Arg);
        Assert.Equal("Arg2", c1.Option);

        c1 = CommandLineParser.Parse<CommandLine1>(["--", "-o"]);
        Assert.Equal("-o", c1.Arg);
        Assert.Null(c1.Option);


        var c2 = CommandLineParser.Parse<CommandLine2>("--stuff blah abc def".Split(' '));
        Assert.Equal("blah", c2.Stuff);
        Assert.True(c2.Args.SequenceEqual(["abc", "def"]));

        c2 = CommandLineParser.Parse<CommandLine2>("def --stuff thingy abc".Split(' '));
        Assert.Equal("thingy", c2.Stuff);
        Assert.True(c2.Args.SequenceEqual(["def", "abc"]));

        c2 = CommandLineParser.Parse<CommandLine2>("--stuff stuff -- abc --stuff blah -- def".Split(' '));
        Assert.Equal("stuff", c2.Stuff);
        Assert.True(c2.Args.SequenceEqual(["abc", "--stuff", "blah", "--", "def"]));

        try
        {
            CommandLineParser.Parse<CommandLine2>("--blah stuff".Split(' '));
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
}
