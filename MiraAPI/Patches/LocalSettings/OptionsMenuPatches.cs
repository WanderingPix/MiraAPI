using System.Collections.Generic;
using System.Linq;
using HarmonyLib;
using MiraAPI.LocalSettings;
using Reactor.Localization.Utilities;
using Reactor.Utilities.Extensions;
using TMPro;
using UnityEngine;
using UnityEngine.Events;

namespace MiraAPI.Patches.LocalSettings;

[HarmonyPatch(typeof(OptionsMenuBehaviour))]
public static class OptionsMenuPatches
{
    internal static BoxCollider2D MaskCollider;
    /// <summary>
    /// Creates the tabs and their content
    /// </summary>
    [HarmonyPostfix]
    [HarmonyPatch(nameof(OptionsMenuBehaviour.Start))]
    public static void StartPostfix(OptionsMenuBehaviour __instance)
    {
        // Fix for tabs not being clickable in the main menu
        if (!AmongUsClient.Instance.IsInGame)
        {
            __instance.Background.GetComponent<BoxCollider2D>().enabled = false;
            __instance.transform.FindChild("Tint").SetLocalZ(5.6f);
        }

        var maskObj = new GameObject
        {
            layer = 5,
            name = "SpriteMask",
        };
        maskObj.transform.SetParent(__instance.transform);
        maskObj.transform.localPosition = new Vector3(0, -0.3f, 0);
        maskObj.transform.localScale = new Vector3(500, 120, 1);

        var blank = Sprite.Create(
            Texture2D.whiteTexture,
            new Rect(0, 0, Texture2D.whiteTexture.width, Texture2D.whiteTexture.height),
            new Vector2(0.5f, 0.5f));

        var mask = maskObj.AddComponent<SpriteMask>();
        mask.sprite = blank;
        mask.isCustomRangeActive = true;
        mask.sortingLayerName = "Default";
        mask.backSortingOrder = 100;
        mask.frontSortingOrder = 500;

        MaskCollider = maskObj.AddComponent<BoxCollider2D>();
        MaskCollider.size = new Vector2(0.01f, 0.1f);
        MaskCollider.isTrigger = true;
        MaskCollider.enabled = true;

        float yOffset = 0;
        var i = 0;
        foreach (var settings in LocalSettingsManager.Tabs)
        {
            settings.CreateTab(__instance, ref i, ref yOffset);
        }
    }

    /// <summary>
    /// Modifies how opening tabs is handled for the custom ones to work
    /// </summary>
    [HarmonyPrefix]
    [HarmonyPatch(nameof(OptionsMenuBehaviour.OpenTabGroup))]
    public static bool OpenTabGroupPrefix(OptionsMenuBehaviour __instance, ref int index)
    {
        __instance.Tabs.ToList().ForEach(x => x.Close()); // Close all vanilla tabs
        LocalSettingsManager.Tabs.ForEach(CustomClose); // Close all mod tabs

        if (index >= 10) // Tabs with index 10 and above are the mod settings tabs
        {
            CustomOpen(LocalSettingsManager.Tabs[index - 10]);
            return false;
        }

        return true;
    }

    /// <summary>
    /// Makes the custom tabs close when opening the options menu
    /// </summary>
    [HarmonyPostfix]
    [HarmonyPatch(nameof(OptionsMenuBehaviour.Open))]
    public static void OpenPostfix(OptionsMenuBehaviour __instance)
    {
        LocalSettingsManager.Tabs.ForEach(CustomClose);
    }

    private static void CustomOpen(LocalSettingsTab tab)
    {
        if (tab.TabButton?.Button)
            tab.TabButton!.Button.color = tab.TabHoverColor;

        if (tab.TabButton?.Rollover)
            tab.TabButton!.Rollover.OutColor = tab.TabHoverColor;

        if (tab.TabButton?.Content)
            tab.TabButton!.Content.SetActive(true);
    }
    private static void CustomClose(LocalSettingsTab tab)
    {
        if (tab.TabButton?.Button)
            tab.TabButton!.Button.color = tab.TabColor;

        if (tab.TabButton?.Rollover)
            tab.TabButton!.Rollover.OutColor = tab.TabColor;

        if (tab.TabButton?.Content)
            tab.TabButton!.Content.SetActive(false);
    }
}
