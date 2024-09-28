namespace RT.CommandLine;

/// <summary>Describes the behavior of an enum-typed field with the <see cref="EnumOptionsAttribute"/>.</summary>
public enum EnumBehavior
{
    /// <summary>Specifies that an enum is considered to represent a single value.</summary>
    SingleValue,
    /// <summary>Specifies that an enum is considered to represent a bitfield containing multiple values.</summary>
    MultipleValues
}
