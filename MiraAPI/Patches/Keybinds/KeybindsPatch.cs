using System.Linq;
using BepInEx;
using BepInEx.Unity.IL2CPP;
using HarmonyLib;
using MiraAPI.Keybinds;
using MiraAPI.PluginLoading;
using MiraAPI.Utilities;
using Reactor.Utilities;
using Rewired;

namespace MiraAPI.Patches.Keybinds;

[HarmonyPatch(typeof(InputManager_Base), nameof(InputManager_Base.Awake))]
public static class KeybindMenuPatch
{
    private static bool _registered;

    [HarmonyPrefix]
    private static void AwakePrefix(InputManager_Base __instance)
    {
        if (_registered)
        {
            return;
        }

        KeybindUtils.RewiredInputManager = __instance;
        foreach (var keybind in KeybindManager.Keybinds)
        {
            if (__instance.userData.actions.ToArray().Any(x => x.name == keybind.Id))
            {
                Logger<MiraApiPlugin>.Warning($"Keybind of id {keybind.Id} already exists. Skipping it");
                continue;
            }

            PluginInfo? info = IL2CPPChainloader.Instance.Plugins.Values
                .FirstOrDefault(p => p.Instance == keybind.SourcePlugin);
            string group = info!.Metadata.Name;
            if (keybind.SourcePlugin is IMiraPlugin miraPlugin)
            {
                group = miraPlugin.OptionsTitleText;
            }

            keybind.RewiredInputAction = __instance.userData.RegisterModBind(
                keybind.Id,
                keybind.Name,
                group,
                keybind.DefaultKey,
                modifiers: keybind.ModifierKeys);
        }
        _registered = true;
    }
}
