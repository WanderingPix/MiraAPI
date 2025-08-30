using System.Linq;
using HarmonyLib;
using MiraAPI.Keybinds;
using Rewired.UI.ControlMapper;
using UnityEngine;
using UnityEngine.UI;

namespace MiraAPI.Patches.Keybinds;

[HarmonyPatch(typeof(ControlMapper))]
public static class ControlMapperPatches
{
    [HarmonyPrefix]
    [HarmonyPatch(nameof(ControlMapper.Update))]
    private static void UpdatePrefix(ControlMapper __instance)
    {
        foreach (var element in __instance.themedElements)
        {
            var info = element.GetComponent<InputFieldInfo>();
            if (info == null)
            {
                continue;
            }

            var key = info.glyphOrText?.actionElementMap?.keyboardKeyCode;
            if (key == null)
            {
                continue;
            }

            var conflicts = KeybindManager.GetConflicts();
            if (conflicts.Any(x => x.Key.ToString() == key.ToString()))
            {
                element.GetComponent<Image>().color = Color.red;
            }
        }
    }
}
