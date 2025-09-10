using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using HarmonyLib;
using MiraAPI.LocalSettings;
using MiraAPI.Modifiers;
using MiraAPI.Modifiers.Types;
using MiraAPI.PluginLoading;
using MiraAPI.Roles;
using MiraAPI.Utilities.Assets;
using Reactor.Utilities.Extensions;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;
using Object = UnityEngine.Object;

namespace MiraAPI.Patches.Freeplay;

[HarmonyPatch(typeof(TaskAdderGame))]
internal static class TaskAdderPatches
{
    private static Scroller _scroller = null!;
    private static Dictionary<string, TaskFolder> folders = new();

    [HarmonyPostfix]
    [HarmonyPatch(nameof(TaskAdderGame.Begin))]
    public static void AddRolesFolder(TaskAdderGame __instance)
    {
        GameObject inner = new("Inner");
        inner.transform.SetParent(__instance.TaskParent.transform, false);

        _scroller = __instance.TaskParent.gameObject.AddComponent<Scroller>();
        _scroller.allowX = false;
        _scroller.allowY = true;
        _scroller.Inner = inner.transform;

        var scrollerHelper = __instance.TaskParent.gameObject.AddComponent<ManualScrollHelper>();
        scrollerHelper.scroller = _scroller;
        scrollerHelper.verticalAxis = RewiredConstsEnum.Action.TaskRVertical;
        scrollerHelper.scrollSpeed = 10f;

        GameObject hitbox = new("Hitbox")
        {
            layer = 5,
        };
        hitbox.transform.SetParent(__instance.TaskParent.transform, false);
        hitbox.transform.localScale = new Vector3(7.5f, 6.5f, 1);
        hitbox.transform.localPosition = new Vector3(2.8f, -2.2f, 0);

        var mask = hitbox.AddComponent<SpriteMask>();
        mask.sprite = MiraAssets.NextButton.LoadAsset();
        mask.alphaCutoff = 0.0f;

        var collider = hitbox.AddComponent<BoxCollider2D>();
        collider.size = new Vector2(1f, 1f);
        collider.enabled = true;

        _scroller.ClickMask = collider;

        __instance.TaskPrefab.GetComponent<PassiveButton>().ClickMask = collider;
        __instance.RoleButton.GetComponent<PassiveButton>().ClickMask = collider;
        __instance.RootFolderPrefab.GetComponent<PassiveButton>().ClickMask = collider;
        __instance.RootFolderPrefab.gameObject.SetActive(false);
        __instance.TaskParent = inner.transform;

        var crewmateFolder = __instance.Root.SubFolders.ToArray().FirstOrDefault(x => x.FolderName == "Crewmate")!;
        var impostorFolder = __instance.Root.SubFolders.ToArray().FirstOrDefault(x => x.FolderName == "Impostor")!;
        var neutralFolder = __instance.CreateFolder("Neutral", __instance.Root, 2, Color.gray);
        var modifiersFolder = __instance.CreateFolder("Modifiers", __instance.Root, 0, Color.blue);

        folders.Clear();
        folders.Add("Crewmate", crewmateFolder);
        folders.Add("Impostor", impostorFolder);
        folders.Add("Neutral", neutralFolder);
        folders.Add("Modifiers", modifiersFolder);

        int folderIdx = 3;
        foreach (var plugin in MiraPluginManager.Instance.RegisteredPlugins)
        {
            var pluginFolders = new Dictionary<string, TaskFolder>();

            foreach (var role in plugin.InternalRoles
                         .Where(x => x.Value is ICustomRole { Configuration.ShowInFreeplay: true })
                         .Select(x => x.Value))
            {
                var customRole = role as ICustomRole;
                var folderName = customRole!.Configuration.FreeplayFolder;
                if (!folders.TryGetValue(folderName, out var teamFolder))
                {
                    teamFolder = __instance.CreateFolder(folderName, __instance.Root, folderIdx++, customRole.IntroConfiguration?.IntroTeamColor ?? Color.gray);
                    folders.Add(folderName, teamFolder);
                }

                if (!pluginFolders.TryGetValue(folderName, out var pluginFolder))
                {
                    pluginFolder = __instance.CreateFolder(plugin.PluginInfo.Metadata.Name, teamFolder);
                    pluginFolders.Add(folderName, teamFolder);
                }

                pluginFolder.RoleChildren.Add(role);
            }

            if (plugin.InternalModifiers.Exists(x => x.ShowInFreeplay))
            {
                __instance.CreateFolder(plugin.PluginInfo.Metadata.Name, modifiersFolder)
                    .gameObject.name = plugin.PluginId;
            }
        }

        // Modifier folder at the end
        __instance.Root.SubFolders.Insert(folderIdx, modifiersFolder);

        __instance.GoToRoot();
    }

    private static TaskFolder CreateFolder(this TaskAdderGame instance, string name, TaskFolder parent, int idx = -1, Color? color = null)
    {
        var folder = Object.Instantiate(instance.RootFolderPrefab, instance.transform);
        folder.gameObject.SetActive(false);
        folder.FolderName = name;
        folder.SetCustomColor(color ?? new Color(0.937f, 0.811f, 0.592f));
        folder.name = name + "Folder";
        if (idx == -1)
            parent.SubFolders.Add(folder);
        else if (idx > 0)
            parent.SubFolders.Insert(idx, folder);
        return folder;
    }

    private static void SetCustomColor(this TaskFolder folder, Color color)
    {
        folder.currentFolderColor = color;
        folder.folderSpriteRenderer.color = color;
        folder.buttonRolloverHandler.OutColor = color;
        folder.buttonRolloverHandler.UnselectedColor = color;
    }

    private static void AddFileAsChildCustom(
        this TaskAdderGame instance,
        TaskAddButton item,
        ref float xCursor,
        ref float yCursor,
        ref float maxHeight)
    {
        item.transform.SetParent(instance.TaskParent);
        item.transform.localPosition = new Vector3(xCursor, yCursor, 0f);
        item.transform.localScale = Vector3.one;
        maxHeight = Mathf.Max(maxHeight, item.Text.bounds.size.y + 1.3f);
        xCursor += instance.fileWidth;
        if (xCursor > instance.lineWidth)
        {
            xCursor = 0f;
            yCursor -= maxHeight;
            maxHeight = 0f;
        }

        instance.ActiveItems.Add(item.transform);
    }

    private static bool IsChildOf(this TaskFolder child, TaskFolder parent) => parent.SubFolders
        .ToArray()
        .Any(x => x.FolderName == child.FolderName);

    // yes it might be crazy patching the entire method, but i tried so many other methods and only this works :cry:
    // true -chip
    [HarmonyPrefix]
    [HarmonyPatch(nameof(TaskAdderGame.ShowFolder))]
    public static bool ShowPatch(TaskAdderGame __instance, TaskFolder taskFolder)
    {
        StringBuilder stringBuilder = new StringBuilder(64);
        __instance.Hierarchy.Add(taskFolder);
        foreach (var t in __instance.Hierarchy)
        {
            stringBuilder.Append(t.FolderName);
            stringBuilder.Append('\\');
        }
        __instance.PathText.text = stringBuilder.ToString();
        var prettyEnabled = LocalSettingsTabSingleton<MiraApiSettings>.Instance.PrettyTaskAdder.Value;
        if (prettyEnabled)
        {
            __instance.PathText.fontSizeMin = 3;
            __instance.PathText.fontSizeMax = 3;
            __instance.transform.FindChild("TitleText_TMP")?.gameObject.DestroyImmediate();
        }

        __instance.ActiveItems.ToArray().Do(x => x.gameObject.Destroy());
        __instance.ActiveItems.Clear();

        float num = 0f;
        float num2 = 0f;
        float num3 = 0f;
        foreach (var t in taskFolder.SubFolders)
        {
            TaskFolder taskFolder2 = Object.Instantiate(t, __instance.TaskParent);
            taskFolder2.gameObject.SetActive(true);
            taskFolder2.Parent = __instance;
            taskFolder2.transform.localPosition = new Vector3(num, num2, 0f);
            taskFolder2.transform.localScale = Vector3.one;
            num3 = Mathf.Max(num3, taskFolder2.Text.bounds.size.y + 1.1f);
            num += __instance.folderWidth;
            if (num > __instance.lineWidth)
            {
                num = 0f;
                num2 -= num3;
                num3 = 0f;
            }

            __instance.ActiveItems.Add(taskFolder2.transform);
            if (taskFolder2 != null && taskFolder2.Button != null)
            {
                ControllerManager.Instance.AddSelectableUiElement(taskFolder2.Button);
            }
        }

        var list = taskFolder.TaskChildren
            .ToArray()
            .OrderBy(x => x.TaskType)
            .ToArray();
        foreach (var task in list)
        {
            TaskAddButton taskAddButton = Object.Instantiate(__instance.TaskPrefab);
            taskAddButton.MyTask = task;
            switch (task.TaskType)
            {
                case TaskTypes.DivertPower:
                {
                    var targetSystem = task.Cast<DivertPowerTask>().TargetSystem;
                    taskAddButton.Text.text = TranslationController.Instance.GetString(
                        StringNames.DivertPowerTo,
                        TranslationController.Instance.GetString(targetSystem));
                    break;
                }
                case TaskTypes.FixWeatherNode:
                {
                    var nodeId = task.Cast<WeatherNodeTask>().NodeId;
                    taskAddButton.Text.text =
                        TranslationController.Instance.GetString(
                            StringNames.FixWeatherNode) + " " +
                        TranslationController.Instance.GetString(
                            WeatherSwitchGame.ControlNames[nodeId]);
                    break;
                }
                default:
                    taskAddButton.Text.text =
                        TranslationController.Instance.GetString(task.TaskType);
                    break;
            }

            __instance.AddFileAsChildCustom(taskAddButton, ref num, ref num2, ref num3);
            if (taskAddButton != null && taskAddButton.Button != null)
            {
                ControllerManager.Instance.AddSelectableUiElement(taskAddButton.Button, false);
            }
        }

        var roleChildren = taskFolder.RoleChildren
            .ToArray()
            .Where(x => !(taskFolder.IsChildOf(__instance.Root) && x is ICustomRole))
            .ToArray();
        foreach (var role in roleChildren)
        {
            TaskAddButton roleAddButton = Object.Instantiate(__instance.RoleButton);
            roleAddButton.SafePositionWorld = __instance.SafePositionWorld;
            roleAddButton.Text.text = prettyEnabled ? role.NiceName : "Be_" + role.NiceName + ".exe";
            roleAddButton.Role = role;
            if (prettyEnabled)
            {
                roleAddButton.Text.fontSizeMin = 1;
                roleAddButton.FileImage.sprite = role.IsImpostor
                    ? MiraAssets.ImpostorFile.LoadAsset()
                    : MiraAssets.CrewmateFile.LoadAsset();
            }
            if (role is ICustomRole custom)
            {
                if (prettyEnabled && custom.Team == ModdedRoleTeams.Custom) roleAddButton.FileImage.sprite = MiraAssets.CustomTeamFile.LoadAsset();
                var customColor = custom.IntroConfiguration?.IntroTeamColor ?? Color.gray;
                if (custom.Team is ModdedRoleTeams.Crewmate)
                {
                    customColor = Palette.CrewmateBlue;
                }
                else if (custom.Team is ModdedRoleTeams.Impostor)
                {
                    customColor = Palette.ImpostorRed;
                }
                roleAddButton.FileImage.color = customColor;
                roleAddButton.RolloverHandler.OutColor = customColor;
            }
            __instance.AddFileAsChildCustom(roleAddButton, ref num, ref num2, ref num3);
            if (roleAddButton != null && roleAddButton.Button != null)
            {
                ControllerManager.Instance.AddSelectableUiElement(roleAddButton.Button);
            }
        }

        if (folders.TryGetValue("Modifiers", out var modifiersFolder) && taskFolder.IsChildOf(modifiersFolder))
        {
            var plugin = MiraPluginManager.GetPluginByGuid(taskFolder.gameObject.name.Replace("(Clone)", string.Empty));
            if (plugin != null)
            {
                foreach (var modifier in plugin.InternalModifiers)
                {
                    if (!modifier.ShowInFreeplay)
                    {
                        continue;
                    }

                    var taskAddButton = Object.Instantiate(__instance.RoleButton);
                    taskAddButton.name = modifier.TypeId.ToString(NumberFormatInfo.InvariantInfo);
                    taskAddButton.role = null;
                    taskAddButton.MyTask = null;
                    taskAddButton.SafePositionWorld = __instance.SafePositionWorld;
                    taskAddButton.Text.text = modifier.ModifierName;
                    taskAddButton.Text.fontSizeMin = 1;
                    taskAddButton.Text.EnableMasking();
                    taskAddButton.FileImage.color = modifier.FreeplayFileColor;
                    taskAddButton.RolloverHandler.OutColor = modifier.FreeplayFileColor;
                    if (modifier is TimedModifier timed)
                    {
                        taskAddButton.FileImage.sprite = MiraAssets.TimedModifierFile.LoadAsset();
                        taskAddButton.Text.text += $" ({timed.Duration}s)";
                    }
                    else
                    {
                        taskAddButton.FileImage.sprite = MiraAssets.ModifierFile.LoadAsset();
                    }

                    taskAddButton.Button.OnClick = new Button.ButtonClickedEvent();
                    var m = modifier;
                    taskAddButton.Button.OnClick.AddListener((UnityAction)(() =>
                    {
                        var id = m.TypeId;
                        if (PlayerControl.LocalPlayer.HasModifier(id))
                        {
                            PlayerControl.LocalPlayer.RemoveModifier(id);
                            taskAddButton.Overlay.enabled = false;
                        }
                        else
                        {
                            PlayerControl.LocalPlayer.AddModifier(id);
                            taskAddButton.Overlay.enabled = true;
                        }
                    }));

                    __instance.AddFileAsChildCustom(taskAddButton, ref num, ref num2, ref num3);

                    ControllerManager.Instance.AddSelectableUiElement(taskAddButton.Button);
                }
            }
        }

        if (_scroller)
        {
            _scroller.CalculateAndSetYBounds(__instance.ActiveItems.Count, 6, 4.5f, 1.65f);
            _scroller.SetYBoundsMin(0.0f);
            _scroller.ScrollToTop();
        }

        ControllerManager.Instance.SetBackButton(__instance.Hierarchy.Count == 1
            ? __instance.BackButton
            : __instance.FolderBackButton);

        __instance.SetPreviousControllerSelection(taskFolder.FolderName);
        return false;
    }
}
