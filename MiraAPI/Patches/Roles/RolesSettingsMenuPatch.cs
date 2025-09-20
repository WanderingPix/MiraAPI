using System.Linq;
using AmongUs.GameOptions;
using HarmonyLib;
using MiraAPI.Roles;
using UnityEngine;

namespace MiraAPI.Patches.Roles;

[HarmonyPatch(typeof(RolesSettingsMenu))]
public static class RolesSettingsMenuPatch
{
    [HarmonyPrefix]
    [HarmonyPatch(nameof(RolesSettingsMenu.SetQuotaTab))]
    private static bool SetQuotaTabPrefix(RolesSettingsMenu __instance)
    {
        float num = 0.662f;
        float num2 = -1.928f;
        __instance.roleTabs = new();
        __instance.roleTabs.Add(__instance.AllButton);
        var list = CustomRoleManager.AllRoles.Where(r => r is not ICustomRole && r.TeamType == RoleTeamTypes.Crewmate && r.Role != RoleTypes.Crewmate && r.Role != RoleTypes.CrewmateGhost).ToList();
        var list2 = CustomRoleManager.AllRoles.Where(r => r is not ICustomRole && r.TeamType == RoleTeamTypes.Impostor && r.Role != RoleTypes.Impostor && r.Role != RoleTypes.ImpostorGhost).ToList();
        for (int i = 0; i < list.Count; i++)
        {
            __instance.AddRoleTab(list[i], ref num2);
        }
        for (int j = 0; j < list2.Count; j++)
        {
            __instance.AddRoleTab(list2[j], ref num2);
        }
        CategoryHeaderEditRole categoryHeaderEditRole = Object.Instantiate<CategoryHeaderEditRole>(__instance.categoryHeaderEditRoleOrigin, Vector3.zero, Quaternion.identity, __instance.RoleChancesSettings.transform);
        categoryHeaderEditRole.SetHeader(StringNames.CrewmateRolesHeader, 20);
        categoryHeaderEditRole.transform.localPosition = new Vector3(4.986f, num, -2f);
        num -= 0.522f;
        int num3 = 0;
        for (int k = 0; k < list.Count; k++)
        {
            __instance.CreateQuotaOption(list[k], ref num, num3);
            num3++;
        }
        num -= 0.22f;
        CategoryHeaderEditRole categoryHeaderEditRole2 = Object.Instantiate<CategoryHeaderEditRole>(__instance.categoryHeaderEditRoleOrigin, Vector3.zero, Quaternion.identity, __instance.RoleChancesSettings.transform);
        categoryHeaderEditRole2.SetHeader(StringNames.ImpostorRolesHeader, 20);
        categoryHeaderEditRole2.transform.localPosition = new Vector3(4.986f, num, -2f);
        num -= 0.522f;
        for (int l = 0; l < list2.Count; l++)
        {
            __instance.CreateQuotaOption(list2[l], ref num, num3);
            num3++;
        }

        return false;
    }
}
