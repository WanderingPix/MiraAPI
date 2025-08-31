using BepInEx.Configuration;
using MiraAPI.LocalSettings;
using MiraAPI.LocalSettings.Attributes;
using Reactor.Utilities;

namespace MiraAPI.Example;

public class ExampleLocalSettings(ConfigFile config) : LocalSettingsTab(config)
{
    public override string TabName => "Mira Example";

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
}
