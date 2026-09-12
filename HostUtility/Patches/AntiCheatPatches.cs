using System.Collections.Generic;
using AmongUs.GameOptions;
using HarmonyLib;
using HostUtility.Components;
using InnerNet;
using Hazel;
using Reactor.Utilities;
using Rewired.Utils;
using UnityEngine;

namespace HostUtility.Patches;

[HarmonyPatch]
public class AntiCheatPatch
{
    // Lobby murder & murder spam anticheat
    
    [HarmonyPatch(typeof(PlayerControl), nameof(PlayerControl.MurderPlayer))]
    [HarmonyPostfix]
    public static void PlayerControl_MurderPlayer_Postfix(PlayerControl __instance)
    {
        if (!AmongUsClient.Instance.AmHost) return;
        var trackingData = __instance.GetComponent<TrackingDataBehaviour>();
        if (trackingData.timeSinceLastMurder < 0.25f)
        {
            AmongUsClient.Instance.KickWithReason(__instance.Data.ClientId, "Spamming murder player RPC", "", true);
        }
        else trackingData.timeSinceLastMurder = 0;
        if (LobbyBehaviour.Instance) AmongUsClient.Instance.KickWithReason(__instance.Data.ClientId, "Attempting to murder player in lobby", "", true);
    }
    
    // SetColor spam anticheat
    
    [HarmonyPatch(typeof(PlayerControl), nameof(PlayerControl.SetColor))]
    [HarmonyPostfix]
    public static void PlayerControl_SetColor_Postfix(PlayerControl __instance)
    {
        if (!AmongUsClient.Instance.AmHost) return;
        var trackingData = __instance.GetComponent<TrackingDataBehaviour>();
        if (trackingData.timeSinceLastSetColor < 0.25f)
        {
            AmongUsClient.Instance.KickWithReason(__instance.Data.ClientId, "Spamming set color RPC", "", true);
        }
        else trackingData.timeSinceLastSetColor = 0;
    }
    
    //[HarmonyPatch(typeof(ShipStatus), nameof(ShipStatus.UpdateSystem))]
    //[HarmonyPostfix]
    //public static void ShipStatus_UpdateSystem_Postfix(ShipStatus __instance, ref SystemTypes systemType, ref PlayerControl player, ref byte amount)
    //{
    //    if (!AmongUsClient.Instance.AmHost) return;
    //    var trackingData = player.GetComponent<TrackingDataBehaviour>();
    //    if (trackingData.timeSinceLastUpdateSystem < 0.25f)
    //    {
    //        AmongUsClient.Instance.KickWithReason(player.Data.ClientId, "Spamming update system RPC", "", true);
    //    }
    //    else trackingData.timeSinceLastUpdateSystem = 0;
    //}

    // Meeting-call anticheat
    
    [HarmonyPatch(typeof(PlayerControl), nameof(PlayerControl.ReportDeadBody))]
    public static bool PlayerControl_ReportDeadBody_Postfix(PlayerControl __instance)
    {
        var amongUsClient = AmongUsClient.Instance;
        if (!amongUsClient.AmHost) return true;
        
        bool isCheating = false;
        string reason = "";
        if (LobbyBehaviour.Instance)
        {
            isCheating = true;
            reason = "Attempting to call meeting in lobby";
        }
        else if (amongUsClient.GameState == InnerNetClient.GameStates.Started)
        {
            if (GameManager.Instance.IsHideAndSeek())
            {
                isCheating = true;
                reason = "Attempting to call meeting in HnS";
            }

            if (MeetingHud.Instance)
            {
                isCheating = true;
                reason = "Attempting to call meeting in a meeting";
            }
            
            if (IntroCutscene.Instance)
            {
                isCheating = true;
                reason = "Attempting to call meeting in intro cutscene";
            }
            
            if (PlayerControl.LocalPlayer.Data.Role == null)
            {
                isCheating = true;
                reason = "Attempting to call meeting before role gen";
            }
        }

        if (isCheating)
        {
            amongUsClient.KickWithReason(__instance.Data.ClientId, reason, "", true);
        }
        return !isCheating;
    }
    
    private static Dictionary<PlayerControl, float> MessageCooldowns = new Dictionary<PlayerControl, float>();
    [HarmonyPatch(typeof(ChatController), nameof(ChatController.AddChat))]
    [HarmonyPostfix]
    public static void ChatController_AddChat_Postfix(ChatController __instance, ref PlayerControl sourcePlayer)
    {
        if (!AmongUsClient.Instance.AmHost) return;
        if (!MessageCooldowns.TryGetValue(sourcePlayer, out var cooldown))
        {
            MessageCooldowns.Add(sourcePlayer, 0);
        }
        else if (cooldown > 0f && PluginSingleton<HostUtilityPlugin>.Instance.CheckMessageCooldowns.Value)
        {
            AmongUsClient.Instance.KickWithReason(sourcePlayer.Data.ClientId, "Bypassing message cooldowns", "", true);
        }
    }

    //Checks RPCs sent for cheat menus.
    private static Dictionary<byte, string> CheatRpcDictionary = new()
    {
        { 202, "SlopMenuCrew" },
        { 201, "SlopMenuCrew" },
        { 121, "ChocooMenu" },
        { 250, "KillNetwork" },
        { 101, "SickoMenu" },
        { 164, "SickoMenu" },
        { 85, "AmongUsMenu" }
    };
    
    [HarmonyPatch(typeof(PlayerControl), nameof(PlayerControl.HandleRpc))]
    [HarmonyPostfix]
    public static void PlayerControl_HandleRpc_Prefix(PlayerControl __instance, ref byte callId, ref MessageReader reader)
    {
        if (!AmongUsClient.Instance.AmHost) return;
        foreach (byte cheatCallId in CheatRpcDictionary.Keys)
        {
            if (callId == cheatCallId)
            {
                if (CheatRpcDictionary.TryGetValue(cheatCallId, out string cheatName)) AmongUsClient.Instance.KickWithReason(__instance.Data.ClientId, "using " + cheatName, "", true);
                return;
            }
        }
    }

    [HarmonyPatch(typeof(ChatController), nameof(ChatController.Update))]
    [HarmonyPostfix]
    public static void ChatController_Update_Postfix()
    {
        var newCooldowns = new Dictionary<PlayerControl, float>();
        foreach (var data in MessageCooldowns)
        {
            if (data.Key == null) continue;
            newCooldowns.Add(data.Key, data.Value - Time.deltaTime);
        }
        MessageCooldowns = newCooldowns;
    }
    
    // Prevent any kicks against host from any source, including votekick
    
    [HarmonyPatch(typeof(InnerNetClient), nameof(InnerNetClient.KickPlayer))]
    [HarmonyPrefix]
    public static bool KickPlayer_Prefix(InnerNetClient __instance, int clientId, bool ban)
    {
        if (!__instance.AmHost) return false;
        if (!(clientId == __instance.ClientId))
        {
            Logger<HostUtilityPlugin>.Warning("Preventing kick against host!");
            return false;
        }
        return !(clientId == __instance.ClientId);
    }
}
