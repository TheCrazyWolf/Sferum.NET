using System.ComponentModel.DataAnnotations;
using System.Reflection;

namespace SferumNet.Scenarios.Extensions;

public static class EnumExtension
{
    public static string GetDisplayName(this Enum enumValue)
    {
        return enumValue.GetType()
                        .GetMember(enumValue.ToString())
                        .First()
                        .GetCustomAttribute<DisplayAttribute>()
                        ?.GetName() ?? string.Empty;
    }
}