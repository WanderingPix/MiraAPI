﻿using AmongUs.GameOptions;
using Il2CppInterop.Runtime;
using MiraAPI.Networking;
using MiraAPI.PluginLoading;
using Reactor.Localization.Utilities;
using Reactor.Networking.Rpc;
using Reactor.Utilities;
using Reactor.Utilities.Extensions;
using System;
using System.Collections.Generic;
using System.Linq;
using HarmonyLib;
using TMPro;
using UnityEngine;
using Object = UnityEngine.Object;

namespace MiraAPI.Roles;

public static class CustomRoleManager
{
    public static readonly Dictionary<ushort, RoleBehaviour> CustomRoles = new();

    private static int GetNextRoleId(ushort requestedId)
    {
        while (CustomRoles.ContainsKey(requestedId) || requestedId <= Enum.GetNames<RoleTypes>().Length)
        {
            Logger<MiraApiPlugin>.Error(requestedId + ", " + (requestedId+1));
            requestedId++;
        }

        return requestedId;
    }

    public static void RegisterInRoleManager()
    {
        RoleManager.Instance.AllRoles = RoleManager.Instance.AllRoles.Concat(CustomRoles.Values).ToArray();
    }

    internal static RoleBehaviour RegisterRole(Type roleType, ushort roleId, MiraPluginInfo parentMod)
    {
        if (!(typeof(RoleBehaviour).IsAssignableFrom(roleType) && typeof(ICustomRole).IsAssignableFrom(roleType)))
        {
            Logger<MiraApiPlugin>.Error($"{roleType?.Name} does not inherit from RoleBehaviour or ICustomRole.");
            return null;
        }

        var roleBehaviour = (RoleBehaviour)new GameObject(roleType.Name).DontDestroy().AddComponent(Il2CppType.From(roleType));

        if (roleBehaviour is not ICustomRole customRole)
        {
            roleBehaviour.gameObject.Destroy();
            return null;
        }

        roleBehaviour.Role = (RoleTypes)GetNextRoleId(roleId);
        roleBehaviour.TeamType = customRole.Team == ModdedRoleTeams.Neutral ? RoleTeamTypes.Crewmate : (RoleTeamTypes)customRole.Team;
        roleBehaviour.NameColor = customRole.RoleColor;
        roleBehaviour.StringName = CustomStringName.CreateAndRegister(customRole.RoleName);
        roleBehaviour.BlurbName = CustomStringName.CreateAndRegister(customRole.RoleDescription);
        roleBehaviour.BlurbNameLong = CustomStringName.CreateAndRegister(customRole.RoleLongDescription);
        roleBehaviour.AffectedByLightAffectors = customRole.AffectedByLight;
        roleBehaviour.CanBeKilled = customRole.CanGetKilled;
        roleBehaviour.CanUseKillButton = customRole.CanKill;
        roleBehaviour.TasksCountTowardProgress = customRole.TasksCount;
        roleBehaviour.CanVent = customRole.CanUseVent;
        roleBehaviour.DefaultGhostRole = customRole.GhostRole;
        roleBehaviour.MaxCount = 15;
        roleBehaviour.RoleScreenshot = customRole.OptionsScreenshot.LoadAsset();

        if (customRole.IsGhostRole)
        {
            RoleManager.GhostRoles.Add(roleBehaviour.Role);
        }

        CustomRoles.Add(roleId, roleBehaviour);

        if (customRole.HideSettings)
        {
            return roleBehaviour;
        }

        var config = parentMod.PluginConfig;
        config.Bind(customRole.NumConfigDefinition, 1);
        config.Bind(customRole.ChanceConfigDefinition, 100);

        return roleBehaviour;
    }

    public static MiraPluginInfo FindParentMod(ICustomRole role)
    {
        return MiraPluginManager.Instance.RegisteredPlugins.First(plugin => plugin.Value.CustomRoles.ContainsValue(role as RoleBehaviour)).Value;
    }

    public static bool GetCustomRoleBehaviour(RoleTypes roleType, out ICustomRole result)
    {
        CustomRoles.TryGetValue((ushort)roleType, out var temp);
        if (temp != null)
        {
            result = temp as ICustomRole;
            return true;
        }

        result = null;
        return false;
    }

    public static TaskPanelBehaviour CreateRoleTab(ICustomRole role)
    {
        var ogPanel = HudManager.Instance.TaskStuff.transform.FindChild("TaskPanel").gameObject.GetComponent<TaskPanelBehaviour>();
        var clonePanel = Object.Instantiate(ogPanel.gameObject, ogPanel.transform.parent);
        clonePanel.name = "RolePanel";

        var newPanel = clonePanel.GetComponent<TaskPanelBehaviour>();
        newPanel.open = false;

        var tab = newPanel.tab.gameObject;
        tab.GetComponentInChildren<TextTranslatorTMP>().Destroy();

        newPanel.transform.localPosition = ogPanel.transform.localPosition - new Vector3(0, 1, 0);

        UpdateRoleTab(newPanel, role);
        return newPanel;
    }

    public static void UpdateRoleTab(TaskPanelBehaviour panel, ICustomRole role)
    {
        var tabText = panel.tab.gameObject.GetComponentInChildren<TextMeshPro>();
        var ogPanel = HudManager.Instance.TaskStuff.transform.FindChild("TaskPanel").gameObject.GetComponent<TaskPanelBehaviour>();
        if (tabText.text != role.RoleName)
        {
            tabText.text = role.RoleName;
        }

        var y = ogPanel.taskText.textBounds.size.y + 1;
        panel.closedPosition = new Vector3(ogPanel.closedPosition.x, ogPanel.open ? y + 0.2f : 2f, ogPanel.closedPosition.z);
        panel.openPosition = new Vector3(ogPanel.openPosition.x, ogPanel.open ? y : 2f, ogPanel.openPosition.z);

        panel.SetTaskText(role.SetTabText().ToString());
    }
    
    public static void SyncAllRoleSettings(int targetId=-1)
    {
        List<NetData> data = [];
        int count = 0;
        foreach (var role in CustomRoles.Values)
        {
            ICustomRole customRole = role as ICustomRole;
            if (customRole is null or { HideSettings: true })
            {
                continue;
            }
            
            customRole.ParentMod.PluginConfig.TryGetEntry<int>(customRole.NumConfigDefinition, out var numEntry);
            customRole.ParentMod.PluginConfig.TryGetEntry<int>(customRole.ChanceConfigDefinition, out var chanceEntry);
            
            var netData = new NetData((uint)role.Role, BitConverter.GetBytes(numEntry.Value).AddRangeToArray(BitConverter.GetBytes(chanceEntry.Value)));
            
            data.Add(netData);
            count += netData.GetLength();
                
            if (count > 1000)
            {
                Rpc<SyncRoleOptionsRpc>.Instance.SendTo(PlayerControl.LocalPlayer, targetId, data.ToArray());
                data.Clear();
                count = 0;
            }
        }
        if (data.Count > 0)
        {
            Rpc<SyncRoleOptionsRpc>.Instance.SendTo(PlayerControl.LocalPlayer, targetId, data.ToArray());
        }
    }

    public static void HandleSyncRoleOptions(NetData[] data)
    {
        foreach (var netData in data)
        {
            if (CustomRoles.TryGetValue((ushort)netData.Id, out var role))
            {
                var customRole = role as ICustomRole;
                if (customRole is null or { HideSettings: true })
                {
                    continue;
                }
                
                var num = BitConverter.ToInt32(netData.Data, 0);
                var chance = BitConverter.ToInt32(netData.Data, 4);
                
                customRole.ParentMod.PluginConfig.TryGetEntry<int>(customRole.NumConfigDefinition, out var numEntry);
                customRole.ParentMod.PluginConfig.TryGetEntry<int>(customRole.ChanceConfigDefinition, out var chanceEntry);
                
                numEntry.Value = num;
                chanceEntry.Value = chance;
            }
        }
    }
}