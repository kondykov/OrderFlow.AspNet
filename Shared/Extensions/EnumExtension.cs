using System.ComponentModel;

namespace OrderFlow.Shared.Extensions;

public static class EnumExtension
{
    /// <summary>
    ///     Получить атрибут "Описание"
    /// </summary>
    /// <param name="value">Значение перечисления</param>
    /// <returns>Значение атрибута "Описание"</returns>
    public static string GetDescription(this Enum value)
    {
        var fieldInfo = value.GetType().GetField(value.ToString());

        var attributes = (DescriptionAttribute[])fieldInfo!.GetCustomAttributes(typeof(DescriptionAttribute), false);

        return attributes.Length > 0 ? attributes[0].Description : value.ToString();
    }
}