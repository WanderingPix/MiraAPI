using HarmonyLib;
using MiraAPI.Events;
using MiraAPI.Events.Vanilla.Chat;
using UnityEngine;

namespace MiraAPI.Patches;

#pragma warning disable SA1629
/// <summary>
/// Allows players to paste text into chat.
/// Source: https://github.com/CallOfCreator/NewMod/blob/main/NewMod/Patches/ClipboardPatch.cs
/// </summary>
#pragma warning restore SA1629
[HarmonyPatch(typeof(ChatController))]
public static class ChatControllerPatch
{
    [HarmonyPatch(nameof(ChatController.Update))]
    [HarmonyPrefix]
    public static void UpdatePrefix(ChatController __instance)
    {
        if (!HudManager.Instance.Chat.IsOpenOrOpening) return;

        var ctrlPressed = Input.GetKey(KeyCode.LeftControl) || Input.GetKey(KeyCode.RightControl);

        if (!ctrlPressed || !Input.GetKeyDown(KeyCode.V)) return;

        var clipboard = GUIUtility.systemCopyBuffer;

        if (string.IsNullOrWhiteSpace(clipboard)) return;
        clipboard = clipboard.Replace("<", string.Empty)
            .Replace(">", string.Empty)
            .Replace("\r", string.Empty);

        if (!Input.GetKey(KeyCode.LeftShift) && !Input.GetKey(KeyCode.RightShift))
            clipboard = clipboard.Replace("\n", string.Empty);

        __instance.freeChatField.textArea.SetText(__instance.freeChatField.textArea.text + clipboard);
    }

    [HarmonyPatch(nameof(ChatController.Toggle))]
    [HarmonyPostfix]
    public static void TogglePostfix(ChatController __instance)
    {
        if (__instance.IsOpenOrOpening) MiraEventManager.InvokeEvent(new OpenChatEvent(__instance));
        else MiraEventManager.InvokeEvent(new CloseChatEvent(__instance));
    }

    [HarmonyPatch(nameof(ChatController.AddChat))]
    [HarmonyPrefix]
    public static bool AddChatPrefix(ChatController __instance, ref PlayerControl sourcePlayer, ref string chatText)
    {
        var @event = new BeforeSendChatMessageEvent(__instance, chatText, sourcePlayer);
        MiraEventManager.InvokeEvent(@event);
        if (@event.IsCancelled) return false;
        return true;
    }

    [HarmonyPatch(typeof(ChatBubble), nameof(ChatBubble.SetText))]
    [HarmonyPostfix]
    public static void SetTextPostfix(ChatBubble __instance, string chatText)
    {
        var @event = new AfterSendChatMessageEvent(__instance, __instance.playerInfo.Object, chatText);
        MiraEventManager.InvokeEvent(@event);
    }
}
