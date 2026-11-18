using Hazel;

namespace HostUtility;

public static class Networking
{
    public static void PromotePlayer(PlayerControl player, Permissions permissionsLevel)
    {
        var w = AmongUsClient.Instance.StartRpcImmediately(
            PlayerControl.LocalPlayer.NetId, 104, SendOption.Reliable, AmongUsClient.Instance.HostId);
        w.Write(player.Data.PlayerId);
        w.Write((uint)permissionsLevel);
        AmongUsClient.Instance.FinishRpcImmediately(w);
    }
}