namespace RT.CommandLine;

/// <summary>
///     Specifies that a field of an enum type should be interpreted as multiple possible options, each specified by an <see
///     cref="OptionAttribute"/> on the enum values in the enum type.</summary>
[AttributeUsage(AttributeTargets.Field, Inherited = false, AllowMultiple = false)]
public sealed class EnumOptionsAttribute(EnumBehavior behavior) : Attribute
{
    /// <summary>
    ///     Specifies whether the enum is considered to represent a single value or a bitfield containing multiple values.</summary>
    public EnumBehavior Behavior { get; private set; } = behavior;
}
