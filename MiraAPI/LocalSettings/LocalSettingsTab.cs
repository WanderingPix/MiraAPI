using System.Collections.Generic;
using System.Linq;
using HarmonyLib;
using MiraAPI.Utilities.Assets;
using Reactor.Utilities.Extensions;
using TMPro;
using UnityEngine;
using UnityEngine.Events;

namespace MiraAPI.LocalSettings;

/// <summary>
/// Represents a local settings tab. Gets a button by default
/// </summary>
public class LocalSettingsTab
{
    public string TabName { get; }
    public Color TabColor { get; }
    public Color TabHoverColor { get; }
    public Color TabActiveColor { get; }
    public LoadableAsset<Sprite>? TabIcon { get; }

    public TabGroup? TabButton { get; protected set; }
    private bool ShouldCreateButton { get; set; }

    public List<ILocalSetting> Settings { get; }

    /// <inheritdoc />
    public LocalSettingsTab(string tabName) : this(
        tabName,
        Color.white,
        Palette.AcceptedGreen,
        Palette.AcceptedGreen,
        createButton: true)
    {
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="LocalSettingsTab"/> class.
    /// </summary>
    /// <param name="tabName">The name of the tab.</param>
    /// <param name="tabColor">The color of the tab.</param>
    /// <param name="tabHoverColor">The hover color of the tab.</param>
    /// <param name="tabActiveColor">The active color of the tab.</param>
    /// <param name="tabIcon">The tab icon. Defaults to no icon.</param>
    /// <param name="createButton">Should the tab button be created. Without it the tab needs to be opened manually.</param>
    public LocalSettingsTab(string tabName, Color tabColor, Color tabHoverColor, Color tabActiveColor, LoadableAsset<Sprite>? tabIcon = null, bool createButton = false)
    {
        TabName = tabName;
        TabColor = tabColor;
        TabHoverColor = tabHoverColor;
        TabActiveColor = tabActiveColor;
        TabIcon = tabIcon;
        ShouldCreateButton = createButton;
        Settings = new();
    }

    public GameObject? CreateTab(OptionsMenuBehaviour instance, ref int tabIdx, ref float offset)
    {
        var tab = Object.Instantiate(instance.transform.FindChild("GeneralTab").gameObject, instance.transform);
        tab.name = $"{TabName}Tab";
        tab.transform.DestroyChildren();
        tab.gameObject.SetActive(false);

        if (TabButton != null)
        {
            TabButton.Content = tab.gameObject;
        }

        var toggle = instance.transform.FindChild("GeneralTab").FindChild("ChatGroup").FindChild("CensorChatButton").GetComponent<ToggleButtonBehaviour>();
        var slider = instance.transform.FindChild("GeneralTab").FindChild("SoundGroup").FindChild("SFXSlider").GetComponent<SlideBar>();

        float contentOffset = 0;
        var contentOrder = 1;
        var contentIndex = 1;
        foreach (var setting in Settings)
        {
            setting.CreateOption(
                toggle,
                slider,
                tab.transform,
                ref contentOffset,
                ref contentOrder,
                contentIndex == Settings.Count);
            contentIndex++;
        }

        if (ShouldCreateButton)
        {
            var tabButtonObject = Object.Instantiate(instance.Tabs[0], instance.transform);
            tabButtonObject.name = $"{TabName} Button";
            tabButtonObject.transform.localPosition = new Vector3(2.4f, 2.1f - offset, 5.5f);
            tabButtonObject.transform.localScale = new Vector3(1.25f, 1.25f, 1);
            tabButtonObject.Button.color = TabColor;
            TabButton = tabButtonObject;

            var tabButtonText = tabButtonObject.transform.FindChild("Text_TMP").GetComponent<TextMeshPro>();
            tabButtonText.GetComponent<TextTranslatorTMP>().Destroy(); // i hate text translators
            tabButtonText.transform.localPosition = new Vector3(0.078f, 0, 0);
            tabButtonText.transform.localScale = new Vector3(0.9f, 0.9f);
            tabButtonText.alignment = TextAlignmentOptions.Right;
            tabButtonText.text = $"<b>{GetShortName(TabName)}</b>";

            var tabButton = tabButtonObject.GetComponent<PassiveButton>();
            var rollover = tabButtonObject.Rollover;
            rollover.OverColor = TabColor;
            rollover.OutColor = TabColor;

            SpriteRenderer tabButtonRend = null;
            if (TabIcon != null)
            {
                tabButtonRend = new GameObject("sprite").AddComponent<SpriteRenderer>();
                tabButtonRend.gameObject.layer = 5; // ui layer
                tabButtonRend.transform.SetParent(tabButtonObject.transform);
                tabButtonRend.transform.localPosition = new Vector3(0.5f, 0, -2);
                tabButtonRend.transform.localScale = new Vector3(0.5f, 0.5f, 1);
                tabButtonRend.sprite = TabIcon.LoadAsset();
                tabButtonText.gameObject.SetActive(false);
            }

            int tabIndex = tabIdx; // Local copies of the tab variables so we can use them in the lambdas
            float tabOffset = offset;
            tabButton.OnClick.AddListener((UnityAction)(() =>
            {
                instance.OpenTabGroup(tabIndex + 10);
            }));
            tabButton.OnMouseOver.AddListener((UnityAction)(() =>
            {
                tabButton.transform.localPosition = new Vector3(3.55f, 2.1f - tabOffset, 5.5f);

                if (tabButtonRend != null)
                {
                    tabButtonRend.gameObject.SetActive(false);
                }
                tabButtonText.gameObject.SetActive(true);
                tabButtonText.transform.localPosition = new Vector3(0.045f, 0, 0);
                tabButtonText.maxVisibleCharacters = int.MaxValue;
                tabButtonText.text = TabName;
            }));
            tabButton.OnMouseOut.AddListener((UnityAction)(() =>
            {
                tabButton.transform.localPosition = new Vector3(2.4f, 2.1f - tabOffset, 5.5f);

                if (tabButtonRend != null)
                {
                    tabButtonRend.gameObject.SetActive(true);
                    tabButtonText.gameObject.SetActive(false);
                }
                tabButtonText.transform.localPosition = new Vector3(0.1f, 0, 0);
                tabButtonText.maxVisibleCharacters = 4;
                tabButtonText.text = $"<b>{GetShortName(TabName)}</b>";
            }));
        }

        return tab;
    }

    private static string GetShortName(string name)
    {
        string shortName = string.Empty;
        name.Split(' ').Do(x => shortName += x[0]);
        return shortName;
    }
}
