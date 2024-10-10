namespace RT.CommandLine.Tests;

#pragma warning disable CS0649 // Field is never assigned to and will always have its default value

class Test4Cmd1
{
    // Expected order: one, two, three
    [IsPositional, IsMandatory] public int One;
    [IsPositional, IsMandatory] public int Two;
    [IsPositional, IsMandatory] public int Three;
}

class Test4Cmd2
{
    // Expected order: three, one, two
    [IsPositional(1), IsMandatory] public int One;
    [IsPositional(1), IsMandatory] public int Two;
    [IsPositional(0), IsMandatory] public int Three;
}

class Test4Cmd3
{
    // Expected order: two, one, three
    [IsPositional(1), IsMandatory] public int One;
    [IsPositional(0), IsMandatory] public int Two;
    [IsPositional(1), IsMandatory] public int Three;
}
