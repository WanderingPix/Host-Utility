using System.Collections.Generic;
using Hazel;
using InnerNet;

namespace HostUtility;

public static class HandshakeManager
{
    public static List<PlayerControl> ModdedPlayers = [];
    public static bool IsModdedPlayer(PlayerControl player)
    {
        return ModdedPlayers.Contains(player);
    }

    public static void RpcHandshake(this PlayerControl player, int clientId = -1)
    {
        var writer = AmongUsClient.Instance.StartRpcImmediately(
            player.NetId, 100, SendOption.Reliable, clientId);
        AmongUsClient.Instance.FinishRpcImmediately(writer);
    }
}