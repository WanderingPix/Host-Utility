using System;
using System.Collections.Generic;
using System.Linq;
using AmongUs.GameOptions;
using HarmonyLib;
using Hazel;
using HostUtility.BanListAPI;
using HostUtility.Components;
using HostUtility.GameSettings;
using HostUtility.Utilities;
using UnityEngine;

namespace HostUtility.Patches;

[HarmonyPatch]
public class PlayerControlPatches
{
    private const float StartupDelay = 1f;

    [HarmonyPatch(typeof(PlayerControl), nameof(PlayerControl.Start))]
    [HarmonyPostfix]
    public static void PlayerControl_Start_Postfix(PlayerControl __instance)
    {
        if (__instance == null) return;

        var nameContainer = __instance.cosmetics?.nameTextContainer;
        var nameText = __instance.cosmetics?.nameText;
        if (nameContainer != null && nameText != null)
        {
            var iconContainer = new GameObject("IconContainer");
            iconContainer.transform.SetParent(nameContainer.transform);
            iconContainer.transform.localPosition = nameText.transform.localPosition + new Vector3(0, 0.35f, 0);

            __instance.StartCoroutine(Effects.ActionAfterDelay(StartupDelay, new Action(() =>
            {
                if (iconContainer == null || __instance == null) return;
                iconContainer.AddComponent<PlayerIconsBehaviour>().Initialize(__instance);
            })));
        }

        __instance.gameObject.AddComponent<TrackingDataBehaviour>().myPlayer = __instance;

        __instance.StartCoroutine(Effects.ActionAfterDelay(StartupDelay, new Action(() =>
        {
            if (__instance == null) return;

            if (__instance == PlayerControl.LocalPlayer)
            {
                __instance.RpcHandshake();
                return;
            }

            RunHostChecks(__instance);
        })));
    }

    private static void RunHostChecks(PlayerControl player)
    {
        var client = AmongUsClient.Instance;
        if (client == null || !client.AmHost) return;
        if (player == null || player.Data == null) return;

        var tab = SettingTabSingleton<ModerationTab>.Instance;
        if (tab == null) return;

        var data = player.Data;
        var clientId = data.ClientId;
        var name = data.PlayerName ?? string.Empty;
        var banNames = tab.BanInappropriateNames.Value;

        if (RulesetManager.HasRuleset)
        {
            player.SendChatTo(RulesetManager.CurrentRuleSet);
        }
        if (banNames && BanWords.ContainsSwear(name))
        {
            client.KickWithReason(clientId, "Inappropriate Username", true);
            return;
        }

        if (banNames && BotNames.Names.Contains(name))
        {
            client.KickWithReason(clientId, "Bot Player", true);
            return;
        }

        if (IsBlank(name)) player.RpcSetName("Unknown Player");

        if (data.PlayerLevel < tab.PlayerLevelCap.Value)
        {
            client.KickWithReason(clientId, "Low level", false);
            return;
        }

        var clientData = client.GetClientFromCharacter(player);
        if (clientData != null && BanListManager.IsTargetOnBanList(clientData, out var banReason, out var banListName))
        {
            client.KickWithReason(clientId, banReason, banListName, false);
            return;
        }

        var puid = client.GetClient(clientId)?.ProductUserId;
        if (!string.IsNullOrEmpty(puid)
            && FriendsListManager.Instance != null
            && FriendsListManager.Instance.IsPlayerBlocked(puid))
        {
            client.KickWithReason(clientId, "Blocked Player", false);
        }
    }

    public static readonly HashSet<char> InvisibleCharacters = new()
    {
        '\u0000', '\u0001', '\u0002', '\u0003', '\u0004', '\u0005', '\u0006', '\u0007', '\u0008',
        '\u000B', '\u000C', '\u000E', '\u000F', '\u0010', '\u0011', '\u0012', '\u0013', '\u0014',
        '\u0015', '\u0016', '\u0017', '\u0018', '\u0019', '\u001A', '\u001B', '\u001C', '\u001D',
        '\u001E', '\u001F', '\u007F',
        '\u00A0',
        '\u00AD',
        '\u034F',
        '\u061C',
        '\u115F', '\u1160',
        '\u17B4', '\u17B5',
        '\u180B', '\u180C', '\u180D', '\u180E',
        '\u200B', '\u200C', '\u200D', '\u200E', '\u200F',
        '\u202A', '\u202B', '\u202C', '\u202D', '\u202E', '\u202F',
        '\u2060', '\u2061', '\u2062', '\u2063', '\u2064',
        '\u2066', '\u2067', '\u2068', '\u2069',
        '\u206A', '\u206B', '\u206C', '\u206D', '\u206E', '\u206F',
        '\u3164',
        '\uFEFF',
        '\uFFA0',
        '\uFFF9', '\uFFFA', '\uFFFB',
    };

    private static bool IsBlank(string s)
    {
        if (string.IsNullOrEmpty(s)) return true;
        foreach (var c in s)
        {
            if (!InvisibleCharacters.Contains(c) && !char.IsWhiteSpace(c)) return false;
        }

        return true;
    }

    private static PlayerControl FindPlayer(byte playerId)
    {
        foreach (var p in PlayerControl.AllPlayerControls)
        {
            if (p != null && p.Data != null && p.Data.PlayerId == playerId) return p;
        }

        return null;
    }

    private static void ApplyPermissions(PlayerControl sender, MessageReader reader)
    {
        var client = AmongUsClient.Instance;
        if (client == null || sender.OwnerId != client.HostId) return;

        var target = FindPlayer(reader.ReadByte());
        var permissions = (Permissions)reader.ReadInt32();
        if (target == null) return;

        TrackingDataBehaviour.Get(target)?.SetPermissions(permissions);
    }

    [HarmonyPatch(typeof(PlayerControl), nameof(PlayerControl.HandleRpc))]
    [HarmonyPostfix]
    public static void PlayerControl_HandleRpc_Postfix(PlayerControl __instance, byte callId, MessageReader reader)
    {
        if (__instance == null || reader == null) return;

        try
        {
            switch (callId)
            {
                case 100:
                    if (!HandshakeManager.ModdedPlayers.Contains(__instance))
                        HandshakeManager.ModdedPlayers.Add(__instance);
                    PlayerIconsBehaviour.AllIcons.Find(x => x != null && x.owner == __instance)?.ModdedIcon?.SetActive(true);
                    break;

                case 101:
                {
                    var client = AmongUsClient.Instance;
                    if (client == null || client.HostId != client.ClientId) break;
                    if (!__instance.CanModifySettings()) break;

                    var logic = GameManager.Instance?.LogicOptions;
                    var optionsManager = GameOptionsManager.Instance;
                    if (logic == null || optionsManager == null) break;

                    HostUtilityPlugin.Instance.Log.LogInfo($"[101] from {__instance.PlayerId}");
                    logic.Deserialize(reader);
                    optionsManager.GameHostOptions = optionsManager.CurrentGameOptions;
                    logic.SyncOptions();
                    HostUtilityPlugin.Instance.Log.LogInfo("DING DING DING!!! SYNCED!!!");
                    break;
                }

                case 102:
                case 104:
                    ApplyPermissions(__instance, reader);
                    break;

                case 103:
                {
                    if (AmongUsClient.Instance == null || !AmongUsClient.Instance.AmHost) break;
                    var player = FindPlayer(reader.ReadByte());
                    if (player != null && player.CanStartGame()) GameStartManager.Instance?.BeginGame();
                    break;
                }
            }
        }
        catch (Exception e)
        {
            HostUtilityPlugin.Instance.Log.LogError($"HandleRpc {callId} from {__instance.PlayerId} failed: {e}");
        }
    }
}   