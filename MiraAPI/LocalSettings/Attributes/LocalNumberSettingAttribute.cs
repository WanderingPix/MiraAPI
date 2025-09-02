﻿using System;
using BepInEx.Configuration;
using MiraAPI.LocalSettings.SettingTypes;
using MiraAPI.Utilities;

namespace MiraAPI.LocalSettings.Attributes;

/// <summary>
/// Creates a number setting for the <see cref="ConfigEntry{T}"/>.
/// </summary>
/// <param name="min">Minimum range.</param>
/// <param name="max">Maximum range.</param>
/// <param name="increment">Increment per use.</param>
/// <param name="suffixType">Suffix for the value.</param>
/// <param name="formatString">Format string used when formating.</param>
/// <inheritdoc/>
[AttributeUsage(AttributeTargets.Property)]
public class LocalNumberSettingAttribute(
    string? name = null,
    string? description = null,
    float min = 1,
    float max = 5,
    float increment = 1,
    MiraNumberSuffixes suffixType = MiraNumberSuffixes.None,
    string? formatString = null
    ) : LocalSettingAttribute(name, description)
{
    /// <inheritdoc/>
    internal override LocalNumberSetting CreateSetting(Type tab, ConfigEntryBase configEntryBase)
    {
        return new LocalNumberSetting(tab, configEntryBase, name, description, new FloatRange(min, max), increment, suffixType, formatString);
    }
}
