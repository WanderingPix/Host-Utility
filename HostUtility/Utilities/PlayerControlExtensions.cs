using HostUtility.Components;

namespace HostUtility.Utilities;

public static class PlayerControlExtensions
{
    public static bool CanModifySettings(this PlayerControl player)
    {
        var data = TrackingDataBehaviour.Get(player);
        if (data == null) return false;
        return (int)data.permissions >= (uint)Permissions.CoHost;
    }
    
    public static bool CanStartGame(this PlayerControl player)
    {
        return CanModifySettings(player);
    }
    
    public static bool IsModerator(this PlayerControl player)
    {
        var data = TrackingDataBehaviour.Get(player);
        if (data == null) return false;
        return (int)data.permissions >= (uint)Permissions.Moderator;
    }
}