using System.Drawing;
using System.Linq;
using System.Reflection;
using HarmonyLib;
using HostUtility.Components;
using HostUtility.PlayerReporting;
using Il2CppSystem;
using InnerNet;
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
    [HarmonyPatch(typeof(ReportReasonScreen), nameof(ReportReasonScreen.Submit))]
    [HarmonyPrefix]
    public static void ReportReasonScreen_Submit_Prefix(ReportReasonScreen __instance)
    {
        var btn1 = __instance.Buttons[0];
        var btn2 = __instance.Buttons[1];
        var btn3 = __instance.Buttons[3];
        if (btn1.Target.color == btn1.OverColor ||
            btn2.Target.color == btn2.OverColor ||
            btn3.Target.color == btn3.OverColor)
        {
            ReportingManager.ReportPlayer(PlayerControl.AllPlayerControls.ToArray().First(x => x.Data.PlayerName == __instance.NameText.text));
        }
    }
}