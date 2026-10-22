using AmongUs.Data;
using HarmonyLib;
using HostUtility.Components;

namespace HostUtility.Patches;

[HarmonyPatch(typeof(GameStartManager))]
public static class GameStartManagerPatches
{
    public static string CancelText = "Press again to cancel.";
    public static string StartText = "Start";
    [HarmonyPatch(nameof(GameStartManager.Start))]
    [HarmonyPrefix]
    public static void GameStartManager_Start_Prefix(GameStartManager __instance)
    {
        __instance.MinPlayers = 1;
        StartText = __instance.StartButton.buttonText.text;
        __instance.StartButton.OnClick.AddListener(new System.Action(() =>
        {
            if (__instance.StartButton.buttonText.text == CancelText)
            {
                __instance.ResetStartState();
                __instance.StartButton.buttonText.text = StartText;
            }
            else __instance.StartButton.buttonText.text = CancelText;
        }));
    }

    [HarmonyPatch(nameof(GameStartManager.ReallyBegin))]
    [HarmonyPrefix]
    public static bool GameStartManager_ReallyBegin_Prefix(GameStartManager __instance, ref bool neverShow)
    {
        __instance.StartButton.gameObject.SetActive(true);
        __instance.startState = GameStartManager.StartingStates.Countdown;
        __instance.GameSizePopup.SetActive(false);
        if (neverShow)
            DataManager.Player.Onboarding.AlwaysShowMinPlayerWarning = false;
        DataManager.Player.Onboarding.ViewedMinPlayerWarning = true;
        DataManager.Player.Save();
        __instance.StartButtonClient.gameObject.SetActive(false);
        __instance.GameStartTextParent.SetActive(false);
        __instance.countDownTimer = HostUtilityPlugin.Instance.GameStartCountdownTime.Value;
        __instance.startState = GameStartManager.StartingStates.Countdown;
        AmongUsClient.Instance.KickNotJoinedPlayers();
        return false;
    }

    [HarmonyPatch(nameof(GameStartManager.FinallyBegin))]
    [HarmonyPrefix]
    public static void OnGameStart()
    {
        foreach (var icon in PlayerIconsBehaviour.AllIcons)
        {
            icon.gameObject.SetActive(false);
        }
    }
}