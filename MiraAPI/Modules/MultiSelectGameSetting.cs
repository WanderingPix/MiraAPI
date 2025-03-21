// TODO: Remove later before completing the pr
#pragma warning disable
using System;
using AmongUs.GameOptions;
using Il2CppInterop.Runtime.InteropTypes.Arrays;
using Reactor.Utilities.Attributes;

namespace MiraAPI.Modules;

[RegisterInIl2Cpp, Serializable]
public class MultiSelectGameSetting : BaseGameSetting
{
    public MultiSelectGameSetting() => Type = OptionTypes.MultipleChoice; // How nice of the game to provide an enum value themselves

    public override string GetValueString(float value)
    {
        return TranslationController.Instance.GetString(default(StringNames), null, null);
    }

    public Int32OptionNames OptionName;

    public Il2CppStructArray<StringNames> Values;
}
