using System.Drawing;
using System.Reflection;
using HarmonyLib;
using HostUtility.Components;
using Il2CppSystem;
using UnityEngine;
using UnityEngine.UI;
using Action = System.Action;
using Color = System.Drawing.Color;

namespace HostUtility.Patches;

[HarmonyPatch(typeof(HudManager))]
public class HudManagerPatches
{
    [HarmonyPatch(nameof(HudManager.Start))]
    [HarmonyPostfix]
    public static void HudManager_Start_Postfix(HudManager __instance)
    {
        var btn = UnityEngine.Object.Instantiate(__instance.GameMenu.CensorChatButton, __instance.GameMenu.transform);
        btn.Text.text = "CMD";
        btn.enabled = false;
        var passiveButton = btn.GetComponent<PassiveButton>();
        passiveButton.OnClick = new Button.ButtonClickedEvent();
        passiveButton.OnClick.AddListener(new Action(() =>
        {
            if (Minigame.Instance) return;
            __instance.GameMenu.Close();
            __instance.GameMenu.gameObject.SetActive(false);
            CommandSelector.CreateAndShow();
        }));
        var pos = btn.gameObject.AddComponent<AspectPosition>();
        pos.Alignment = AspectPosition.EdgeAlignments.LeftBottom;
        pos.DistanceFromEdge = new Vector3(1.25f, 0.325f, 0);
        pos.AdjustPosition();
    }
}