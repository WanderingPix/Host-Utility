using System.Collections.Generic;
using AmongUs.GameOptions;
using HarmonyLib;
using Rewired.Utils;
using UnityEngine;

namespace HostUtility.Patches;

[HarmonyPatch]
public class AntiCheatPatch
{
    [HarmonyPatch(typeof(PlayerControl), nameof(PlayerControl.MurderPlayer))]
    [HarmonyPostfix]
    public static void PlayerControl_MurderPlayer_Postfix(PlayerControl __instance)
    {
        if (LobbyBehaviour.Instance.IsNullOrDestroyed()) return;
        if (AmongUsClient.Instance.AmHost) AmongUsClient.Instance.KickWithReason(__instance.Data.ClientId, "Suspected cheater (MurderPlayer called while in lobby)", true);
    }
    
    private static Dictionary<PlayerControl, float> MessageCooldowns = new Dictionary<PlayerControl, float>();
    [HarmonyPatch(typeof(ChatController), nameof(ChatController.AddChat))]
    [HarmonyPostfix]
    public static void ChatController_AddChat_Postfix(ChatController __instance, ref PlayerControl sourcePlayer)
    {
        if (!AmongUsClient.Instance.AmHost) return;
        if (!MessageCooldowns.TryGetValue(sourcePlayer, out var cooldown))
        {
            MessageCooldowns.Add(sourcePlayer, 2f);
            return;
        }

        if (cooldown > 0f)
        {
            AmongUsClient.Instance.KickWithReason(sourcePlayer.Data.ClientId, "Bypassing message cooldowns", true);
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
}