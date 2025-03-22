// TODO: Remove later before completing the pr
#pragma warning disable
using System;
using System.Collections.Generic;
using AmongUs.GameOptions;
using Il2CppInterop.Runtime.Attributes;
using Il2CppInterop.Runtime.InteropTypes.Arrays;
using Reactor.Utilities.Attributes;

namespace MiraAPI.Modules;

[RegisterInIl2Cpp, Serializable]
public class MultiSelectGameSetting : BaseGameSetting
{
    public Int32OptionNames OptionName;
    public Dictionary<int, StringNames> EnumNames;

    public override string GetValueString(float value) => TranslationController.Instance.GetString(EnumNames[(int)value]);
}
