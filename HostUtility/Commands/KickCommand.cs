using System.Collections.Generic;
using System.Linq;
using HostUtility.Commands.Arguments;

namespace HostUtility.Commands;

public class KickCommand : ChatCommand
{
    public override string Command => "kick";

    public override string Description => "Kicks a specified player.";

    public override List<CommandArgument> Arguments => new(
        [
            new PlayerArgument("PlayerName", _ => true)
        ]
    );

    public override void Execute(string[] args)
    {
        var target = (Arguments[0] as PlayerArgument)?.Deserialize(args[0]);
        if (target == null) return;
        AmongUsClient.Instance.KickPlayer(AmongUsClient.Instance.GetClientFromCharacter(target).Id, false);
    }
}