using System.Linq;
using HarmonyLib;
using HostUtility.Components;
using HostUtility.Components.UI;
using HostUtility.GameSettings;
using UnityEngine;

namespace HostUtility.Patches;

[HarmonyPatch(typeof(ChatController))]
public class ChatControllerPatches
{
    [HarmonyPatch(nameof(ChatController.AddChat))]
    [HarmonyPostfix]
    public static void ChatController_AddChat_Postfix(ChatController __instance, ref PlayerControl sourcePlayer, string chatText)
    {
        var trackingDataBehaviour = sourcePlayer.GetComponent<TrackingDataBehaviour>();
        trackingDataBehaviour.chatMessages = trackingDataBehaviour.chatMessages.Append(chatText).ToList();
        if (sourcePlayer.AmOwner)
        {
            if (chatText.StartsWith("/cmd"))
            {
                var cmd = ChatCommandsManager.Commands.Find(x => chatText.StartsWith($"/cmd {x.Command.ToLower()}"));
                cmd?.Execute(chatText.Replace("/cmd " + cmd.Command.ToLower() + " ", "").Split(","));
            }
        }
        if (!AmongUsClient.Instance.AmHost) return;

        if (BanWords.ContainsSwear(chatText) && SettingTabSingleton<ModerationTab>.Instance.BanInappropriateMessages.Value)
        {
            AmongUsClient.Instance.KickWithReason(sourcePlayer.Data.ClientId, "Inappropriate Message", "",true);
        }
    }

    public static ChatInputFieldButton cancelButton;
    [HarmonyPatch(nameof(ChatController.Awake))]
    [HarmonyPostfix]
    public static void ChatController_Awake_Postfix(ChatController __instance)
    {
        Object.Instantiate(HostUtilityAssets.SuggestionsMenuPrefab, __instance.chatScreen.transform);
        Object.Instantiate(HostUtilityAssets.ArgumentsMenuPrefab, __instance.chatScreen.transform);
        cancelButton = Object.Instantiate(__instance.freeChatField.submitButton, __instance.freeChatField.submitButton.transform.parent, true);
        cancelButton.text.text = "Cancel";
        cancelButton.text.color = new Color(1, 0.7f, 0.7f);
        foreach (var rend in cancelButton.backgroundSprites)
        {
            rend.color = new Color(1, 0.3f, 0.3f);
        }
        foreach (var icon in cancelButton.iconSprites)
        {
            icon.color = cancelButton.text.color;
            icon.flipX = !icon.flipX;
        }

        cancelButton.OnPressed = new System.Action(() =>
        {
            __instance.freeChatField.textArea.SetText(string.Empty);
            CommandArgumentsMenu.Instance.ForceClose();
        });
        Object.Destroy(cancelButton.text.GetComponent<TextTranslatorTMP>());
        cancelButton.gameObject.SetActive(false);
    }

    [HarmonyPatch(typeof(TextBoxTMP), nameof(TextBoxTMP.IsCharAllowed))]
    [HarmonyPostfix]
    public static void TextBoxTMP_IsCharAllowed_Postfix(TextBoxTMP __instance, ref bool __result, char i)
    {
        var s = i.ToString();
        if (__instance.allowAllCharacters) __result = s != "\b";
    }
    
    [HarmonyPatch(typeof(TextBoxTMP), nameof(TextBoxTMP.SetText))]
    [HarmonyPostfix]
    public static void SetTextPostfix(TextBoxTMP __instance)
    {
        var t = __instance.outputText;
        if (t != null && t.text.Contains("\r"))
            t.text = t.text.Replace("\r\n", "\n").Replace("\r", "\n");
    }
}