using BepInEx.Configuration;
using MiraAPI.LocalSettings;
using MiraAPI.LocalSettings.Attributes;
using Reactor.Utilities;

namespace MiraAPI.Example;

public static class ExampleLocalSetings
{
    public static LocalSettingsTab ModTab { get; } = new("Example Tab");

    [LocalSetting]
    public static ConfigEntry<bool> ExampleToggleSetting { get; private set; }
}
