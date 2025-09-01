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
    protected override bool ShouldCreateLabels => false;

    public override LocalSettingTabAppearance TabAppearance => new()
    {
        TabColor = Color.cyan,
        ToggleActiveColor = Color.blue,
        ToggleInactiveColor = Color.red,
    };

    [LocalToggleSetting]
    public ConfigEntry<bool> ExampleToggle { get; private set; } = config.Bind("General", "Example Bool", true);

    [LocalToggleSetting]
    public ConfigEntry<bool> ExampleToggle2 { get; private set; } = config.Bind("General", "Example Bool 2", false);

    [LocalSliderSetting(suffixType: MiraNumberSuffixes.Percent, formatString: "0", roundValue: true)]
    public ConfigEntry<float> ExampleSlider { get; private set; } = config.Bind("General", "Slider", 50f);

    [LocalNumberSetting(suffixType: MiraNumberSuffixes.Seconds)]
    public ConfigEntry<int> ExampleNumber { get; private set; } = config.Bind("General", "Example Number", 4);

    [LocalEnumSetting]
    public ConfigEntry<ExampleEnumSetting> ExampleEnum { get; private set; } = config.Bind("General", "Example Enum", ExampleEnumSetting.Fries);
}

public enum ExampleEnumSetting
{
    Cheeseburger,
    Sandwich,
    Fries,
    ChickenNuggets,
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
