using System.Collections.Generic;
using UnityEngine;

namespace HostUtility;

public enum Permissions
{
    Player = 0,
    Moderator = 1,
    CoHost = 2,
    Host = 3
}
public static class PermissionVisuals
{
    public static Dictionary<Permissions, Color> PermissionLevelColors = new()
    {
        { Permissions.Host, Color.yellow },
        { Permissions.Moderator, Color.cyan },
        { Permissions.CoHost, Color.red },
        { Permissions.Player, Color.white },
    };
}