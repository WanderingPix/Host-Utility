using System;
using System.Collections.Generic;
using Reactor.Utilities.Attributes;
using UnityEngine;

namespace HostUtility.Components;

[RegisterInIl2Cpp]
public class TrackingDataBehaviour : MonoBehaviour
{
    public List<string> chatMessages = new();
    public static TrackingDataBehaviour Local;
    public PlayerControl myPlayer;

    private void Start()
    {
        if (myPlayer.AmOwner) Local = this;
    }
}