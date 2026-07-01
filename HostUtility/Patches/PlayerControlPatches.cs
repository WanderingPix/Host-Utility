using System.Linq;
using AmongUs.InnerNet.GameDataMessages;
using HarmonyLib;
using Hazel;
using HostUtility.AUFiles;
using Reactor.Utilities;

namespace HostUtility.Patches;

[HarmonyPatch]
public class PlayerControlPatches
{
    [HarmonyPatch(typeof(PlayerControl), nameof(PlayerControl.Start))]
    [HarmonyPostfix]
    public static void PlayerControl_Start_Postfix(PlayerControl __instance)
    {
        if (!AmongUsClient.Instance.AmHost) return;
        var plugin = PluginSingleton<HostUtilityPlugin>.Instance;
        __instance.StartCoroutine(Effects.ActionAfterDelay(0.2f, new System.Action(() =>
        {
            if (__instance == PlayerControl.LocalPlayer) return;

            if (BanWords.ContainsSwear(__instance.Data.PlayerName) && plugin.BanInappropriateNames.Value)
                AmongUsClient.Instance.KickWithReason(__instance.Data.ClientId, "Inappropriate username", true);
        })));
        __instance.StartCoroutine(Effects.ActionAfterDelay(1.5f, new System.Action(() =>
        {
            if (__instance.Data.PlayerLevel < plugin.MinLevel.Value)
                AmongUsClient.Instance.KickWithReason(__instance.Data.ClientId, "Low level", false);
        })));
        __instance.StartCoroutine(Effects.ActionAfterDelay(1f, new System.Action(() =>
        {
            if (plugin.KickSuspectedPlayers.Value && __instance.Data.FriendCode != string.Empty && AUFilesManager.Data.entries.Count(x => x.friend_code == __instance.Data.FriendCode) > 0)
                AmongUsClient.Instance.KickWithReason(__instance.Data.ClientId, "Suspected EDater/Pdf", false);
            if (FriendsListManager.Instance.IsPlayerBlocked(AmongUsClient.Instance.GetClient(__instance.Data.ClientId).ProductUserId)) 
                AmongUsClient.Instance.KickWithReason(__instance.Data.ClientId, "Blocked player", false);
        })));
        SendHUMessage(__instance);
    }

    private static void SendHUMessage(PlayerControl target)
    {
        if (target.AmOwner) return;
        var plugin = PluginSingleton<HostUtilityPlugin>.Instance;
        string message = $"<size=130%>This Lobby is running using Host Utility!{HostUtilityPlugin.Version}</size>\n" + 
                                                                              "Currently set options:\n" + 
                                                                              $"- Chat Message Filtering: {plugin.BanInappropriateMessages.Value}\n" +
                                                                              $"- Player Names Filtering: {plugin.BanInappropriateNames.Value}\n" +
                                                                              $"- Minimum Level required to play: {plugin.MinLevel.Value}\n" + 
                                                                              $"- Kick suspected e-daters and predators: {plugin.KickSuspectedPlayers.Value}" +
                                                                              $"- Game Start countdown time: {plugin.GameStartCountdownTime.Value}";
        var rpc = AmongUsClient.Instance.StartRpcImmediately(PlayerControl.LocalPlayer.NetId, (byte)RpcCalls.SendChat, SendOption.Reliable, (int)target.NetId);
        rpc.Write(message);
        AmongUsClient.Instance.FinishRpcImmediately(rpc);
    }
}