using RT.Util;

namespace RT.CommandLine;

/// <summary>Use this to specify that a command-line parameter is mandatory.</summary>
[AttributeUsage(AttributeTargets.Field, Inherited = false, AllowMultiple = false), RummageKeepUsersReflectionSafe]
public sealed class IsMandatoryAttribute() : Attribute
{
}
