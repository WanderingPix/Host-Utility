using HostUtility.Utilities;
using Hazel;
using HostUtility.Components;

namespace HostUtility;

public static class Networking
{
    private static PlayerControl FindPlayer(byte id)
    {
        foreach (var p in PlayerControl.AllPlayerControls)
            if (p != null && p.Data != null && p.Data.PlayerId == id) return p;
        return null;
    }

    public static void RpcPromotePlayer(PlayerControl player, Permissions level, bool shouldNotify = true)
    {
        if (!AmongUsClient.Instance.AmHost || player == null) return;

        var w = AmongUsClient.Instance.StartRpcImmediately(
            PlayerControl.LocalPlayer.NetId, 104, SendOption.Reliable, -1);
        w.Write(player.Data.PlayerId);
        w.Write((uint)level);
        w.Write(shouldNotify);
        AmongUsClient.Instance.FinishRpcImmediately(w);
        TrackingDataBehaviour.Get(player)?.SetPermissions(level);
    }

    public static void HandlePromotePlayer(PlayerControl sender, MessageReader reader)
    {
        var targetId = reader.ReadByte();
        var level = (Permissions)reader.ReadUInt32();
        bool shouldNotify = reader.ReadBoolean();
        if (sender.OwnerId != AmongUsClient.Instance.HostId) return;

        TrackingDataBehaviour.Get(FindPlayer(targetId))?.SetPermissions(level, shouldNotify);
    }

    public static void CmdKickPlayer(int clientId, bool ban)
    {
        var w = AmongUsClient.Instance.StartRpcImmediately(
            PlayerControl.LocalPlayer.NetId, 102, SendOption.Reliable, AmongUsClient.Instance.HostId);
        w.Write(clientId);
        w.Write(ban);
        AmongUsClient.Instance.FinishRpcImmediately(w);
    }

    public static void HandleKickPlayer(PlayerControl source, MessageReader reader)
    {
        var clientId = reader.ReadInt32();
        var ban = reader.ReadBoolean();

        var client = AmongUsClient.Instance;
        if (!client.AmHost) return;
        if (!source.IsModerator()) return;
        if (clientId == client.HostId) return;
        if (client.GetClient(clientId) == null) return;

        client.KickPlayer(clientId, ban);
    }

    public static void CmdSyncSettings()
    {
        var w = AmongUsClient.Instance.StartRpcImmediately(
            PlayerControl.LocalPlayer.NetId, 101, SendOption.Reliable, AmongUsClient.Instance.HostId);
        GameManager.Instance.LogicOptions.Serialize(w);
        AmongUsClient.Instance.FinishRpcImmediately(w);
    }
    
    public static void HandleSyncSettings(PlayerControl source, MessageReader reader)
    {
        if (TrackingDataBehaviour.Local.permissions != Permissions.Host) return;
        if (!source.CanModifySettings()) return;

        var logic = GameManager.Instance?.LogicOptions;
        var optionsManager = GameOptionsManager.Instance;
        if (logic == null || optionsManager == null) return;

        HostUtilityPlugin.Instance.Log.LogInfo($"[101] from {source.PlayerId}");
        logic.Deserialize(reader);
        optionsManager.GameHostOptions = optionsManager.CurrentGameOptions;
        logic.SyncOptions();
        HostUtilityPlugin.Instance.Log.LogInfo("DING DING DING!!! SYNCED!!!");
        return;
    }
}