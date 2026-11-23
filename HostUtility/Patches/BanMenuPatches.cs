using HarmonyLib;
using HostUtility;
using HostUtility.Utilities;
using InnerNet;

[HarmonyPatch]
public static class BanMenuPatches
{
    [HarmonyPatch(typeof(InnerNetClient), nameof(InnerNetClient.CanBan))]
    [HarmonyPatch(typeof(InnerNetClient), nameof(InnerNetClient.CanKick))]
    [HarmonyPostfix]
    public static void BanKick_Postfix(ref bool __result)
    {
        if (PlayerControl.LocalPlayer && PlayerControl.LocalPlayer.IsModerator())
            __result = true;
    }

    [HarmonyPatch(typeof(InnerNetClient), nameof(InnerNetClient.KickPlayer))]
    [HarmonyPrefix]
    public static bool KickPlayer_Prefix(int clientId, bool ban)
    {
        if (!PlayerControl.LocalPlayer) return true;
        if (!PlayerControl.LocalPlayer.CanKickOrBan(out bool sendRequest)) return false;
        if (!sendRequest) return true;

        Networking.CmdKickPlayer(clientId, ban);
        return false;
    }
}