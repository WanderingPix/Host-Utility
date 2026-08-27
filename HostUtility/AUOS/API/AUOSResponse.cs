using System;

namespace HostUtility.AUOS.API;

// ReSharper disable InconsistentNaming
[Serializable]
public class AUOSResponse
{
    public AUOSEntry[] entries { get; set; }
    public int totalCount => entries.Length;
}