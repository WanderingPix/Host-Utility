using System.Collections.Generic;
using Reactor.Utilities;

namespace HostUtility.Commands;

public class ConfigCommand : ChatCommand
{
    public override string Command => "config";

    public override string Description => "Helpful command for configuring the mod.";

    public override List<CommandArgument> Arguments => [
        new CommandArgument(CommandArgument.ArgumentType.String, "Option Name"),
        new CommandArgument(CommandArgument.ArgumentType.String, "Value")
    ];

    public override string GetInfoText()
    {
        var plugin = PluginSingleton<HostUtilityPlugin>.Instance;
        return "List of available options:\n" +
               "<size=80%>" + 
               $"<b>ChatMessageFilter</b> : Determines if chat messages should be checked for bad words, and kicks the player. Currently set to: {plugin.BanInappropriateMessages.Value}\n" + 
               $"<b>PlayerNamesFilter</b> : Determines if player names should be checked for bad words, and bans the player. Currently set to: {plugin.BanInappropriateNames.Value}\n" + 
               $"<b>MinimumLevel</b> : Determines the minimum level needed for players in this lobby, and kicks any players below it. Currently set to: {plugin.MinLevel.Value}\n" + 
               $"<b>KickSuspectedPlayers</b> : Auto kicks suspected e-daters and predators depending on if they're in the AU Files registry. Currently set to: {plugin.KickSuspectedPlayers.Value}\n" +
               $"<b>GameStartCountdownTime</b> : Determines the countdown time for the game starting. Currently set to: {plugin.GameStartCountdownTime.Value}\n" +
               "</size>";
    }

    public override void Execute(string[] args)
    {
        var plugin = PluginSingleton<HostUtilityPlugin>.Instance;
        var opt = args[0].ToLower();
        var value = args[1].ToLower();
        switch (opt)
        {
            case "chatmessagefilter":
                plugin.BanInappropriateMessages.Value = bool.Parse(value);
                break;
            case "playernamesfilter":
                plugin.BanInappropriateNames.Value = bool.Parse(value);
                break;
            case "minimumlevel":
                plugin.MinLevel.Value = int.Parse(value);
                break;
            case "kicksuspectedplayers":
                plugin.KickSuspectedPlayers.Value = bool.Parse(value);
                break;
            case "gamestartcountdowntime":
                plugin.GameStartCountdownTime.Value = int.Parse(value);
                break;
            default:
                HudManager.Instance.Chat.AddChatWarning(base.GetInfoText());
                return;
                break;
        }
        HudManager.Instance.Chat.AddChatWarning("Changes saved successfully.");
    }
}