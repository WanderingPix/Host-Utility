using System.Linq;
using HarmonyLib;
using HostUtility.Components;
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

        if (BanWords.ContainsSwear(chatText) && HostUtilityPlugin.Instance.BanInappropriateMessages.Value)
        {
            AmongUsClient.Instance.KickWithReason(sourcePlayer.Data.ClientId, "Inappropriate Message", "",true);
        }
    }
    [HarmonyPatch(nameof(ChatController.Awake))]
    [HarmonyPostfix]
    public static void ChatController_Awake_Postfix(ChatController __instance)
    {
        Object.Instantiate(HostUtilityAssets.SuggestionsMenuPrefab, __instance.chatScreen.transform);
        Object.Instantiate(HostUtilityAssets.ArgumentsMenuPrefab, __instance.chatScreen.transform);
    }
}