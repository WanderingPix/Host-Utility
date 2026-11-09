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