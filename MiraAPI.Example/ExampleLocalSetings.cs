using BepInEx.Configuration;
using MiraAPI.LocalSettings;
using MiraAPI.LocalSettings.Attributes;
using MiraAPI.Utilities;
using Reactor.Utilities;
using UnityEngine;

namespace MiraAPI.Example;

public class ExampleLocalSettings(ConfigFile config) : LocalSettingsTab(config)
{
    public override string TabName => "Mira Example";
    public override LocalSettingTabAppearance TabAppearance => new()
    {
        TabColor = Color.cyan,
        ToggleActiveColor = Color.blue,
        ToggleInactiveColor = Color.red,
        SliderHoverColor = Color.magenta,
    };

    [LocalSliderSetting(min: 0, max: 15, suffixType: MiraNumberSuffixes.Multiplier)]
    public ConfigEntry<float> ExampleFloatEntry { get; private set; } = config.Bind("General", "Example Float Setting", 50f);

    [LocalSettingToggle]
    public ConfigEntry<bool> PlayStartupSound { get; private set; } = config.Bind("Audio", "Play Startup Sound", true);

    [LocalSettingToggle]
    public ConfigEntry<bool> MuteInBackground { get; private set; } = config.Bind("Audio", "Mute In Background", false);

    [LocalSettingToggle]
    public ConfigEntry<bool> AutoSaveEnabled { get; private set; } = config.Bind("General", "Auto Save Enabled", true);

    [LocalSettingToggle]
    public ConfigEntry<bool> EnableCloudSync { get; private set; } = config.Bind("General", "Enable Cloud Sync", false);

    [LocalSettingToggle]
    public ConfigEntry<bool> ShowAdvancedTooltips { get; private set; } = config.Bind("UI", "Show Advanced Tooltips", true);

    [LocalSettingToggle]
    public ConfigEntry<bool> CompactUI { get; private set; } = config.Bind("UI", "Compact UI Layout", false);

    [LocalSettingToggle]
    public ConfigEntry<bool> EnableControllerSupport { get; private set; } = config.Bind("Input", "Enable Controller Support", true);

    [LocalSettingToggle]
    public ConfigEntry<bool> EnableVibration { get; private set; } = config.Bind("Input", "Enable Controller Vibration", true);

    [LocalSettingToggle]
    public ConfigEntry<bool> ShowPlayerNames { get; private set; } = config.Bind("Gameplay", "Show Player Names", true);

    [LocalSettingToggle]
    public ConfigEntry<bool> EnableHardMode { get; private set; } = config.Bind("Gameplay", "Enable Hard Mode", false);
    [LocalSettingToggle]
    public ConfigEntry<bool> EnableHardMode2 { get; private set; } = config.Bind("Gameplay", "Enable Hard Mode", false);
    [LocalSettingToggle]
    public ConfigEntry<bool> EnableHardMode3 { get; private set; } = config.Bind("Gameplay", "Enable Hard Mode", false);
    [LocalSettingToggle]
    public ConfigEntry<bool> EnableHardMode4 { get; private set; } = config.Bind("Gameplay", "Enable Hard Mode", false);
    [LocalSettingToggle]
    public ConfigEntry<bool> EnableHardMode5 { get; private set; } = config.Bind("Gameplay", "Enable Hard Mode", false);

    [LocalNumberSetting]
    public ConfigEntry<int> NumberExample { get; private set; } = config.Bind("Gameplay", "Number of cheeseburgers", 3);

    [LocalEnumSetting]
    public ConfigEntry<ExampleSettingEnum> EnumExample { get; private set; } = config.Bind("Gameplay", "Anything else", ExampleSettingEnum.Fries);
}

public enum ExampleSettingEnum
{
    Fries,
    OnionRings,
    ChickenNuggets,
    Gun,
}

public class Tab1(ConfigFile config) : LocalSettingsTab(config) { public override string TabName => "Tab"; public override LocalSettingTabAppearance TabAppearance => new(); }
public class Tab2(ConfigFile config) : LocalSettingsTab(config) { public override string TabName => "Tab"; public override LocalSettingTabAppearance TabAppearance => new(); }
public class Tab4(ConfigFile config) : LocalSettingsTab(config) { public override string TabName => "Tab"; public override LocalSettingTabAppearance TabAppearance => new(); }
public class Tab3(ConfigFile config) : LocalSettingsTab(config) { public override string TabName => "Tab"; public override LocalSettingTabAppearance TabAppearance => new(); }
public class Tab5(ConfigFile config) : LocalSettingsTab(config) { public override string TabName => "Tab"; public override LocalSettingTabAppearance TabAppearance => new(); }
public class Tab6(ConfigFile config) : LocalSettingsTab(config) { public override string TabName => "Tab"; public override LocalSettingTabAppearance TabAppearance => new(); }
public class Tab7(ConfigFile config) : LocalSettingsTab(config) { public override string TabName => "Tab"; public override LocalSettingTabAppearance TabAppearance => new(); }
public class Tab8(ConfigFile config) : LocalSettingsTab(config) { public override string TabName => "Tab"; public override LocalSettingTabAppearance TabAppearance => new(); }
public class Tab9(ConfigFile config) : LocalSettingsTab(config) { public override string TabName => "Tab"; public override LocalSettingTabAppearance TabAppearance => new(); }
public class Tab10(ConfigFile config) : LocalSettingsTab(config) { public override string TabName => "Tab"; public override LocalSettingTabAppearance TabAppearance => new(); }
public class Tab11(ConfigFile config) : LocalSettingsTab(config) { public override string TabName => "Tab"; public override LocalSettingTabAppearance TabAppearance => new(); }
