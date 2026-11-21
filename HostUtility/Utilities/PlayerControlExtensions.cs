using Hazel;
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
    
    public static bool CanStartGame(this PlayerControl player, out bool sendRequest)
    {
        sendRequest = false;
        var data = TrackingDataBehaviour.Get(player);
        if (data == null) return false;
        sendRequest = data.permissions != Permissions.Host;
        return CanModifySettings(player);
    }
    
    public static bool IsModerator(this PlayerControl player)
    {
        var data = TrackingDataBehaviour.Get(player);
        if (data == null) return false;
        return (int)data.permissions >= (uint)Permissions.Moderator;
    }
    
    public static void SendChatTo(this PlayerControl target, string text, PlayerControl asPlayer = null)
    {
        asPlayer ??= PlayerControl.LocalPlayer;

        var writer = AmongUsClient.Instance.StartRpcImmediately(
            asPlayer.NetId,
            (byte)RpcCalls.SendChat,
            SendOption.Reliable,
            target.Data.ClientId
        );
        writer.Write(text);
        AmongUsClient.Instance.FinishRpcImmediately(writer);
    }
}