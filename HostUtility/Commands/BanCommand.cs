using System.Collections.Generic;
using System.Linq;

namespace HostUtility.Commands;

public class BanCommand : ChatCommand
{
    public override string Command => "ban";

    public override string Description => "Bans a specified player.";

    public override List<CommandArgument> Arguments => new(new []
    {
        new CommandArgument(CommandArgument.ArgumentType.Player, "Player Name")
    });

    public override void Execute(string[] args)
    {
        string playerName = args[0];
        var target = PlayerControl.AllPlayerControls.ToArray().First(x => x.Data.PlayerName == playerName);
        if (target == null) return;
        AmongUsClient.Instance.KickWithReason(target.Data.ClientId, "Banned by host", true);
    }
}