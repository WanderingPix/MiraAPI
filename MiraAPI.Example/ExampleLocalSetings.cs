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
        ToggleActiveColor = Color.blue,
        ToggleInactiveColor = Color.red,
    };

    [LocalSettingsButton]
    public LocalSettingsButton ExampleButton { get; private set; } = new("Example Button", OnExampleButtonClick);

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

    private static void OnExampleButtonClick(LocalSettingsTab instance)
    {
        Logger<ExamplePlugin>.Warning("Button clicked");
    }
}

public enum ExampleEnumSetting
{
    Cheeseburger,
    Sandwich,
    Fries,
    ChickenNuggets,
}
