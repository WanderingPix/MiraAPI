using Rewired;

namespace MiraAPI.Keybinds;

/// <summary>
/// Represents a vanilla keybind.
/// </summary>
public class VanillaKeybind : BaseKeybind
{
    /// <summary>
    /// Gets the <see cref="ActionButton"/> this keybind is binded to.
    /// </summary>
    public ActionButton? Button { get; }

    /// <summary>
    /// Initializes a new instance of the <see cref="VanillaKeybind"/> class.
    /// </summary>
    /// <param name="button">The keybind action button.</param>
    /// <param name="action">The rewired input action.</param>
    public VanillaKeybind(ActionButton button, InputAction action) : base(action.name)
    {
        Button = button;
        RewiredInputAction = action;
    }
}
