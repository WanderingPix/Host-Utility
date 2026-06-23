using System.Drawing;
using HarmonyLib;
using Il2CppSystem;
using UnityEngine;
using Color = System.Drawing.Color;

namespace HostUtility.Patches;

[HarmonyPatch(typeof(HudManager))]
public class HudManagerPatches
{
    [HarmonyPatch(nameof(HudManager.Start))]
    [HarmonyPostfix]
    public static void HudManager_Start_Postfix(HudManager __instance)
    {
        __instance.Chat.freeChatField.OnChangedEvent += new System.Action(() =>
        {
            __instance.Chat.freeChatField.textArea.outputText.color =
                __instance.Chat.freeChatField.Text.StartsWith("/") ? new Color32(50, 100, 150, 255) : new Color32(0, 0, 0, 255);
        });
        __instance.Chat.AddChatWarning(
            "<size=130%>Thanks for downloading Host Utility!</size>\nList of all available commands:");
        string allCommands = string.Empty;
        foreach (var command in ChatCommandsManager.Commands)
        {
            allCommands += $"<b>/{command.Command}</b> : {command.Description}\n<size=70%>";
            foreach (var arg in command.Arguments)
            {
                allCommands += $"- {arg.Name} ({arg.Type.ToString()})\n";
            }
            allCommands += "</size>\n";
        }
        __instance.Chat.AddChatWarning("<color=black>" + allCommands + "</color>");
        __instance.Chat.AddChatWarning(
            "<size=50%> Edater/PDF Filtering is powered by https://au.tntaddict.net/aufiles which may not be fully accurate!");
    }
}