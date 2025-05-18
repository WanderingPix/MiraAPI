using HarmonyLib;
using MiraAPI.Example.Buttons.Freezer;
using MiraAPI.Example.Extension.Options;
using MiraAPI.GameOptions;

namespace MiraAPI.Example.Extension.Patches;

[HarmonyPatch(typeof(FreezeButton), nameof(FreezeButton.MaxUses), MethodType.Getter)]
public static class FreezeButtonPatch
{
    public static void Postfix(ref int __result)
    {
        __result = (int)OptionGroupSingleton<ExtendedFreezerOptions>.Instance.FreezeUses;
    }
}
