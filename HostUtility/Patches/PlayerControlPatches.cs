using System.Linq;
using AmongUs.InnerNet.GameDataMessages;
using HarmonyLib;
using Hazel;
using HostUtility.AUFiles;
using HostUtility.BanListAPI;
using HostUtility.BanListAPI.Providers.AUFiles;
using HostUtility.Components;
using Reactor.Utilities;

namespace HostUtility.Patches;

[HarmonyPatch]
public class PlayerControlPatches
{
    [HarmonyPatch(typeof(PlayerControl), nameof(PlayerControl.Start))]
    [HarmonyPostfix]
    public static void PlayerControl_Start_Postfix(PlayerControl __instance)
    {
        __instance.gameObject.AddComponent<TrackingDataBehaviour>().myPlayer = __instance;
        if (!AmongUsClient.Instance.AmHost) return;
        var plugin = PluginSingleton<HostUtilityPlugin>.Instance;
        if (__instance == PlayerControl.LocalPlayer) return;
        __instance.StartCoroutine(Effects.ActionAfterDelay(1f, new System.Action(() =>
        {
            if (__instance == PlayerControl.LocalPlayer) return;
            if (plugin.ShowPlayerPlatforms.Value)
            {
                var platformName = AmongUsClient.Instance.GetClientFromCharacter(__instance).PlatformData.PlatformName;
                if (platformName == "112") platformName = "Starlight Mobile";
                if (platformName == "TESTNAME") platformName = "Unknown";
                __instance.cosmetics.nameText.text += $" ({platformName})";
            }
            if (plugin.ShowPlayerIDs.Value) __instance.cosmetics.nameText.text += $" (ID: {__instance.PlayerId})";
            
            if (BanWords.ContainsSwear(__instance.Data.PlayerName) && plugin.BanInappropriateNames.Value) AmongUsClient.Instance.KickWithReason(__instance.Data.ClientId, "Inappropriate username", "",true);
            if (BotNames.Names.Contains(__instance.Data.PlayerName) && plugin.BanInappropriateNames.Value) AmongUsClient.Instance.KickWithReason(__instance.Data.ClientId, "Bot Player", "",true);
            if (__instance.Data.PlayerLevel < plugin.MinLevel.Value) AmongUsClient.Instance.KickWithReason(__instance.Data.ClientId, "Low level", "",false);
            if (BanListManager.IsTargetOnBanList(AmongUsClient.Instance.GetClientFromCharacter(__instance), out string banReason, out string banListName)) AmongUsClient.Instance.KickWithReason(__instance.Data.ClientId, banReason, banListName, false);
            if (FriendsListManager.Instance.IsPlayerBlocked(AmongUsClient.Instance.GetClient(__instance.Data.ClientId).ProductUserId)) AmongUsClient.Instance.KickWithReason(__instance.Data.ClientId, "Blocked player", "", false);
        })));
        __instance.SetName(__instance.Data.PlayerName + $"({AmongUsClient.Instance.GetClient(__instance.Data.ClientId).PlatformData.PlatformName})");
    }
}