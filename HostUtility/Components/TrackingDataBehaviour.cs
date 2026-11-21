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
        if (myPlayer.AmOwner)
        {
            Local = this;
            if (AmongUsClient.Instance.AmHost) permissions = Permissions.Host;
        }
        Data.Add(myPlayer, this);
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

    public void SetPermissions(Permissions value)
    {
        permissions = value;
        if (myPlayer.AmOwner)
        {
            HudManager.Instance.ShowPopUp("You're been promoted to " + value);
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