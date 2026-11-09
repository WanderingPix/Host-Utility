using System.Collections.Generic;
using System.Linq;
using HostUtility.Commands.Arguments;

namespace HostUtility.Commands;

public class BanCommand : ChatCommand
{
    public override string Command => "ban";

    public override string Description => "Bans a specified player.";

    public override List<CommandArgument> Arguments => new(new []
    {
        new PlayerArgument("TargetPlayer",player => !player.AmOwner)
    });

    public override void Execute(string[] args)
    {
        var target = (Arguments[0] as PlayerArgument)?.Deserialize(args[0]);
        if (target == null) return;
        AmongUsClient.Instance.KickPlayer(AmongUsClient.Instance.GetClientFromCharacter(target).Id, true);
    }
}