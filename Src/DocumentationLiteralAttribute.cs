using RT.Util;

namespace RT.CommandLine;

/// <summary>
///     This is a legacy attribute. Do not use in new programs. This attribute is equivalent to <see
///     cref="DocumentationEggsMLAttribute"/>.</summary>
[AttributeUsage(AttributeTargets.Field | AttributeTargets.Class | AttributeTargets.Method, Inherited = false, AllowMultiple = false), RummageKeepUsersReflectionSafe]
public class DocumentationLiteralAttribute(string documentation) : DocumentationEggsMLAttribute(documentation)
{
}
