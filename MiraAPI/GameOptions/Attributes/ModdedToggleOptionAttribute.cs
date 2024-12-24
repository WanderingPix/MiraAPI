﻿using MiraAPI.GameOptions.OptionTypes;
using System;
using System.Reflection;

namespace MiraAPI.GameOptions.Attributes;

/// <summary>
/// Attribute for a toggle option.
/// </summary>
/// <param name="title">The option title.</param>
/// <param name="roleType">An optional role type.</param>
/// <param name="modeType">An optional game mode type.</param>
[AttributeUsage(AttributeTargets.Property)]
public class ModdedToggleOptionAttribute(string title, Type? roleType = null, Type? modeType = null) : ModdedOptionAttribute(title, roleType, modeType)
{
    internal override IModdedOption CreateOption(object? value, PropertyInfo property)
    {
        var toggleOpt = new ModdedToggleOption(Title, (bool)(value ?? false), RoleType, ModeType);
        return toggleOpt;
    }

    /// <inheritdoc />
    public override void SetValue(object value)
    {
        var opt = HolderOption as ModdedToggleOption;
        opt?.SetValue((bool)value);
    }

    /// <inheritdoc />
    public override object GetValue()
    {
        if (HolderOption is ModdedToggleOption opt)
        {
            return opt.Value;
        }
        throw new InvalidOperationException($"Holder option for {Title} is not a ModdedToggleOption.");
    }
}
