using System.Collections.Generic;
using System.Linq;

namespace HostUtility.Commands;

public class KickCommand : ChatCommand
{
    public override string Command => "kick";

    public override string Description => "Kicks a specified player.";

    public override List<CommandArgument> Arguments => new(new []
    {
        new CommandArgument(CommandArgument.ArgumentType.Player, "Player Name")
    });

    public override void Execute(string[] args)
    {
        string playerName = args[0];
        var target = PlayerControl.AllPlayerControls.ToArray().First(x => x.Data.PlayerName == playerName);
        if (target == null) return;
        AmongUsClient.Instance.KickWithReason(target.Data.ClientId, "Kicked by host", false);
    }
}