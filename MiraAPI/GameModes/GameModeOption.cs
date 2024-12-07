using MiraAPI.GameOptions;
using MiraAPI.GameOptions.OptionTypes;

namespace MiraAPI.GameModes;

/// <summary>
/// The game mode option.
/// </summary>
public class GameModeOption : AbstractOptionGroup
{
    /// <inheritdoc/>
    public override string GroupName => "Game Mode";

    public ModdedNumberOption CurrentMode { get; set; } = new("Current Game Mode", 0, 0, CustomGameModeManager.LastId, 1, Utilities.MiraNumberSuffixes.None)
    {
        ChangedEvent = x => CustomGameModeManager.SetGameMode((uint)x),
    };
}
