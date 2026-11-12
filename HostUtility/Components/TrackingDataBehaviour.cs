using System.Collections.Generic;
using UnityEngine;

namespace HostUtility.Components;

public class TrackingDataBehaviour : MonoBehaviour
{
    public List<string> chatMessages = new();
    public int permissions = 0;
    public static TrackingDataBehaviour Local;
    public PlayerControl myPlayer;
    private static Dictionary<PlayerControl, TrackingDataBehaviour> Data = new();

    public static TrackingDataBehaviour Get(PlayerControl player)
    {
        return Data[player];
    }
    private void Start()
    {
        if (myPlayer.AmOwner)
        {
            Local = this;
            if (AmongUsClient.Instance.AmHost) permissions = Permissions.HostPermissions;
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

    public void SetPermissions(int value)
    {
        permissions = value;
        if (permissions >= Permissions.CoHostPermissions)
        {
            var infoPane = GameStartManager.Instance?.LobbyInfoPane;
            infoPane.HostViewButton.gameObject.SetActive(true);
            GameStartManager.Instance?.ClientInfoPanelButtons.gameObject.SetActive(true);
            GameStartManager.Instance?.HostInfoPanelButtons.gameObject.SetActive(true);
        }
    }
}