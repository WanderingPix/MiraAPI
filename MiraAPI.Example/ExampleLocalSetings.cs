using BepInEx.Configuration;
using MiraAPI.LocalSettings;
using MiraAPI.LocalSettings.Attributes;
using Reactor.Utilities;

namespace MiraAPI.Example;

public static class ExampleLocalSetings
{
    public static LocalSettingsTab ModTab { get; } = new LocalSettingsTab("Example Tab");

    [LocalSettingToggle]
    public static ConfigEntry<bool> ExampleToggleSetting { get; set; }

    internal static void BindSettings(ConfigFile config)
    {
        ExampleToggleSetting = config.Bind("General", "Example Toggle", false);
    }
}