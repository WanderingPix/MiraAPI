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

    /// <summary>
    /// Gets or sets the game mode option.
    /// </summary>
    public ModdedNumberOption CurrentMode { get; set; } = new("Game Mode: {0}", 0, 0, CustomGameModeManager.LastId, 1, Utilities.MiraNumberSuffixes.None)
    {
        ChangedEvent = x => CustomGameModeManager.SetGameMode((uint)x),
    };
}
