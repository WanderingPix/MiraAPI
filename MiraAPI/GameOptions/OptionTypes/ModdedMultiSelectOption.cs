using System;
using System.Globalization;
using System.Linq;
using Il2CppInterop.Runtime.InteropTypes.Arrays;
using MiraAPI.Modules;
using MiraAPI.Networking;
using Reactor.Localization.Utilities;
using UnityEngine;
using Object = UnityEngine.Object;

namespace MiraAPI.GameOptions.OptionTypes;

/// <summary>
/// An option for selecting an enum value.
/// </summary>
/// <typeparam name="T">The enum type.</typeparam>
public class ModdedMultiSelectOption<T> : ModdedOption<MultiSelectValue<T>> where T : struct, Enum
{
    /// <summary>
    /// Gets the string values of the enum.
    /// </summary>
    public string[]? Values { get; }

    /// <summary>
    /// Gets a value indicating "all of the above".
    /// </summary>
    public T All { get; }

    /// <summary>
    /// Gets a value indicating "none of the above".
    /// </summary>
    public T None { get; }

    /// <summary>
    /// Initializes a new instance of the <see cref="ModdedMultiSelectOption{T}"/> class.
    /// </summary>
    /// <param name="title">The title of the option.</param>
    /// <param name="defaultValue">The default value as an int.</param>
    /// <param name="all">The all value.</param>
    /// <param name="none">The none value.</param>
    /// <param name="values">An option list of string values to use in place of the enum name.</param>
    public ModdedMultiSelectOption(string title, T defaultValue, T all, T none, string[]? values = null) : base(title, defaultValue)
    {
        Values = values ?? Enum.GetNames<T>();
        All = all;
        None = none;
        Data = ScriptableObject.CreateInstance<MultiSelectGameSetting>();
        var data = (MultiSelectGameSetting)Data;

        data.Title = StringName;
        data.Type = global::OptionTypes.String;
        data.Values = (values is null
            ? Enum.GetNames<T>()
            : values)
            .Select(CustomStringName.CreateAndRegister).ToArray();
    }

    /// <inheritdoc />
    public override OptionBehaviour CreateOption(ToggleOption toggleOpt, NumberOption numberOpt, StringOption stringOpt, Transform container)
    {
        var stringOption = Object.Instantiate(stringOpt, container);

        stringOption.SetUpFromData(Data, 20);
        stringOption.OnValueChanged = (Action<OptionBehaviour>)ValueChanged;

        // SetUpFromData method doesn't work correctly so we must set the values manually
        stringOption.Title = StringName;
        stringOption.Values = (Data as StringGameSetting)?.Values ?? new Il2CppStructArray<StringNames>(0);
        stringOption.Value = Convert.ToInt32(Value, NumberFormatInfo.InvariantInfo);

        OptionBehaviour = stringOption;

        return stringOption;
    }

    /// <inheritdoc />
    public override float GetFloatData()
    {
        return Convert.ToSingle(Value, NumberFormatInfo.InvariantInfo);
    }

    /// <inheritdoc />
    public override NetData GetNetData()
    {
        return new NetData(Id, Value.ToBytes());
    }

    /// <inheritdoc />
    public override void HandleNetData(byte[] data)
    {
        SetValue(MultiSelectValue<T>.FromBytes(data));
    }

    /// <inheritdoc />
    public override MultiSelectValue<T> GetValueFromOptionBehaviour(OptionBehaviour optionBehaviour)
    {
        return []; // TODO: Add implementation
    }

    /// <inheritdoc />
    protected override void OnValueChanged(MultiSelectValue<T> newValue)
    {
        HudManager.Instance.Notifier.AddSettingsChangeMessage(StringName, Data!.GetValueString(Convert.ToInt32(newValue, NumberFormatInfo.InvariantInfo)), false);
        if (!OptionBehaviour)
        {
            return;
        }

        if (OptionBehaviour is StringOption opt)
        {
            opt.Value = Convert.ToInt32(newValue, NumberFormatInfo.InvariantInfo);
        }
    }
}
