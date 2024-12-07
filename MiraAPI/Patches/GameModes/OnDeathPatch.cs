using HarmonyLib;
using MiraAPI.GameModes;

namespace MiraAPI.Patches.GameModes;

[HarmonyPatch(typeof(KillAnimation))]
internal static class OnDeathPatch
{
    [HarmonyPostfix, HarmonyPatch(nameof(KillAnimation.CoPerformKill))]
    public static void OnDeathPostfix([HarmonyArgument(0)] PlayerControl source, [HarmonyArgument(1)] PlayerControl target)
    {
        CustomGameModeManager.ActiveMode?.OnDeath(target);
    }
}
