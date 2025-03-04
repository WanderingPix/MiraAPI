namespace MiraAPI.Hud;

/// <summary>
/// The location of the custom button on the screen.
/// </summary>
public enum ButtonLocation
{
    /// <summary>
    /// Placed in the bottom left.
    /// </summary>
    BottomLeft,

    /// <summary>
    /// Placed inbetween BottomLeft and BottomRight
    /// Note: Don't put too many of these in the center or it'll look cluttered
    /// <summary>
    Center,

    /// <summary>
    /// Placed in the bottom right.
    /// </summary>
    BottomRight,
}
