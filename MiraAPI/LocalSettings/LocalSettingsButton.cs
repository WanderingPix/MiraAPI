﻿using System;
using MiraAPI.Utilities;
using Reactor.Localization.Utilities;
using Reactor.Utilities.Extensions;
using TMPro;
using UnityEngine;
using UnityEngine.Events;
using Object = UnityEngine.Object;

namespace MiraAPI.LocalSettings;

/// <summary>
/// Represents a local settings button.
/// </summary>
public class LocalSettingsButton(string text, Action<LocalSettingsTab> onClick)
{
    /// <summary>
    /// Gets or sets the button text.
    /// </summary>
    public string Text { get; set; } = text;

    /// <summary>
    /// Gets or sets the button on-click action.
    /// </summary>
    public Action<LocalSettingsTab> OnClick { get; set; } = onClick;

    /// <summary>
    /// Gets the tab instance this button belongs to.
    /// </summary>
    public LocalSettingsTab Tab { get; internal set; }

    internal GameObject CreateButton(ToggleButtonBehaviour toggle, Transform parent, ref float offset, ref int order, bool last)
    {
        var button = Object.Instantiate(toggle, parent).GetComponent<PassiveButton>();
        var tmp = button.transform.FindChild("Text_TMP").GetComponent<TextMeshPro>();
        var rollover = button.GetComponent<ButtonRolloverHandler>();
        tmp.GetComponent<TextTranslatorTMP>().Destroy();
        button.gameObject.SetActive(true);

        var toggleComp = button.GetComponent<ToggleButtonBehaviour>();
        var background = toggleComp.Background;
        button.transform.FindChild("ButtonHighlight")?.gameObject.DestroyImmediate();
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

        tmp.text = Text;
        button.name = Text;
        button.OnClick = new UnityEngine.UI.Button.ButtonClickedEvent();
        rollover.OutColor = Tab!.TabAppearance.ButtonColor;
        rollover.OverColor = Tab!.TabAppearance.ButtonHoverColor;
        rollover.Target = background;
        background.color = Tab!.TabAppearance.ButtonColor;

        var tab = Tab;
        var click = OnClick;
        button.OnClick.AddListener((UnityAction)(() => click(tab)));

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
}
