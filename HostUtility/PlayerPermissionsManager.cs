using System.Collections.Generic;

namespace HostUtility;

public static class PlayerPermissionsManager
{
    private static Dictionary<string, Permissions> Permissions = new Dictionary<string, Permissions>();

    public static void AddPlayer(PlayerControl player, Permissions permission)
    {
        Permissions.Remove(player.Data.FriendCode);
        Permissions.Add(player.Data.FriendCode, permission);
    }

    public static Permissions GetPermissions(PlayerControl player)
    {
        return Permissions.GetValueOrDefault(player.Data.FriendCode, HostUtility.Permissions.Player);
    }
}