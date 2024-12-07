using HarmonyLib;
using MiraAPI.GameModes;

namespace MiraAPI.Patches.GameModes;

[HarmonyPatch(typeof(HudManager))]
internal static class HudPatches
{
    [HarmonyPostfix, HarmonyPatch(nameof(HudManager.Start))]
    public static void HudStartPatch(HudManager __instance) => CustomGameModeManager.ActiveMode?.HudStart(__instance);

    [HarmonyPostfix, HarmonyPatch(nameof(HudManager.Update))]
    public static void HudUpdatePatch(HudManager __instance) => CustomGameModeManager.ActiveMode?.HudUpdate(__instance);
}
