using HarmonyLib;
using Reactor.Utilities;

namespace HostUtility.Patches;

[HarmonyPatch]
public class PlayerControlPatches
{
    [HarmonyPatch(typeof(PlayerControl), nameof(PlayerControl.Start))]
    [HarmonyPostfix]
    public static void PlayerControl_Start_Postfix(PlayerControl __instance)
    {
        var plugin = PluginSingleton<HostUtilityPlugin>.Instance;
        __instance.StartCoroutine(Effects.ActionAfterDelay(0.2f, new System.Action(() =>
        {
            if (!AmongUsClient.Instance.AmHost) return;
            if (__instance == PlayerControl.LocalPlayer) return;

            if (BanWords.ContainsSwear(__instance.Data.PlayerName) && plugin.BanInappropriateNames.Value)
                AmongUsClient.Instance.KickWithReason(__instance.Data.ClientId, "Inappropriate username", true);
        })));
        __instance.StartCoroutine(Effects.ActionAfterDelay(1f, new System.Action(() =>
        {
            if (__instance.Data.PlayerLevel < plugin.MinLevel.Value)
                AmongUsClient.Instance.KickWithReason(__instance.Data.ClientId, "Low level", false);
        })));
    }
}