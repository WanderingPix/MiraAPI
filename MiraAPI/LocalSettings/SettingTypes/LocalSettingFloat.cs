using System;
using System.Globalization;
using BepInEx.Configuration;
using MiraAPI.Utilities;
using Reactor.Localization.Utilities;
using Reactor.Utilities.Extensions;
using TMPro;
using UnityEngine;
using UnityEngine.Events;
using Object = UnityEngine.Object;

namespace MiraAPI.LocalSettings.SettingTypes;

/// <summary>
/// Local setting class for toggles.
/// </summary>
public class LocalSettingFloat : LocalSettingBase<float>
{
    /// <summary>
    /// Gets the color of the slider.
    /// </summary>
    public Color SliderColor { get; }

    /// <summary>
    /// Gets the range of the slider.
    /// </summary>
    public FloatRange SliderRange { get; }

    /// <summary>
    /// Gets a format for the text to use to format the number.
    /// </summary>
    public string FormatString { get; }

    /// <summary>
    /// Gets a value indicating whether the value should be rounded.
    /// </summary>
    public bool RoundValue { get; }

    /// <summary>
    /// Gets the suffix for the number value.
    /// </summary>
    public MiraNumberSuffixes SuffixType { get; }

    /// <summary>
    /// Initializes a new instance of the <see cref="LocalSettingFloat"/> class.
    /// </summary>
    /// /// <param name="tab">Tab that the toggle belongs to.</param>
    /// <param name="configEntry">The binded config entry.</param>
    /// <param name="name">The name of the setting. Defaults to ConfigEntry key.</param>
    /// <param name="description">The desription of the setting. No description by default.</param>
    /// <param name="suffixType">Gets the suffix for the number value.</param>
    /// <param name="formatString">Gets a format for the text to use to format the number.</param>
    /// <param name="sliderColor">Gets the color of the slider.</param>
    /// <param name="sliderRange">Gets the range of the slider.</param>
    /// <param name="roundValue">Gets whether the value should be rounded.</param>
    public LocalSettingFloat(
        Type tab,
        ConfigEntryBase configEntry,
        string? name = null,
        string? description = null,
        FloatRange? sliderRange = null,
        MiraNumberSuffixes? suffixType = null,
        string? formatString = null,
        bool roundValue = true)
        : base(tab, configEntry, name, description)
    {
        SliderColor = Palette.AcceptedGreen;
        SuffixType = suffixType ?? MiraNumberSuffixes.None;
        SliderRange = sliderRange ?? new FloatRange(0, 100);
        RoundValue = roundValue;
        FormatString = formatString ?? "0.0";
    }

    /// <inheritdoc />
    public override GameObject? CreateOption(ToggleButtonBehaviour toggle, SlideBar slider, Transform parent, ref float offset, ref int order, bool last)
    {
        var newSlider = Object.Instantiate(slider, parent).GetComponent<SlideBar>();
        var rollover = newSlider.GetComponent<ButtonRolloverHandler>();
        newSlider.Title = newSlider.transform.FindChild("Text_TMP").GetComponent<TextMeshPro>(); // Why the hell slider has a title property that is not even assigned???
        newSlider.Title.GetComponent<TextTranslatorTMP>().Destroy();
        newSlider.gameObject.SetActive(true);

        if (order == 2)
            offset += 0.5f;

        newSlider.Bar.transform.localPosition = new Vector3(2.85f, 0, 0);
        newSlider.transform.localPosition = new Vector3(-2.12f, 1.85f - offset, -7);
        newSlider.name = Name;
        newSlider.Range = new FloatRange(-1.5f, 1.5f);
        newSlider.SetValue(Mathf.InverseLerp(SliderRange.min, SliderRange.max, GetValue()));
        rollover.OverColor = SliderColor;
        newSlider.Title.transform.localPosition = new Vector3(0.5f, 0, -1f);
        newSlider.Title.horizontalAlignment = HorizontalAlignmentOptions.Right;
        newSlider.Title.text = GetValueText();

        newSlider.OnValueChange.AddListener((UnityAction)(() =>
        {
            SetValue(RoundValue
                ? Mathf.Round(Mathf.Lerp(SliderRange.min, SliderRange.max, newSlider.Value))
                : Mathf.Lerp(SliderRange.min, SliderRange.max, newSlider.Value));

            newSlider.Title.text = GetValueText();
        }));

        order = 1;
        offset += 0.5f;
        return newSlider.gameObject;
    }

    private string FormatValue(float value)
    {
        return SuffixType switch
        {
            MiraNumberSuffixes.None => value.ToString(FormatString, NumberFormatInfo.InvariantInfo),
            MiraNumberSuffixes.Multiplier => value.ToString(FormatString, NumberFormatInfo.InvariantInfo) + "x",
            MiraNumberSuffixes.Percent => value.ToString(FormatString, NumberFormatInfo.InvariantInfo) + "%",
            _ => TranslationController.Instance.GetString(
                StringNames.GameSecondsAbbrev,
                (Il2CppSystem.Object[])[value.ToString(FormatString, CultureInfo.InvariantCulture)]),
        };
    }
    private string GetValueText()
    {
        var value = GetValue();
        return $"<font=\"LiberationSans SDF\" material=\"LiberationSans SDF - Chat Message Masked\">{Name}: <b>{FormatValue(value)} / {FormatValue(SliderRange.max)}</font></b>";
    }
}
