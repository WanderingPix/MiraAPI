using HarmonyLib;
using MiraAPI.GameModes;

[HarmonyPatch(typeof(DeadBody))]
public static class DeadBodyPatch
{
    [HarmonyPrefix, HarmonyPatch(nameof(DeadBody.OnClick))]
    public static bool OnClickPatch(DeadBody __instance)
    {
        return CustomGameModeManager.ActiveMode?.CanReport(__instance) == true;
    }
}
