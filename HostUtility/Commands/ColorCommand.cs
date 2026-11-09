using System.Collections.Generic;
using System.Linq;
using HostUtility.Commands.Arguments;

namespace HostUtility.Commands;

public class ColorCommand : ChatCommand
{
    public override string Command => "color";

    public override string Description => "Sets a color for a certain player.";

    public override List<CommandArgument> Arguments => new([
        new PlayerArgument("TargetPlayer",_ => true),
        new ColorArgument("ColorName")
    ]);

    public override void Execute(string[] args)
    {
        var target = (Arguments[0] as PlayerArgument)?.Deserialize(args[0]);
        if (target == null) return;
        var colorId = (Arguments[1] as ColorArgument)?.Deserialize(args[1]);

        if (colorId != null) target.RpcSetColor((byte)colorId);
    }
}