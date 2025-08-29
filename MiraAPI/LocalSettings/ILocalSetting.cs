using BepInEx.Configuration;
using UnityEngine;

namespace MiraAPI.LocalSettings;

/// <summary>
/// Interface for all local settings
/// </summary>
public interface ILocalSetting
{
    string Name { get; }
    string Description { get; }
    GameObject? Setting { get; }
    ConfigEntryBase ConfigEntry { get; }
    GameObject? CreateOption(ToggleButtonBehaviour toggle, SlideBar slider, Transform parent, ref float offset, ref int order, bool last);
}
