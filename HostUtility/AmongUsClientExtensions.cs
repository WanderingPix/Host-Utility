using System.Linq;

namespace HostUtility;

public static class AmongUsClientExtensions
{
    public static void KickWithReason(this AmongUsClient instance, int clientId, string reason, string banListName, bool ban)
    {
        if (!instance.AmHost) return;
        var client = AmongUsClient.Instance.GetClient(clientId);
        if (client.Character && client.Character.AmOwner) return;
        if (FriendsListManager.Instance.IsPlayerFriend(client.ProductUserId)) return;
        string properReason = reason.IsNullOrWhiteSpace() ? "None given" : reason;
        AmongUsClient.Instance.KickWithReason(clientId, $"You were kicked for being on the \n{banListName} player ban list.\n Reason: {properReason}" + reason, ban);
        if (HudManager.Instance)
        {
            string punishment = ban ? "banned" : "kicked";
            string optionalText1 = banListName == string.Empty ? string.Empty : $" for being on {banListName} ban list";
            string optionalText2 = banListName == string.Empty ? string.Empty : $"\n Reason: {reason}";
            HudManager.Instance.Chat.AddChatWarning($"A player has been {punishment}{optionalText1}.<size=60%>" +
                                                    optionalText2);
        }
    }
    
    public static void KickWithReason(this AmongUsClient instance, int clientId, string reason, bool ban)
    {
        if (!instance.AmHost) return;
        var client = AmongUsClient.Instance.GetClient(clientId);
        if (client.Character && client.Character.AmOwner) return;
        if (FriendsListManager.Instance.IsPlayerFriend(client.ProductUserId)) return;
        AmongUsClient.Instance.KickWithReason(clientId, reason, ban);
        if (HudManager.Instance)
        {
            string punishment = ban ? "banned" : "kicked";
            HudManager.Instance.Chat.AddChatWarning($"A player has been {punishment}.<size=60%>" +
                                                    $"Reason: {reason}");
        }
    }
}