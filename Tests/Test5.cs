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
