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
    LocalSettingsTab tab,
    string? name = null,
    string? description = null,
    Color? toggleColor = null,
    Color? toggleHoverColor = null,
    Color? toggleActiveColor = null
    ) : LocalSettingAttribute<LocalSettingToggle>(tab, name, description)
{
    /// <inheritdoc/>
    protected override LocalSettingToggle CreateSetting(ConfigEntryBase configEntry)
    {
        return new LocalSettingToggle(tab, configEntry, name, description, toggleColor, toggleHoverColor, toggleActiveColor);
    }
}
