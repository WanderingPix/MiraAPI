﻿using System;
using System.Linq;
using BepInEx.Configuration;
using MiraAPI.Utilities;
using Reactor.Utilities.Extensions;
using TMPro;
using UnityEngine;
using UnityEngine.Events;
using Object = UnityEngine.Object;

namespace MiraAPI.LocalSettings.SettingTypes;

/// <summary>
/// Local setting class for enums.
/// </summary>
public class LocalEnumSetting : LocalSettingBase<int>
{
    /// <summary>
    /// Gets the enum type of the setting.
    /// </summary>
    public Type EnumType { get; }

    /// <summary>
    /// Gets the enum values.
    /// </summary>
    public string[] Values { get; }

    /// <summary>
    /// Initializes a new instance of the <see cref="LocalEnumSetting"/> class.
    /// </summary>
    /// <inheritdoc/>
    /// <param name="enumType">The enum type.</param>
    /// <param name="values">The optional values array to replace the enum names.</param>
    public LocalEnumSetting(
        Type tab,
        ConfigEntryBase configEntry,
        Type enumType,
        string? name = null,
        string? description = null,
        string[]? values = null)
        : base(tab, configEntry, name, description)
    {
        EnumType = enumType;
        Values = values ?? Enum
            .GetValues(configEntry.SettingType)
            .Cast<Enum>()
            .Select(x => x.ToDisplayString())
            .ToArray();
    }

    /// <inheritdoc />
    public override GameObject CreateOption(ToggleButtonBehaviour toggle, SlideBar slider, Transform parent, ref float offset, ref int order, bool last)
    {
        var button = Object.Instantiate(toggle, parent).GetComponent<PassiveButton>();
        var tmp = button.transform.FindChild("Text_TMP").GetComponent<TextMeshPro>();
        var rollover = button.GetComponent<ButtonRolloverHandler>();
        tmp.GetComponent<TextTranslatorTMP>().Destroy();
        button.gameObject.SetActive(true);

        var toggleComp = button.GetComponent<ToggleButtonBehaviour>();
        var background = toggleComp.Background;
        var highlight = button.transform.FindChild("ButtonHighlight")?.GetComponent<SpriteRenderer>();
        if (highlight != null)
        {
            highlight.color = Tab!.TabAppearance.EnumHoverColor;
            highlight.gameObject.SetActive(false);
        }
        toggleComp.Destroy();

        if (last && order == 1)
        {
            // Button in the middle
            button.transform.localPosition = new Vector3(0, 1.85f - offset, -7);
        }
        else
        {
            button.transform.localPosition = new Vector3(order == 1 ? -1.185f : 1.185f, 1.85f - offset, -7);
        }

        tmp.text = GetValueText();
        button.name = Name;
        button.OnClick = new UnityEngine.UI.Button.ButtonClickedEvent();
        rollover.OutColor = Tab!.TabAppearance.EnumColor;
        rollover.OverColor = Tab!.TabAppearance.EnumHoverColor;
        background.color = Tab!.TabAppearance.EnumColor;

        button.OnClick.AddListener((UnityAction)(() =>
        {
            int value = GetValue();
            value++;
            if (value >= Values.Length)
            {
                value = 0;
            }

            SetValue(value);
            tmp.text = GetValueText();
        }));
        button.OnMouseOver.AddListener((UnityAction)(() =>
        {
            if (!Description.IsNullOrWhiteSpace())
            {
                tmp.text = Description;
            }
            highlight?.gameObject.SetActive(true);
        }));
        button.OnMouseOut.AddListener((UnityAction)(() =>
        {
            tmp.text = GetValueText();
            highlight?.gameObject.SetActive(false);
        }));

        Helpers.DivideSize(button.gameObject, 1.1f);

        order++;
        if (order > 2 && !last)
        {
            offset += 0.5f;
            order = 1;
        }
        if (last)
            offset += 0.6f;

        return button.gameObject;
    }

    /// <inheritdoc/>
    protected override string GetValueText()
    {
        return $"<font=\"LiberationSans SDF\" material=\"LiberationSans SDF - Chat Message Masked\">{Name}: <b>{Values[GetValue()]}</font></b>";
    }
}
