using AmongUs.GameOptions;
using HarmonyLib;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace HostUtility.Patches;

[HarmonyPatch(typeof(CreateGameOptions))]
public class CreateGameOptionsPatches
{
    [HarmonyPatch(nameof(CreateGameOptions.Start))]
    [HarmonyPostfix]
    public static void CreateGameOptions_Start_Postfix(CreateGameOptions __instance)
    {
        __instance.crossPlayButtons[0].transform.parent.gameObject.SetActive(true);
        var content = __instance.transform.GetChild(0).GetChild(4);
        content.transform.localScale = Vector3.one * 0.9f;
        var createGame = content.GetChild(2);
        createGame.transform.localPosition -= new Vector3(0, 0.4f, 0);

        var hostAuthorityOption =
            Object.Instantiate(__instance.AprilFoolsToggle, __instance.AprilFoolsToggle.transform.parent, true);
        hostAuthorityOption.SetActive(true);
        hostAuthorityOption.transform.localPosition -= new Vector3(0, 0.5f, 0);
        hostAuthorityOption.transform.GetChild(0).GetChild(0).GetComponent<TextMeshPro>().text = "Host Authority";
        var onButton = hostAuthorityOption.transform.GetChild(1).GetComponent<PassiveButton>();
        onButton.OnClick =
            new Button.ButtonClickedEvent();
        var offButton = hostAuthorityOption.transform.GetChild(2).GetComponent<PassiveButton>();
        offButton.OnClick =
            new Button.ButtonClickedEvent();
        onButton.OnClick.AddListener(new System.Action(() =>
        {
            HostUtilityPlugin.Instance.UseHostAuthority.Value = true;
            onButton.SelectButton(true);
            offButton.SelectButton(false);
        }));
        offButton.OnClick.AddListener(new System.Action(() =>
        {
            HostUtilityPlugin.Instance.UseHostAuthority.Value = false;
            onButton.SelectButton(false);
            offButton.SelectButton(true);
        }));
        onButton.OnMouseOver = offButton.OnMouseOver = onButton.OnMouseOut = offButton.OnMouseOut = new();
        onButton.SelectButton(HostUtilityPlugin.Instance.UseHostAuthority.Value);
        offButton.SelectButton(!HostUtilityPlugin.Instance.UseHostAuthority.Value);
        __instance.AprilFoolsToggle.transform.position -= new Vector3(0, 0.75f, 0);
    }
    
    private static TextMeshPro hostAuthorityText;
    [HarmonyPatch(typeof(ConfirmCreatePopUp), nameof(ConfirmCreatePopUp.SetupInfo))]
    [HarmonyPostfix]
    public static void ConfirmCreatePopUp_SetupInfo_Postfix(ConfirmCreatePopUp __instance)
    {
        if (!hostAuthorityText)
        {
            var tooltipText = __instance.transform.GetChild(1).GetChild(2);
            hostAuthorityText = Object.Instantiate(tooltipText, tooltipText.parent).GetComponent<TextMeshPro>();
            Object.Destroy(hostAuthorityText.GetComponent<TextTranslatorTMP>());
        }
        hostAuthorityText.gameObject.SetActive(HostUtilityPlugin.Instance.UseHostAuthority.Value);
        hostAuthorityText.text = "You are using Host Authority Mode, vanilla players may not be able to find your lobby.";
        hostAuthorityText.transform.localPosition = new Vector3(0f, -2.3f, -0f);
    }
}