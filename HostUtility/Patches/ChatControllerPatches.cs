using System.Linq;
using HarmonyLib;
using Reactor.Utilities;

namespace HostUtility.Patches;

[HarmonyPatch(typeof(ChatController))]
public class ChatControllerPatches
{
    [HarmonyPatch(nameof(ChatController.AddChat))]
    [HarmonyPostfix]
    public static void ChatController_AddChat_Postfix(ChatController __instance, ref PlayerControl sourcePlayer, ref string chatText)
    {
        if (!AmongUsClient.Instance.AmHost) return;

        if (sourcePlayer.AmOwner)
        {
            foreach (var command in ChatCommandsManager.Commands)
            {
                if (chatText.StartsWith("/" + command.Command))
                {
                    var args = chatText.Split(" ").ToList();
                    args.RemoveAt(0);
                    command.HandleExecute(args.ToArray(), out bool success);
                }
            }
            return;
        }
        if (BanWords.ContainsSwear(chatText) && PluginSingleton<HostUtilityPlugin>.Instance.BanInappropriateMessages.Value)
        {
            AmongUsClient.Instance.KickWithReason(sourcePlayer.Data.ClientId, "Inappropriate Message", true);
        }
    }
}