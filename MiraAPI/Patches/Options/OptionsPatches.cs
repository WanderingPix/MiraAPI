﻿using HarmonyLib;
using Il2CppSystem;
using MiraAPI.GameModes;
using MiraAPI.GameOptions;
using MiraAPI.Utilities;
using Reactor.Utilities.Extensions;

namespace MiraAPI.Patches.Options;

[HarmonyPatch]
public static class OptionsPatches
{
    [HarmonyPrefix]
    [HarmonyPatch(typeof(ToggleOption), nameof(ToggleOption.Initialize))]
    public static bool ToggleInit(ToggleOption __instance)
    {
        if (!__instance.IsCustom())
        {
            return true;
        }

        __instance.TitleText.text =
            TranslationController.Instance.GetString(__instance.Title, Array.Empty<Object>());

        return false;
    }

    [HarmonyPrefix]
    [HarmonyPatch(typeof(ToggleOption), nameof(ToggleOption.UpdateValue))]
    public static bool ToggleUpdate(ToggleOption __instance)
    {
        return !__instance.IsCustom();
    }

    [HarmonyPrefix]
    [HarmonyPatch(typeof(NumberOption), nameof(NumberOption.Initialize))]
    public static bool NumberInit(NumberOption __instance)
    {
        if (!__instance.IsCustom())
        {
            return true;
        }

        if (__instance == OptionGroupSingleton<GameModeOption>.Instance.CurrentMode.OptionBehaviour)
        {
            __instance.TitleText.text =
                TranslationController.Instance.GetString(__instance.Title, $"<color=#{CustomGameModeManager.ActiveMode?.Color.ToHtmlStringRGBA()}>{CustomGameModeManager.ActiveMode}</color>");
        }
        else
        {
            __instance.TitleText.text =
                TranslationController.Instance.GetString(__instance.Title);
        }

        return false;
    }

    [HarmonyPrefix]
    [HarmonyPatch(typeof(NumberOption), nameof(NumberOption.UpdateValue))]
    public static bool NumberUpdate(NumberOption __instance)
    {
        return !__instance.IsCustom();
    }

    [HarmonyPrefix]
    [HarmonyPatch(typeof(StringOption), nameof(StringOption.Initialize))]
    public static bool StringInit(StringOption __instance)
    {
        if (!__instance.IsCustom())
        {
            return true;
        }

        __instance.TitleText.text =
            TranslationController.Instance.GetString(__instance.Title, Array.Empty<Object>());
        __instance.ValueText.text = TranslationController.Instance.GetString(
            __instance.Values[__instance.Value],
            Array.Empty<Object>());

        return false;
    }

    [HarmonyPrefix]
    [HarmonyPatch(typeof(StringOption), nameof(StringOption.UpdateValue))]
    public static bool StringUpdate(StringOption __instance)
    {
        return !__instance.IsCustom();
    }
}
