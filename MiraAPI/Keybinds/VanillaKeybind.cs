using Rewired;
using TMPro;
using UnityEngine;

namespace MiraAPI.Keybinds;

public class VanillaKeybind
{
    public AbilityButton Ability { get; }
    public InputAction RewiredInputAction { get; }
    public KeyboardKeyCode CurrentKey => KeybindUtils.GetKeycodeByActionId(RewiredInputAction.id);

    public VanillaKeybind(AbilityButton ability, InputAction inputAction)
    {
        Ability = ability;
        RewiredInputAction = inputAction;
    }
}
