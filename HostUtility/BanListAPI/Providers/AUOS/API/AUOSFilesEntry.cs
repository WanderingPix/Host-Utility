using System;

namespace HostUtility.AUOS.API;

// ReSharper disable UnusedMember.Global
// ReSharper disable InconsistentNaming
[Serializable]
public class AUOSEntry
{
    public string Puid { get; set; }
    public int Score { get; set; }
}