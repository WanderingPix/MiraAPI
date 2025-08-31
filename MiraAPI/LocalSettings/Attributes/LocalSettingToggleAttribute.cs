using System;
using BepInEx.Configuration;
using MiraAPI.LocalSettings.SettingTypes;
using UnityEngine;

namespace MiraAPI.LocalSettings.Attributes;

/// <summary>
/// Creates a toggle and binds it with the config entry.
/// </summary>
[AttributeUsage(AttributeTargets.Property)]
public class LocalSettingToggleAttribute(
    string? name = null,
    string? description = null
    ) : LocalSettingAttribute(name, description)
{
    /// <inheritdoc/>
    internal override LocalSettingToggle CreateSetting(Type tab, ConfigEntryBase configEntryBase)
    {
        return new LocalSettingToggle(tab, configEntryBase, name, description);
    }
}
