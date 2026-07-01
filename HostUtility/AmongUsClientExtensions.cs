using System.Linq;

namespace HostUtility;

public static class AmongUsClientExtensions
{
    public static void KickWithReason(this AmongUsClient instance, int clientId, string reason, bool ban)
    {
        if (!instance.AmHost) return;
        var client = AmongUsClient.Instance.GetClient(clientId);
        if (FriendsListManager.Instance.IsPlayerFriend(client.ProductUserId)) return;
        AmongUsClient.Instance.KickPlayer(clientId, ban);
        if (HudManager.Instance)
        {
            string punishment = ban ? "banned" : "kicked";
            HudManager.Instance.Chat.AddChatWarning($"A player has been {punishment}.\n<size=60%>" +
                                                    $"Reason: {reason}");
        }
    }
}