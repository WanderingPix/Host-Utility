using System.Linq;

namespace HostUtility;

public static class AmongUsClientExtensions
{
    public static void KickWithReason(this AmongUsClient instance, int clientId, string reason, string banListName, bool ban)
    {
        if (!instance.AmHost) return;
        var client = AmongUsClient.Instance.GetClient(clientId);
        if (FriendsListManager.Instance.IsPlayerFriend(client.ProductUserId)) return;
        AmongUsClient.Instance.KickPlayer(clientId, ban);
        if (HudManager.Instance)
        {
            string punishment = ban ? "banned" : "kicked";
            string optionalText1 = banListName == string.Empty ? string.Empty : $" for being on {banListName} ban list";
            string optionalText2 = banListName == string.Empty ? string.Empty : $"\n Reason: {reason}";
            HudManager.Instance.Chat.AddChatWarning($"A player has been {punishment}{optionalText1}.<size=60%>" +
                                                    optionalText2);
        }
    }
}