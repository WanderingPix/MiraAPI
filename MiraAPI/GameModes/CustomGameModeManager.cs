using System;
using System.Collections.Generic;
using Il2CppSystem.Threading.Tasks;
using MiraAPI.GameOptions;
using MiraAPI.PluginLoading;
using Reactor.Utilities;
using Reactor.Utilities.Extensions;

namespace MiraAPI.GameModes;

/// <summary>
/// Manages custom gamemodes.
/// </summary>
public static class CustomGameModeManager
{
    private static readonly Dictionary<uint, AbstractGameMode> IdToModeMap = [];

    private static uint _nextId;

    private static uint GetNextId()
    {
        _nextId++;
        return _nextId;
    }

    internal static uint LastId => _nextId;

    /// <summary>
    /// Register gamemode from type.
    /// </summary>
    /// <param name="gameModeType">Type of gamemode class, should inherit from <see cref="AbstractGameMode"/>.</param>
    /// <param name="pluginInfo">The custom plugin info of the mod.</param>
    internal static void RegisterGameMode(Type gameModeType, MiraPluginInfo pluginInfo)
    {
        if (!typeof(AbstractGameMode).IsAssignableFrom(gameModeType))
        {
            return;
        }

        var instance = Activator.CreateInstance(gameModeType);

        if (instance is not AbstractGameMode mode)
        {
            return;
        }

        IdToModeMap.Add(GetNextId(), mode);
        pluginInfo.GameModes.Add(_nextId, mode);

        mode.ID = _nextId;
    }

    /// <summary>
    /// Checks to see if the default game mode is on.
    /// </summary>
    /// <returns>True if the default mode is one.</returns>
    public static bool IsDefault() => (uint)OptionGroupSingleton<GameModeOption>.Instance.CurrentMode.Value == 0;

    /// <summary>
    /// Gets the current gamemode.
    /// </summary>
    public static AbstractGameMode? ActiveMode { get; internal set; }

    internal static void RegisterDefaultMode()
    {
        var defaultMode = new DefaultMode();
        IdToModeMap.Add(0, defaultMode);
        defaultMode.ID = 0;
    }

    internal static void SetGameMode(uint id)
    {
        if (IdToModeMap.TryGetValue(id, out var mode))
        {
            ActiveMode = mode;
        }
        else if (id != 0)
        {
            ActiveMode = IdToModeMap[0];
            OptionGroupSingleton<GameModeOption>.Instance.CurrentMode.SetValue(0);
            Logger<MiraApiPlugin>.Warning($"Unable to find game mode of id {id}!");
        }

        if (OptionGroupSingleton<GameModeOption>.Instance.CurrentMode.OptionBehaviour != null && ActiveMode != null)
        {
            ((NumberOption)OptionGroupSingleton<GameModeOption>.Instance.CurrentMode.OptionBehaviour).TitleText.SetText($"Game Mode: <#{ActiveMode.Color.ToHtmlStringRGBA()}>{ActiveMode}</color>");
        }
    }

    internal static void GetAndSetGameMode()
    {
        var id = (uint)OptionGroupSingleton<GameModeOption>.Instance.CurrentMode.Value;

        if (IdToModeMap.TryGetValue(id, out var mode))
        {
            ActiveMode = mode;
            return;
        }

        ActiveMode = IdToModeMap[0];
        Logger<MiraApiPlugin>.Warning($"Unable to find game mode of id {id}!");
    }
}
