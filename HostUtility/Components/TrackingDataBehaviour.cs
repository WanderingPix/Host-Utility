using System.Collections.Generic;
using HostUtility.Utilities;
using InnerNet;
using UnityEngine;

namespace HostUtility.Components;

public class TrackingDataBehaviour : MonoBehaviour
{
    public List<string> chatMessages = new();
    public Permissions permissions = Permissions.Player;
    public static TrackingDataBehaviour Local;
    public PlayerControl myPlayer;
    private static Dictionary<PlayerControl, TrackingDataBehaviour> Data = new();

    public static TrackingDataBehaviour Get(PlayerControl player)
    {
        if (player.AmOwner) return Local;
        return Data.GetValueOrDefault(player);
    }
    private void Start()
    {
        Data.Add(myPlayer, this);
        if (myPlayer.AmOwner)
        {
            if (AmongUsClient.Instance.AmHost) permissions = Permissions.Host;
            Local = this;
            return;
        }

        if (AmongUsClient.Instance.AmHost)
        {
            Networking.RpcPromotePlayer(myPlayer, 
                PlayerPermissionsManager.GetPermissions(myPlayer));
        }
    }
    public float timeSinceLastSetColor = 1;
    public float timeSinceLastUpdateSystem = 1;
    public float timeSinceLastMurder = 1;

    private void Update()
    {
        timeSinceLastUpdateSystem += Time.deltaTime;
        timeSinceLastSetColor += Time.deltaTime;
        timeSinceLastMurder += Time.deltaTime;
    }

    public void SetPermissions(Permissions value, bool shouldNotify = true)
    {
        permissions = value;
        if (Local && Local.permissions == Permissions.Host) PlayerPermissionsManager.AddPlayer(myPlayer, permissions);
        if (myPlayer.AmOwner)
        {
            if (shouldNotify) HudManager.Instance.Notifier.AddMessage("You're been promoted to " + value);
            bool showSettings = !myPlayer.CanModifySettings();
            var startManager = GameStartManager.Instance;
            string gameName = GameCode.IntToGameName(AmongUsClient.Instance.GameId);
            if (showSettings)
            {
                startManager.HostPrivacyButtons.gameObject.SetActive(false);
                startManager.ClientPrivacyValue.gameObject.SetActive(true);
                startManager.StartButton.gameObject.SetActive(false);
                startManager.StartButtonClient.gameObject.SetActive(true);
                startManager.GameStartTextParent.SetActive(false);
                startManager.HostInfoPanelButtons.gameObject.SetActive(false);
                startManager.ClientInfoPanelButtons.gameObject.SetActive(true);
            }
            else
            {
                if (gameName != null)
                {
                    startManager.HostPrivacyButtons.gameObject.SetActive(true);
                    startManager.ClientPrivacyValue.gameObject.SetActive(false);
                }
                startManager.HostInfoPanelButtons.gameObject.SetActive(true);
                startManager.ClientInfoPanelButtons.gameObject.SetActive(false);
                startManager.StartButton.gameObject.SetActive(true);
                startManager.StartButtonClient.gameObject.SetActive(false);
            }
        }
    }
}