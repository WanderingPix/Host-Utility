using System;
using System.Collections;
using BepInEx.Unity.IL2CPP.Utils.Collections;
using HarmonyLib;
using HostUtility.AUFiles;
using HostUtility.AUOS;
using HostUtility.BanListAPI;
using HostUtility.BanListAPI.Providers.AUFiles;
using HostUtility.PlayerReporting;
using TMPro;
using UnityEngine;
using Object = UnityEngine.Object;

namespace HostUtility.Patches;

[HarmonyPatch(typeof(MainMenuManager))]
public class MainMenuPatch
{
    private static bool HasSeenPreReleasePopUp = false;
    [HarmonyPatch(nameof(MainMenuManager.Start))]
    [HarmonyPostfix]
    public static void StartPostfix(MainMenuManager __instance)
    {
        BanListManager.Initialize();
        
        var original = Object.FindObjectOfType<VersionShower>();
        if (!original)
            return;

        var originalText = original.GetComponentInChildren<TextMeshPro>();
        
        var gameObject = new GameObject("ReactorVersion");

        var text = gameObject.AddComponent<TextMeshPro>();
        text.font = originalText.font;
        text.fontMaterial = originalText.fontMaterial;
        text.UpdateFontAsset();
        text.overflowMode = TextOverflowModes.Overflow;
        text.fontSize = 2;
        text.outlineWidth = 0.1f;
        text.enableWordWrapping = false;
        text.alignment = TextAlignmentOptions.BottomLeft;
        var pos = text.gameObject.AddComponent<AspectPosition>();
        pos.Alignment = AspectPosition.EdgeAlignments.LeftBottom;
        pos.DistanceFromEdge = new Vector3(14.125f, 3.15f, 0f);
        pos.AdjustPosition();

        text.text = BanListManager.GetMainMenuInfoText();

        if (HostUtilityPlugin.IsPreRelease && !HasSeenPreReleasePopUp)
        {
            __instance.StartCoroutine(WaitForStartUp(__instance, () =>
            {
                HasSeenPreReleasePopUp = true;
                DestroyableSingleton<DisconnectPopup>.Instance.ShowCustom(
                    $"<size=150%>Welcome to Host Utility {HostUtilityPlugin.Version} Beta!</size>\nThis version is a <color=red>pre-release</color>, so expect bugs!\n<size=75%>if you find any, open a post in the RFS Discord Server.");
            }).WrapToIl2Cpp());
        }
    }

    private static IEnumerator WaitForStartUp(MainMenuManager __instance, Action callback)
    {
        while (!__instance.finishStartup)
        {
            yield return null;
        }
        callback.Invoke();
    }
}