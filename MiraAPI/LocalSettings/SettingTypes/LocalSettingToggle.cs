using BepInEx.Configuration;
using Reactor.Localization.Utilities;
using TMPro;
using UnityEngine;
using UnityEngine.Events;

namespace MiraAPI.LocalSettings.SettingTypes;

/// <summary>
/// Local setting class for toggles.
/// </summary>
public class LocalSettingToggle : LocalSettingBase<bool>
{
    /// <summary>
    /// Gets the color of the toggle.
    /// </summary>
    public Color ToggleColor { get; }

    /// <summary>
    /// Gets the color of the toggle when hovered.
    /// </summary>
    public Color ToggleHoverColor { get; }

    /// <summary>
    /// Gets the color of the toggle when enabled.
    /// </summary>
    public Color ToggleActiveColor { get; }

    /// <summary>
    /// Initializes a new instance of the <see cref="LocalSettingToggle"/> class.
    /// </summary>
    /// /// <param name="tab">Tab that the toggle belongs to.</param>
    /// <param name="configEntry">The binded config entry.</param>
    /// <param name="name">The name of the setting. Defaults to ConfigEntry key.</param>
    /// <param name="description">The desription of the setting. No description by default.</param>
    /// <param name="toggleColor">The color of the toggle.</param>
    /// <param name="toggleHoverColor">The hover color of the toggle.</param>
    /// <param name="toggleActiveColor">The active color of the toggle.</param>
    public LocalSettingToggle(
        LocalSettingsTab tab,
        ConfigEntryBase configEntry,
        string? name = null,
        string? description = null,
        Color? toggleColor = null,
        Color? toggleHoverColor = null,
        Color? toggleActiveColor = null)
        : base(tab, configEntry, name, description)
    {
        ToggleColor = toggleColor ?? Color.white;
        ToggleHoverColor = toggleHoverColor ?? Palette.AcceptedGreen;
        ToggleActiveColor = toggleActiveColor ?? Palette.AcceptedGreen;
    }

    public override GameObject? CreateOption(ToggleButtonBehaviour toggle, SlideBar slider, Transform parent, ref float offset, ref int order, bool last)
    {
        var toggleObject = Object.Instantiate(toggle, parent).GetComponent<ToggleButtonBehaviour>();
        var tmp = toggleObject.transform.FindChild("Text_TMP").GetComponent<TextMeshPro>();
        var passiveButton = toggleObject.GetComponent<PassiveButton>();
        var rollover = toggleObject.GetComponent<ButtonRolloverHandler>();
        toggleObject.gameObject.SetActive(true);

        if (last && order == 1)
        {
            // Toggle in the middle
            toggleObject.transform.localPosition = new Vector3(0, 1.85f - offset);
        }
        else
        {
            toggleObject.transform.localPosition = new Vector3(order == 1 ? -1.35f : 1.28f, 1.85f - offset);
        }

        toggleObject.BaseText = CustomStringName.CreateAndRegister(Name);
        toggleObject.UpdateText(GetValue());
        toggleObject.name = Name;
        toggleObject.Background.color = GetValue() ? ToggleActiveColor : ToggleColor;
        passiveButton.OnClick = new UnityEngine.UI.Button.ButtonClickedEvent();
        rollover.OverColor = ToggleHoverColor;

        passiveButton.OnClick.AddListener((UnityAction)(() =>
        {
            SetValue(!GetValue());
            toggleObject.UpdateText(GetValue());
            toggleObject.Background.color = GetValue() ? ToggleActiveColor : ToggleColor;
        }));
        passiveButton.OnMouseOver.AddListener((UnityAction)(() =>
        {
            if (!Description.IsNullOrWhiteSpace())
            {
                tmp.text = Description;
            }
        }));
        passiveButton.OnMouseOut.AddListener((UnityAction)(() =>
        {
            toggleObject.UpdateText(GetValue());
            toggleObject.Background.color = GetValue() ? ToggleActiveColor : ToggleColor;
        }));

        order++;
        if (order > 2 && !last)
        {
            offset += 0.5f;
            order = 1;
        }
        if (last)
            offset += 0.6f;

        return toggleObject.gameObject;
    }
}
