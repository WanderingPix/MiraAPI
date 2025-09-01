using System;
using BepInEx.Configuration;
using MiraAPI.LocalSettings.SettingTypes;

namespace MiraAPI.LocalSettings.Attributes;

/// <summary>
/// Creates a toggle setting for the <see cref="ConfigEntry{T}"/>.
/// </summary>
/// <inheritdoc/>
[AttributeUsage(AttributeTargets.Property)]
public class LocalSettingToggleAttribute(
    string? name = null,
    string? description = null
    ) : LocalSettingAttribute(name, description)
{
    /// <inheritdoc/>
    internal override LocalToggleSetting CreateSetting(Type tab, ConfigEntryBase configEntryBase)
    {
        return new LocalToggleSetting(tab, configEntryBase, name, description);
    }
}
