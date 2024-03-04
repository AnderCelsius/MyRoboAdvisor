using System.ComponentModel;

namespace MyRoboAdvisor.Extensions;

public static class EnumExtensions
{
  public static string GetDescription(this Enum value)
  {
    var type = value.GetType();
    string name = Enum.GetName(type, value)!;

    var field = type.GetField(name);
    if (field != null &&
        Attribute.GetCustomAttribute(
          field,
          typeof(DescriptionAttribute)) is DescriptionAttribute attr)
    {
      return attr.Description;
    }

    return name;
  }

  public static T? ToEnum<T>(this string? value)
    where T : struct, Enum
  {
    if (value is null)
    {
      return null;
    }

    var values = Enum.GetValues<T>();

    var names = values
      .Select(v => new { Value = v, Name = Enum.GetName(v)! })
      .ToDictionary(v => v.Name, v => v.Value);

    if (names.TryGetValue(value, out var nameResult))
    {
      return nameResult;
    }

    var descriptions = values
      .Select(v => new { Value = v, Description = v.GetDescription() })
      .ToDictionary(v => v.Description, v => v.Value);

    if (descriptions.TryGetValue(value, out var descriptionResult))
    {
      return descriptionResult;
    }

    return null;
  }
}