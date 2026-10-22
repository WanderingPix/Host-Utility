using System.Linq;
using HarmonyLib;
using InnerNet;
using TMPro;
using UnityEngine;

namespace HostUtility.Patches;

[HarmonyPatch]
public class PlayerVoteAreaPatches
{
    [HarmonyPatch(typeof(PlayerVoteArea), nameof(PlayerVoteArea.SetPlayerId))]
    [HarmonyPostfix]
    public static void PlayerVoteArea_SetPlayerId_Postfix(PlayerVoteArea __instance, ref PlayerId playerId)
    {
        var pid = playerId;
        var owner = PlayerControl.AllPlayerControls.ToArray().First(x => x.Data.PlayerId == pid.Value);

        var grid = new GameObject("Grid").AddComponent<GridArrange>();
        grid.Alignment = GridArrange.StartAlign.Right;
        grid.CellSize = new Vector2(0.3f, -0.3f);
        grid.MaxColumns = 3;
        grid.transform.SetParent(__instance.transform, true);
        grid.transform.localPosition = __instance.LevelNumberText.transform.parent.localPosition;
        __instance.LevelNumberText.transform.parent.SetParent(grid.transform);
        var idIcon = Object.Instantiate(__instance.LevelNumberText.transform.parent, grid.transform).gameObject;
        idIcon.transform.GetChild(1).GetComponent<TextMeshPro>().text = owner.Data.PlayerId.ToString();
        var headerText = idIcon.transform.GetChild(0);
        Object.Destroy(headerText.GetComponent<TextTranslatorTMP>());
        headerText.GetComponent<TextMeshPro>().text = "ID";
        idIcon.GetComponent<SpriteRenderer>().color = Color.magenta;
        var hostIcon = Object.Instantiate(__instance.LevelNumberText.transform.parent, grid.transform).gameObject;
        var text = hostIcon.transform.GetChild(1);
        text.GetComponent<TextMeshPro>().text = "★";
        text.transform.localPosition = Vector3.zero;
        var headerText2 = hostIcon.transform.GetChild(0);
        Object.Destroy(headerText2.GetComponent<TextTranslatorTMP>());
        headerText2.GetComponent<TextMeshPro>().text = "";
        hostIcon.GetComponent<SpriteRenderer>().color = new Color(0.5f, 0, 1, 1);
        var platformIcon = Object.Instantiate(__instance.LevelNumberText.transform.parent, grid.transform).gameObject;
        platformIcon.transform.GetChild(0).gameObject.SetActive(false);
        platformIcon.transform.GetChild(1).gameObject.SetActive(false);
        platformIcon.GetComponent<SpriteRenderer>().color = new Color(0.5f, 0.5f, 1, 1);
        var identifier = Object.Instantiate(FriendsListManager.Instance.UiPrefab.LobbyPlayerBar.PlatformIdentifier, platformIcon.transform);
        identifier.transform.localPosition = new  Vector3(0.45f, 0, -10);
        identifier.SetInfo(AmongUsClient.Instance.GetClientFromCharacter(owner));
        identifier.iconRenderer.color = Color.white;
        owner.StartCoroutine(Effects.ActionAfterDelay(0.05f, new System.Action(() =>
            identifier.gameObject.transform.localScale = new Vector3(1.5f, 1.5f, 1))));
        
        var shader = Shader.Find("Sprites/Default");
        foreach (var rend in identifier.GetComponentsInChildren<SpriteRenderer>())
        {
            rend.material = new Material(shader);
        }
        identifier.transform.localScale = hostIcon.transform.localScale;
        
        idIcon.SetActive(HostUtilityPlugin.Instance.ShowPlayerIDs.Value);
        platformIcon.SetActive(HostUtilityPlugin.Instance.ShowPlayerPlatforms.Value); 
        hostIcon.SetActive(AmongUsClient.Instance.GetHost().Character == owner);

        __instance.Flag.transform.localPosition = new Vector3(1.313f, -0.186f, -3);
    }
}