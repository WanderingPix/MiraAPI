﻿using System;
using BepInEx.Configuration;
using MiraAPI.Utilities;
using Reactor.Utilities.Extensions;
using TMPro;
using UnityEngine;
using UnityEngine.Events;
using Object = UnityEngine.Object;

namespace MiraAPI.LocalSettings.SettingTypes;

/// <summary>
/// Local setting class for sliders.
/// </summary>
public class LocalSliderSetting : LocalSettingBase<float>
{
    /// <summary>
    /// Gets the range of the slider.
    /// </summary>
    public FloatRange SliderRange { get; }

    /// <summary>
    /// Gets a value indicating whether the value should be displayed next to name.
    /// </summary>
    public bool DisplayValue { get; }

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
    /// Initializes a new instance of the <see cref="LocalSliderSetting"/> class.
    /// </summary>
    /// <inheritdoc/>
    /// <param name="sliderRange">The value range.</param>
    /// <param name="suffixType">The suffix used for formating.</param>
    /// <param name="formatString">The format string used for formating.</param>
    /// <param name="roundValue">Should the value be rounded.</param>
    public LocalSliderSetting(
        Type tab,
        ConfigEntryBase configEntry,
        string? name = null,
        string? description = null,
        FloatRange? sliderRange = null,
        bool displayValue = false,
        MiraNumberSuffixes? suffixType = null,
        string? formatString = null,
        bool roundValue = false)
        : base(tab, configEntry, name, description)
    {
        SliderRange = sliderRange ?? new FloatRange(0, 100);
        DisplayValue = displayValue;
        SuffixType = suffixType ?? MiraNumberSuffixes.None;
        FormatString = formatString ?? "0.0";
        RoundValue = roundValue;
    }

    /// <inheritdoc />
    public override GameObject CreateOption(ToggleButtonBehaviour toggle, SlideBar slider, Transform parent, ref float offset, ref int order, bool last)
    {
        var newSlider = Object.Instantiate(slider, parent).GetComponent<SlideBar>();
        var rollover = newSlider.GetComponent<ButtonRolloverHandler>();
        newSlider.Title = newSlider.transform.FindChild("Text_TMP").GetComponent<TextMeshPro>(); // Why the hell slider has a title property that is not even assigned???
        newSlider.Title.GetComponent<TextTranslatorTMP>().Destroy();
        newSlider.gameObject.SetActive(true);
        newSlider.Bar.color = Tab!.TabAppearance.SliderColor;

        if (order == 2)
            offset += 0.5f;

        newSlider.Bar.transform.localPosition = new Vector3(2.85f, 0, 0);
        newSlider.transform.localPosition = new Vector3(-2.12f, 1.85f - offset, -7);
        newSlider.name = Name;
        newSlider.Range = new FloatRange(-1.5f, 1.5f);
        newSlider.SetValue(Mathf.InverseLerp(SliderRange.min, SliderRange.max, GetValue()));
        rollover.OutColor = Tab!.TabAppearance.SliderColor;
        rollover.OverColor = Tab!.TabAppearance.SliderHoverColor;
        newSlider.Title.transform.localPosition = new Vector3(0.5f, 0, -1f);
        newSlider.Title.horizontalAlignment = DisplayValue ? HorizontalAlignmentOptions.Left : HorizontalAlignmentOptions.Center;
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

    /// <inheritdoc/>
    protected override string GetValueText()
    {
        if (DisplayValue)
        {
            var value = GetValue();
            var formated = Helpers.FormatValue(value, SuffixType, FormatString);
            var maxFormated = Helpers.FormatValue(SliderRange.max, SuffixType, FormatString);
            return $"<font=\"LiberationSans SDF\" material=\"LiberationSans SDF - Chat Message Masked\">{Name}: <b>{formated} / {maxFormated}</font></b>";
        }

        return $"<font=\"LiberationSans SDF\" material=\"LiberationSans SDF - Chat Message Masked\">{Name}</font></b>";
    }
}
