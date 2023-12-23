using Xunit;

namespace RT.CommandLine.Tests;

public sealed class CmdLineTests
{
#pragma warning disable 0649 // Field is never assigned to, and will always have its default value null
    private class commandLine
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
        var c = CommandLineParser.Parse<commandLine>("--stuff blah abc def".Split(' '));
        Assert.Equal("blah", c.Stuff);
        Assert.True(c.Args.SequenceEqual(new[] { "abc", "def" }));

        c = CommandLineParser.Parse<commandLine>("def --stuff thingy abc".Split(' '));
        Assert.Equal("thingy", c.Stuff);
        Assert.True(c.Args.SequenceEqual(new[] { "def", "abc" }));

        c = CommandLineParser.Parse<commandLine>("--stuff stuff -- abc --stuff blah -- def".Split(' '));
        Assert.Equal("stuff", c.Stuff);
        Assert.True(c.Args.SequenceEqual(new[] { "abc", "--stuff", "blah", "--", "def" }));
    }
}
