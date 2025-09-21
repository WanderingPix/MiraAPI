﻿using System;
using BepInEx.Configuration;
using MiraAPI.LocalSettings.SettingTypes;
using MiraAPI.Utilities;

namespace MiraAPI.LocalSettings.Attributes;

/// <summary>
/// Creates a slider setting for the <see cref="ConfigEntry{T}"/>.
/// </summary>
/// <inheritdoc/>
/// <param name="min">Minimum range.</param>
/// <param name="max">Maximum range.</param>
/// <param name="roundValue">Should the value be rounded.</param>
/// <param name="suffixType">Suffix for the value.</param>
[AttributeUsage(AttributeTargets.Property)]
public class LocalSliderSettingAttribute(
    string? name = null,
    float min = 0,
    float max = 100,
    string? description = null,
    bool displayValue = false,
    string? formatString = null,
    bool roundValue = false,
    MiraNumberSuffixes suffixType = MiraNumberSuffixes.None
    ) : LocalSettingAttribute(name, description)
{
    private readonly string? _name = name;
    private readonly string? _description = description;

    /// <inheritdoc/>
    public override LocalSliderSetting CreateSetting(Type tab, ConfigEntryBase configEntryBase)
    {
        return new LocalSliderSetting(tab, configEntryBase, _name, _description, new FloatRange(min, max), displayValue, suffixType, formatString, roundValue);
    }
}
