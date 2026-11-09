using System.Collections.Generic;
using UnityEngine;

namespace HostUtility.Components;

public class TrackingDataBehaviour : MonoBehaviour
{
    public List<string> chatMessages = new();
    public static TrackingDataBehaviour Local;
    public PlayerControl myPlayer;

    private void Start()
    {
        if (myPlayer.AmOwner) Local = this;
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
}