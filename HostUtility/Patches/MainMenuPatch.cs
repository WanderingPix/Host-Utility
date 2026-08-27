using HarmonyLib;
using HostUtility.AUFiles;
using HostUtility.AUOS;
using HostUtility.BanListAPI;
using HostUtility.BanListAPI.Providers.AUFiles;
using HostUtility.PlayerReporting;
using TMPro;
using UnityEngine;

namespace HostUtility.Patches;

[HarmonyPatch(typeof(MainMenuManager))]
public class MainMenuPatch
{
    [HarmonyPatch(nameof(MainMenuManager.Start))]
    [HarmonyPostfix]
    public static void StartPostfix(MainMenuManager __instance)
    {
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
    }
}