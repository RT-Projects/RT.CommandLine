using System.Reflection;
using RT.Internal;
using RT.Util;
using RT.Util.Consoles;
using RT.Util.ExtensionMethods;

namespace RT.CommandLine;

static class CmdLineExtensions
{
    public static string[] GetOrderedOptionAttributeNames(this MemberInfo member) =>
        member.GetCustomAttributes<OptionAttribute>().FirstOrDefault()?.Names.OrderBy(compareOptionNames).ToArray();

    private static int compareOptionNames(string opt1, string opt2)
    {
        bool long1 = opt1.StartsWith("--");
        bool long2 = opt2.StartsWith("--");
        if (long1 == long2)
            return StringComparer.OrdinalIgnoreCase.Compare(opt1, opt2);
        else if (long1)
            return 1; // --blah comes after -blah
        else
            return -1;
    }

    public static ConsoleColoredString FormatParameterUsage(this FieldInfo field, bool isMandatory)
    {
        // Positionals
        if (field.IsDefined<IsPositionalAttribute>())
            return (isMandatory ? "{0}" : "[{0}]").Color(CmdLineColor.OptionalityDelimiters).Fmt(
                "<".Color(CmdLineColor.FieldBrackets) + field.Name.Color(CmdLineColor.Field) + ">".Color(CmdLineColor.FieldBrackets));

        // -t name [-t name [...]]    — arrays, multi-value enums with CommandNames
        if (field.FieldType.IsArray ||
            (field.FieldType.IsEnum &&
                field.IsDefined<OptionAttribute>() &&
                field.IsDefined<EnumOptionsAttribute>() &&
                field.GetCustomAttributes<EnumOptionsAttribute>().First().Behavior == EnumBehavior.MultipleValues))
        {
            return (isMandatory ? "{0} {1} [{0} {1} [...]]" : "[{0} {1} [{0} {1} [...]]]").Color(CmdLineColor.OptionalityDelimiters).Fmt(
                field.GetOrderedOptionAttributeNames().First().Color(CmdLineColor.Option),
                "<".Color(CmdLineColor.FieldBrackets) + field.Name.Color(CmdLineColor.Field) + ">".Color(CmdLineColor.FieldBrackets));
        }

        // Enums with Option names
        if (field.FieldType.IsEnum && !field.IsDefined<OptionAttribute>())
        {
            var options = field.FieldType.GetFields(BindingFlags.Public | BindingFlags.Static)
                .Where(fld => fld.IsDefined<OptionAttribute>() && !fld.IsDefined<UndocumentedAttribute>())
                .Select(fi => fi.GetOrderedOptionAttributeNames().First().Color(CmdLineColor.Option))
                .ToArray();

            if (field.IsDefined<EnumOptionsAttribute>() && field.GetCustomAttributes<EnumOptionsAttribute>().First().Behavior == EnumBehavior.MultipleValues)
                // [-t] [-u] [-v]    — multi-value enums with Option names
                return options.Select(opt => "[{0}]".Color(CmdLineColor.OptionalityDelimiters).Fmt(opt)).JoinColoredString(" ");

            // {-t|-u}      — single-value enums with Options
            return (isMandatory ? (options.Length > 1 ? "{{{0}{1}" : "{0}") : "[{0}]").Color(CmdLineColor.OptionalityDelimiters).Fmt(options.JoinColoredString("|".Color(CmdLineColor.OptionalityDelimiters)), "}");
        }

        // -t       — bools
        if (field.FieldType == typeof(bool))
            return "[{0}]".Color(CmdLineColor.OptionalityDelimiters).Fmt(field.GetOrderedOptionAttributeNames().First().Color(CmdLineColor.Option));

        // -t name
        return (isMandatory ? "{0} {1}" : "[{0} {1}]").Color(CmdLineColor.OptionalityDelimiters).Fmt(
            field.GetOrderedOptionAttributeNames().First().Color(CmdLineColor.Option),
            "<".Color(CmdLineColor.FieldBrackets) + field.Name.Color(CmdLineColor.Field) + ">".Color(CmdLineColor.FieldBrackets));
    }

    public static IEnumerable<FieldInfo> GetCommandLineFields(this Type type) =>
        type.GetFields(BindingFlags.Instance | BindingFlags.NonPublic | BindingFlags.Public)
            .Where(f => f.IsDefined<OptionAttribute>() || f.IsDefined<EnumOptionsAttribute>() || f.IsDefined<IsPositionalAttribute>())
            .OrderBy(f => f.GetCustomAttribute<IsPositionalAttribute>()?.Order)
            .ThenBy(f => f.DeclaringType.SelectChain(t => t.BaseType).Count());
}
