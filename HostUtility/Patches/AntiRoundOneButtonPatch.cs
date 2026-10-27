using HarmonyLib;

namespace HostUtility.Patches;

[HarmonyPatch]
public static class AntiRoundOneButtonPatch
{
    public static int CurrentRound = 0;

    [HarmonyPatch(typeof(PlayerControl), nameof(PlayerControl.ReportDeadBody))]
    [HarmonyPrefix]
    public static bool ReportDeadBody_Prefix(PlayerControl __instance, NetworkedPlayerInfo target)
    {
        if (!HostUtilityPlugin.Instance.DisableR1Meetings.Value) return true;
        if (CurrentRound == 1 && !__instance.AmOwner && AmongUsClient.Instance.AmHost && target == null)
        {
            HudManager.Instance.Notifier.AddDisconnectMessage($"Player {target.PlayerName} has tried pressing the meeting button round 1!");
            return false;
        }
        return true;
    }

    [HarmonyPatch(typeof(PlayerControl), nameof(PlayerControl.OnGameStart))]
    [HarmonyPostfix]
    public static void OnGameStart(PlayerControl __instance)
    {
        if (__instance.AmOwner) CurrentRound = 1;
    }
    
    [HarmonyPatch(typeof(PlayerControl), nameof(PlayerControl.ResetForMeeting))]
    [HarmonyPostfix]
    public static void OnMeeting(PlayerControl __instance)
    {
        if (__instance.AmOwner) CurrentRound++;
    }
}