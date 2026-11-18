using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using BepInEx.Unity.IL2CPP.Utils.Collections;
using HostUtility.Components;
using HostUtility.Components.UI;
using HostUtility.GameSettings.OptionTypes;
using Il2CppInterop.Runtime.InteropTypes.Arrays;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using Object = UnityEngine.Object;

namespace HostUtility.GameSettings;

public class PlayerManagementTab : GameSettingsTab
{
    public override string Name => "Player Management";
    public override List<OptionGroup> OptionGroups => [];
    private List<PlayerVoteArea> Buttons = new();
    public override void CreateSettings(GameOptionsMenu gameOptionsMenu, Transform parentTransform, out float y)
    {
        y = 0;
        var container = new GameObject("Container");
        container.transform.SetParent(parentTransform);
        container.transform.localPosition = new Vector3(-0.5f, 1.6f, -25);
        var grid = container.AddComponent<GridArrange>();
        grid.Alignment = GridArrange.StartAlign.Right;
        grid.MaxColumns = 2;
        grid.CellSize = new Vector2(3, 0.8f);

        var nameplatePrefab = HudManager.Instance.MeetingPrefab.PlayerButtonPrefab;
        foreach (var p in PlayerControl.AllPlayerControls)
        {
            var data = TrackingDataBehaviour.Get(p);
            var button = Object.Instantiate(nameplatePrefab, container.transform);
            Buttons.Add(button);
            button.Parent = HudManager.Instance.MeetingPrefab;
            button.SetEnabled();
            if (p.AmOwner) button.SetDisabled();
            button.SetDead(false);
            button.NameText.text = p.Data.PlayerName;
            button.SetPlayerId(p.Data.PlayerId);
            button.PlayerIcon.InitBody();
            button.SetMaskLayer(20);
            button.PlayerIcon.UpdateFromPlayerData(p.Data, PlayerOutfitType.Default, PlayerMaterial.MaskType.SimpleUI, false);
            button.PlayerIcon.transform.localPosition = new Vector3(-1, 0, -20);
            button.PlayerIcon.transform.localScale = new Vector3(0.3f, 0.3f, 1);
            HudManager.Instance.StartCoroutine(Effects.ActionAfterDelay(0.1f,
                new Action(() => button.transform.FindChild("Grid").gameObject.SetActive(false))));
            button.ColorBlindName.enabled = true;
            button.ColorBlindName.color = PermissionVisuals.PermissionLevelColors[data.permissions];
            button.ColorBlindName.text = data.permissions.ToString();
            var passiveButton = button.transform.FindChild("votePlayerBase").GetComponent<PassiveButton>();
            passiveButton.OnClick = new Button.ButtonClickedEvent();
            if (!p.AmOwner)
            {
                passiveButton.OnClick.AddListener(new Action(() =>
                {
                    button.JudgeOverruleButton.gameObject.SetActive(true);
                    button.Buttons.gameObject.SetActive(true);
                    foreach (var btn in Buttons)
                    {
                        if (btn != button) btn.Cancel();
                    }

                    for (int i = 0; i < button.Buttons.transform.childCount; i++)
                    {
                        var c = button.Buttons.transform.GetChild(i);
                        button.StartCoroutine(
                            Effects.All([Effects.ScaleIn(c, 0, 0.7f, 0.1f), Effects.Rotate2D(c, 45, 0, 0.15f)]));
                    }
                }));
            }
            try
            {
                button.SetCosmetics(p.Data);
            }
            catch
            {
                // ignored
            }

            var overrule = button.JudgeOverruleButton.TryCast<PassiveButton>();
            if (overrule == null) return;
            overrule.OnClick = new Button.ButtonClickedEvent();
            overrule.OnClick.AddListener(new Action(() =>
            {
                int nextLevel = (int)data.permissions;
                nextLevel++;
                if (nextLevel > (int)Permissions.CoHost) nextLevel = 0;
                data.SetPermissions((Permissions)nextLevel);
                Networking.PromotePlayer(p, (Permissions)nextLevel);
                button.ColorBlindName.text = data.permissions.ToString();
                button.ColorBlindName.enabled = true;
                button.ColorBlindName.color = PermissionVisuals.PermissionLevelColors[data.permissions];
                button.StartCoroutine(Effects.Bloop(0, button.ColorBlindName.transform));
                button.Cancel();
                SoundManager.Instance.PlaySound(HudManager.Instance.MeetingPrefab.VoteSound, false);
            }));
            var confirm = button.ConfirmButton.TryCast<PassiveButton>();
            if (confirm == null) return;
            confirm.transform.GetChild(0).GetComponent<SpriteRenderer>().color = Color.magenta;
            confirm.OnClick = new Button.ButtonClickedEvent();
            confirm.GetComponent<SpriteRenderer>().sprite = HostUtilityAssets.KickButtonSprite;
            confirm.OnClick.AddListener(new Action(() =>
            {
                AmongUsClient.Instance.KickPlayer(p.Data.ClientId, true);
                button.Cancel();
                button.SetDisabled();
            }));
        }
    }
}