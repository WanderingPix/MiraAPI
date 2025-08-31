using System;
using BepInEx.Configuration;
using MiraAPI.LocalSettings.SettingTypes;
using MiraAPI.Utilities;
using UnityEngine;

namespace MiraAPI.LocalSettings.Attributes;

/// <summary>
/// Creates a float and binds it with the config entry.
/// </summary>
[AttributeUsage(AttributeTargets.Property)]
public class LocalSettingFloatAttribute(
    string? name = null,
    float min = 0,
    float max = 100,
    string? description = null,
    string? formatString = null,
    bool roundValue = true,
    MiraNumberSuffixes suffixType = MiraNumberSuffixes.None
    ) : LocalSettingAttribute(name, description)
{
    /// <inheritdoc/>
    internal override LocalSettingFloat CreateSetting(Type tab, ConfigEntryBase configEntryBase)
    {
        return new LocalSettingFloat(tab, configEntryBase, name, description, new FloatRange(min, max), suffixType, formatString, roundValue);
    }
}
