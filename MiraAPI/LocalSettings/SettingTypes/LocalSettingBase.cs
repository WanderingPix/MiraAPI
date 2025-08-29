using BepInEx.Configuration;
using TMPro;
using UnityEngine;

namespace MiraAPI.LocalSettings.SettingTypes;

public abstract class LocalSettingBase<T> : ILocalSetting
{
    public string Name { get; }
    public string Description { get; }

    public GameObject? Setting { get; }
    public ConfigEntryBase ConfigEntry { get; }

    /// <summary>
    /// Initializes a new instance of the <see cref="LocalSettingBase{T}"/> class.
    /// </summary>
    /// <param name="configEntry">The config entry.</param>
    /// <param name="name">The name of the setting.</param>
    /// <param name="description">The description of the setting.</param>
    protected LocalSettingBase(LocalSettingsTab tab, ConfigEntryBase configEntry, string name = null, string description = null)
    {
        ConfigEntry = configEntry;
        Name = name ?? ConfigEntry.Definition.Key;
        Description = description;

        tab.Settings.Add(this);
    }

    /// <summary>
    /// Creates an instance of the setting.
    /// </summary>
    /// <param name="parent">The transform parent of the setting.</param>
    /// <returns>The created setting.</returns>
    public abstract GameObject? CreateOption(ToggleButtonBehaviour toggle, SlideBar slider, Transform parent, ref float offset, ref int order, bool last);

    /// <summary>
    /// Gets the value of the config entry, casted to T.
    /// </summary>
    /// <returns>The value.</returns>
    public T GetValue()
    {
        return (T)ConfigEntry.BoxedValue;
    }

    /// <summary>
    /// Sets the value of the config entry
    /// </summary>
    /// <param name="value">The value to set to.</param>
    public void SetValue(T value)
    {
        ConfigEntry.BoxedValue = value;
    }
}
