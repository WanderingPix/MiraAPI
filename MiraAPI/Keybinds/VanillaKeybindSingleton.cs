using System.Linq;

namespace MiraAPI.Keybinds;

public static class VanillaKeybindSingleton<T> where T : AbilityButton
{
    internal static VanillaKeybind _instance;

    /// <summary>
    /// Gets the instance of the option group.
    /// </summary>
#pragma warning disable CA1000
    public static VanillaKeybind Instance => _instance;
#pragma warning restore CA1000
}