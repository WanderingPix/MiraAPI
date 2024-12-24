﻿using MiraAPI.GameOptions.OptionTypes;
using MiraAPI.Utilities;
using System;
using System.Reflection;

namespace MiraAPI.GameOptions.Attributes;

/// <summary>
/// A number option attribute for the modded options system.
/// </summary>
/// <param name="title">The title of the option.</param>
/// <param name="min">The minimum value.</param>
/// <param name="max">The maximum value.</param>
/// <param name="increment">The increment.</param>
/// <param name="suffixType">The suffix type.</param>
/// <param name="zeroInfinity">Whether zero is infinity or not.</param>
/// <param name="roleType">An optional role type.</param>
/// <param name="modeType">An optional game mode type.</param>
[AttributeUsage(AttributeTargets.Property)]
public class ModdedNumberOptionAttribute(
    string title,
    float min,
    float max,
    float increment = 1,
    MiraNumberSuffixes suffixType = MiraNumberSuffixes.None,
    bool zeroInfinity = false,
    Type? roleType = null,
    Type? modeType = null)
    : ModdedOptionAttribute(title, roleType, modeType)
{
    internal override IModdedOption CreateOption(object? value, PropertyInfo property)
    {
        return new ModdedNumberOption(Title, (float)(value ?? min+increment), min, max, increment, suffixType, zeroInfinity, RoleType, ModeType);
    }

    /// <inheritdoc />
    public override void SetValue(object value)
    {
        var opt = HolderOption as ModdedNumberOption;
        opt?.SetValue((float)value);
    }

    /// <inheritdoc />
    public override object GetValue()
    {
        if (HolderOption is ModdedNumberOption opt)
        {
            return opt.Value;
        }
        throw new InvalidOperationException($"HolderOption for option \"{Title}\" is not a ModdedNumberOption");
    }
}
