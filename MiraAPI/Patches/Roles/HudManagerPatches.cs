using HarmonyLib;
using InnerNet;
using MiraAPI.Roles;
using Reactor.Utilities.Extensions;

namespace MiraAPI.Patches.Roles;

[HarmonyPatch(typeof(HudManager))]
internal static class HudManagerPatches
{
    private static TaskPanelBehaviour? _roleTab;

    [HarmonyPostfix]
    [HarmonyPatch(nameof(HudManager.SetHudActive), typeof(PlayerControl), typeof(RoleBehaviour), typeof(bool))]
    // ReSharper disable once InconsistentNaming
    public static void SetHudActivePostfix(HudManager __instance, PlayerControl localPlayer, RoleBehaviour role, bool isActive)
    {
        var flag = localPlayer.Data != null && localPlayer.Data.IsDead;

        if (role is ICustomRole customRole)
        {
            __instance.KillButton.ToggleVisible(isActive && customRole.Configuration.UseVanillaKillButton && !flag);
            __instance.ImpostorVentButton.ToggleVisible(isActive && customRole.Configuration.CanUseVent && !flag);
            __instance.SabotageButton.gameObject.SetActive(isActive && customRole.Configuration.CanUseSabotage);
        }

        if (_roleTab)
        {
            _roleTab?.gameObject.SetActive(isActive);
        }
    }

    [HarmonyPostfix]
    [HarmonyPatch(nameof(HudManager.Update))]
    // ReSharper disable once InconsistentNaming
    public static void UpdatePostfix(HudManager __instance)
    {
        var local = PlayerControl.LocalPlayer;

        if (AmongUsClient.Instance.GameState != InnerNetClient.GameStates.Started && !ShipStatus.Instance)
        {
            return;
        }

        var role = local?.Data?.Role;

        if (role is ICustomRole { Configuration.RoleHintType: RoleHintType.RoleTab } customRole)
        {
            if (_roleTab == null)
            {
                _roleTab = CustomRoleManager.CreateRoleTab(customRole);
            }

            CustomRoleManager.UpdateRoleTab(_roleTab, customRole);
        }
        else if (_roleTab != null)
        {
            _roleTab.gameObject.Destroy();
        }
    }
}
