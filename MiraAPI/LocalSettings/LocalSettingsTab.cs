using System;
using System.Collections.Generic;
using BepInEx.Configuration;
using HarmonyLib;
using MiraAPI.Patches.LocalSettings;
using MiraAPI.Utilities;
using MiraAPI.Utilities.Assets;
using Reactor.Utilities.Extensions;
using TMPro;
using UnityEngine;
using UnityEngine.Events;
using Object = UnityEngine.Object;

namespace MiraAPI.LocalSettings;

/// <summary>
/// Represents a local settings tab. Gets a button by default
/// </summary>
public abstract class LocalSettingsTab(ConfigFile config)
{
    /// <summary>
    /// Gets the name of the tab
    /// </summary>
    public abstract string TabName { get; }

    /// <summary>
    /// Gets the tab button icon.
    /// </summary>
    public virtual LoadableAsset<Sprite>? TabIcon => null;

    /// <summary>
    /// Gets the color of the tab button.
    /// </summary>
    public virtual Color TabColor => Color.white;

    /// <summary>
    /// Gets the color of the tab button when hovered.
    /// </summary>
    public Color TabHoverColor => Palette.AcceptedGreen;

    /// <summary>
    /// Gets the color of the tab button when active.
    /// </summary>
    public virtual Color TabActiveColor => Palette.AcceptedGreen;

    /// <summary>
    /// Gets a value indicating whether a button should be created for the tab.
    /// </summary>
    protected virtual bool ShouldCreateButton => true;

    /// <summary>
    /// Gets the mod's config file. Automatically found in the plugin loader.
    /// </summary>
    public ConfigFile Config { get; } = config ?? throw new ArgumentNullException(nameof(config));

    internal TabGroup? TabButton { get; set; }
    internal List<ILocalSetting> Settings { get; } = [];
    private Scroller? Scroller { get; set; }

    internal GameObject? CreateTab(OptionsMenuBehaviour instance, ref int tabIdx, ref float offset)
    {
         if (ShouldCreateButton)
         {
             CreateTabButton(instance, ref tabIdx, ref offset);
         }

         var tab = Object.Instantiate(instance.transform.FindChild("GeneralTab").gameObject, instance.transform);
         tab.name = $"{TabName}Tab";
         tab.transform.DestroyChildren();
         tab.gameObject.SetActive(false);

         if (Scroller == null)
         {
             Scroller = Helpers.CreateScroller(tab.transform, OptionsMenuPatches.MaskCollider);
         }

         if (TabButton != null)
         {
             TabButton.Content = tab.gameObject;
         }

         var generalLabel = instance.transform.FindChild("GeneralTab").FindChild("ControlGroup")
             .FindChild("ControlText_TMP").gameObject;
         var toggle = instance.transform.FindChild("GeneralTab").FindChild("ChatGroup").FindChild("CensorChatButton").GetComponent<ToggleButtonBehaviour>();
         var slider = instance.transform.FindChild("GeneralTab").FindChild("SoundGroup").FindChild("SFXSlider").GetComponent<SlideBar>();

         Dictionary<string, List<ILocalSetting>> entriesByGroup = new();
         foreach (var entry in Settings)
         {
             var group = entry.ConfigEntry.Definition.Section;
             if (!entriesByGroup.ContainsKey(group))
                 entriesByGroup.Add(group, []);

             entriesByGroup[group].Add(entry);
         }

         float contentOffset = 0;
         var contentOrder = 1;
         var contentIndex = 1;

         foreach (var pair in entriesByGroup)
         {
             CreateLabel(generalLabel, Scroller.Inner, pair.Key, ref contentOffset);

             foreach (var setting in pair.Value)
             {
                 var obj = setting.CreateOption(
                     toggle,
                     slider,
                     Scroller.Inner,
                     ref contentOffset,
                     ref contentOrder,
                     contentIndex == pair.Value.Count);

                 obj!.GetComponentsInChildren<SpriteRenderer>(true).Do(x =>
                 {
                     x.maskInteraction = SpriteMaskInteraction.VisibleInsideMask;
                     x.sortingOrder = 150;
                     x.sortingLayerName = "Default";
                 });

                 obj!.GetComponentsInChildren<MeshRenderer>(true).Do(x =>
                 {
                     x.sortingOrder = 151;
                     x.sortingLayerName = "Default";
                 });

                 obj.GetComponentsInChildren<PassiveButton>().Do(x => { x.ClickMask = OptionsMenuPatches.MaskCollider; });

                 contentIndex++;
             }

             contentOrder = 1;
         }

         Scroller.SetBounds(new FloatRange(0, Settings.Count * 0.5f - 5f), new FloatRange(0, 0));
         return tab;
    }

    private void CreateTabButton(OptionsMenuBehaviour instance, ref int tabIdx, ref float offset)
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

        SpriteRenderer? tabButtonRend = null;
        if (TabIcon != null)
        {
            tabButtonRend = new GameObject("sprite").AddComponent<SpriteRenderer>();
            tabButtonRend.gameObject.layer = 5; // ui layer
            tabButtonRend.transform.SetParent(tabButtonObject.transform);
            tabButtonRend.transform.localPosition = new Vector3(0.5f, 0, -2);
            tabButtonRend.transform.localScale = new Vector3(0.2f, 0.2f, 1);
            tabButtonRend.sprite = TabIcon.LoadAsset();
            tabButtonText.gameObject.SetActive(false);
        }

        var tabIndex = tabIdx; // Local copies of the tab variables so we can use them in the lambdas
        var tabOffset = offset;
        tabButton.OnClick.AddListener((UnityAction)(() => { instance.OpenTabGroup(tabIndex + 10); }));
        tabButton.OnMouseOver.AddListener((UnityAction)(() =>
        {
            tabButton.transform.localPosition = new Vector3(3.55f, 2.1f - tabOffset, 5.5f);
            tabButtonObject.Button.color = TabHoverColor;
            tabButtonObject.Rollover.OutColor = TabHoverColor;
            tabButtonText.gameObject.SetActive(true);
            tabButtonText.transform.localPosition = new Vector3(-0.024f, 0, 0);
            tabButtonText.maxVisibleCharacters = int.MaxValue;
            tabButtonText.text = TabName;
            tabButtonText.alignment = TextAlignmentOptions.Left;
        }));
        tabButton.OnMouseOut.AddListener((UnityAction)(() =>
        {
            if (!tabButtonObject.Content.gameObject.activeSelf)
            {
                tabButtonObject.Button.color = TabColor;
                tabButtonObject.Rollover.OutColor = TabColor;
            }

            tabButton.transform.localPosition = new Vector3(2.4f, 2.1f - tabOffset, 5.5f);
            tabButtonText.alignment = TextAlignmentOptions.Right;
            if (tabButtonRend != null)
            {
                tabButtonRend.gameObject.SetActive(true);
                tabButtonText.gameObject.SetActive(false);
            }

            tabButtonText.transform.localPosition = new Vector3(0.1f, 0, 0);
            tabButtonText.maxVisibleCharacters = 4;
            tabButtonText.text = $"<b>{GetShortName(TabName)}</b>";
        }));

        offset += 0.6f;
        tabIdx++;
    }

    private static void CreateLabel(GameObject template, Transform parent, string text, ref float offset)
    {
        var label = Object.Instantiate(template, parent);
        label.transform.localPosition = new Vector3(0, 1.85f - offset);
        label.GetComponent<TextTranslatorTMP>().Destroy();
        label.name = text;

        var tmp = label.GetComponent<TextMeshPro>();
        tmp.text = $"<font=\"LiberationSans SDF\" material=\"LiberationSans SDF - Chat Message Masked\"><b>{text}</b></font>";
        tmp.alignment = TextAlignmentOptions.Center;

        var meshRend = label.GetComponent<MeshRenderer>();
        meshRend.sortingLayerName = "Default";
        meshRend.sortingOrder = 151;

        offset += 0.5f;
    }


    private static string GetShortName(string name)
    {
        var shortName = string.Empty;
        name.Split(' ').Do(x => shortName += x[0]);
        return shortName;
    }
}

