namespace MiraAPI.Utilities;

/// <summary>
/// An enum to dictate how to represent a numeric value.
/// </summary>
public enum MiraNumberSuffixes
{
    /// <summary>
    /// There should be no suffix.
    /// </summary>
    None,

    /// <summary>
    /// Display as a multiplier.
    /// </summary>
    Multiplier,

    /// <summary>
    /// Display in seconds.
    /// </summary>
    Seconds,

    /// <summary>
    /// Display as a percentage.
    /// </summary>
    Percent,

    /// <summary>
    /// Display in units.
    /// </summary>
    Distance,
}
