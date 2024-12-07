using HarmonyLib;
using MiraAPI.GameModes;

[HarmonyPatch]
public static class HudPatches
{
    [HarmonyPostfix, HarmonyPatch(typeof(HudManager), nameof(HudManager.Start))]
    public static void HudStartPatch(HudManager __instance) => CustomGameModeManager.ActiveMode?.HudStart(__instance);

    [HarmonyPostfix, HarmonyPatch(typeof(HudManager), nameof(HudManager.Update))]
    public static void HudUpdatePatch(HudManager __instance) => CustomGameModeManager.ActiveMode?.HudUpdate(__instance);
}
