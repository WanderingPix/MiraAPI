﻿using System;
using System.Reflection;
using MiraAPI.GameOptions.OptionTypes;

namespace MiraAPI.GameOptions.Attributes;

/// <summary>
/// Attribute for creating an enum option.
/// </summary>
[AttributeUsage(AttributeTargets.Property)]
public class ModdedEnumOptionAttribute(string title, Type enumType, string[]? values = null)
    : ModdedOptionAttribute(title)
{
    internal override IModdedOption CreateOption(object? value, PropertyInfo property)
    {
        var opt = new ModdedEnumOption(Title, (int)(value ?? 0), enumType, values);
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

/// <summary>
/// Generic attribute for creating an enum option.
/// </summary>
/// <typeparam name="T">An enum type.</typeparam>
[AttributeUsage(AttributeTargets.Property)]
public class ModdedEnumOptionAttribute<T>(string title, string[]? values = null)
    : ModdedOptionAttribute(title) where T : struct, Enum
{
    internal override IModdedOption CreateOption(object? value, PropertyInfo property)
    {
        var opt = new ModdedEnumOption<T>(Title, (T)(value ?? 0), values);
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
        return HolderOption is ModdedEnumOption<T> opt
            ? Enum.ToObject(typeof(T), opt.Value)
            : throw new InvalidOperationException($"HolderOption for option \"{Title}\" is not a ModdedEnumOption<{typeof(T).Name}>");
    }
}
