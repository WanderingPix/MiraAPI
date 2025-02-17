using MiraAPI.PluginLoading;

namespace MiraAPI.GameModes;

/// <summary>
/// The default game mode.
/// </summary>
[MiraIgnore]
public class DefaultMode : AbstractGameMode
{
    /// <inheritdoc/>
    public override string Name => "Default";

    /// <inheritdoc/>
    public override string Description => "Default Among Us GameMode";
}
