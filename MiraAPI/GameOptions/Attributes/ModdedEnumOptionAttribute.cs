using MiraAPI.GameOptions.OptionTypes;
using System;
using System.Reflection;

namespace MiraAPI.GameOptions.Attributes;

/// <summary>
/// Attribute for creating an enum option.
/// </summary>
/// <param name="title">The title of the option.</param>
/// <param name="enumType">The Enum type.</param>
/// <param name="values">An option list of string values to use in place of the enum name.</param>
/// <param name="roleType">An optional role type.</param>
/// <param name="modeType">An optional game mode type.</param>
[AttributeUsage(AttributeTargets.Property)]
public class ModdedEnumOptionAttribute(string title, Type enumType, string[]? values = null, Type? roleType = null, Type? modeType = null)
    : ModdedOptionAttribute(title, roleType, modeType)
{
    internal override IModdedOption CreateOption(object? value, PropertyInfo property)
    {
        var opt = new ModdedEnumOption(Title, (int)(value ?? 0), enumType, values, RoleType, ModeType);
        return opt;
    }

    /// <inheritdoc />
    public override void SetValue(object value)
    {
        var opt = HolderOption as ModdedEnumOption;
        opt?.SetValue((int)value);
    }

    /// <inheritdoc />
    public override object GetValue()
    {
        return HolderOption is ModdedEnumOption opt
            ? Enum.ToObject(enumType, opt.Value)
            : throw new InvalidOperationException($"HolderOption for option \"{Title}\" with EnumType ${enumType.FullName} is not a ModdedEnumOption");
    }
}
