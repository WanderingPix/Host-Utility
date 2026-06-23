using AmongUs.Data;
using HarmonyLib;
using Reactor.Utilities;

namespace HostUtility.Patches;

[HarmonyPatch(typeof(GameStartManager))]
public static class GameStartManagerPatches
{
    [HarmonyPatch(nameof(GameStartManager.Start))]
    [HarmonyPrefix]
    public static void GameStartManager_Start_Prefix(GameStartManager __instance)
    {
        __instance.MinPlayers = 1;
    }

    [HarmonyPatch(nameof(GameStartManager.ReallyBegin))]
    [HarmonyPrefix]
    public static bool GameStartManager_ReallyBegin_Prefix(GameStartManager __instance, ref bool neverShow)
    {
        __instance.startState = GameStartManager.StartingStates.Countdown;
        __instance.GameSizePopup.SetActive(false);
        if (neverShow)
            DataManager.Player.Onboarding.AlwaysShowMinPlayerWarning = false;
        DataManager.Player.Onboarding.ViewedMinPlayerWarning = true;
        DataManager.Player.Save();
        __instance.StartButton.gameObject.SetActive(false);
        __instance.StartButtonClient.gameObject.SetActive(false);
        __instance.GameStartTextParent.SetActive(false);
        __instance.countDownTimer = PluginSingleton<HostUtilityPlugin>.Instance.GameStartCountdownTime.Value;
        __instance.startState = GameStartManager.StartingStates.Countdown;
        AmongUsClient.Instance.KickNotJoinedPlayers();
        return false;
    }
}