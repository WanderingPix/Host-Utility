using System;
using AmongUs.GameOptions;
using HarmonyLib;
using Hazel;
using HostUtility.Components;
using HostUtility.Utilities;
using InnerNet;
using UnityEngine;
using Object = UnityEngine.Object;

namespace HostUtility.Patches;

[HarmonyPatch]
public class GameOptionsPatches
{
    [HarmonyPatch(typeof(GameOptionsMenu), nameof(GameOptionsMenu.ValueChanged))]
    [HarmonyPatch(typeof(RolesSettingsMenu), nameof(RolesSettingsMenu.ValueChanged))]
    [HarmonyPrefix]
    public static void SendToHost()
    {
        if (PlayerControl.LocalPlayer.CanModifySettings())
        {
            var w = AmongUsClient.Instance.StartRpcImmediately(
                PlayerControl.LocalPlayer.NetId, 101, SendOption.Reliable, AmongUsClient.Instance.HostId);
            GameManager.Instance.LogicOptions.Serialize(w);
            AmongUsClient.Instance.FinishRpcImmediately(w);
        }
    }

    [HarmonyPatch(typeof(GameStartManager), nameof(GameStartManager.ClickEdit))]
    [HarmonyPrefix]
    public static bool ClickEdit(GameStartManager __instance)
    {
        if (__instance.RulesEditPanel != null)
            return false;
        PlayerControl localPlayer = PlayerControl.LocalPlayer;
        if (!PlayerControl.LocalPlayer.CanModifySettings())
            return false;
        localPlayer.NetTransform.Halt();
        __instance.RulesEditPanel = Object.Instantiate<GameObject>(__instance.PlayerOptionsMenu);
        __instance.RulesEditPanel.transform.SetParent(Camera.main.transform, false);
        __instance.RulesEditPanel.transform.localPosition = __instance.GameOptionsPosition;
        DestroyableSingleton<TransitionFade>.Instance.DoTransitionFade((GameObject) null, __instance.RulesEditPanel.gameObject, (Action) null);
        __instance.RulesViewPanel.SetActive(false);
        if ((bool) (Object) PlayerCustomizationMenu.Instance)
            PlayerCustomizationMenu.Instance.Close(false);
        __instance.SelectViewButton(false);
        __instance.LobbyInfoPane.DeactivatePane();
        return false;
    }
    
    [HarmonyPatch(typeof(InnerNetClient), nameof(InnerNetClient.AmHost), MethodType.Getter)]
    [HarmonyPostfix]
    public static void AmHost(ref bool __result)
    {
        if (__result) return;
        if (PlayerControl.LocalPlayer == null) return;
        if (!LobbyBehaviour.Instance) return;
        if (PlayerControl.LocalPlayer.CanModifySettings()) __result = true;
    }
}