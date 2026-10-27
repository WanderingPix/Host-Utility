using System.Collections.Generic;
using System.Linq;
using HostUtility.Commands.Arguments;

namespace HostUtility.Commands;

public class ColorAllCommand : ChatCommand
{
    public override string Command => "colorall";

    public override string Description => "Sets a color for all players.";

    public override List<CommandArgument> Arguments => new([
        new ColorArgument("ColorName")
    ]);

    public override void Execute(string[] args)
    {
        var colorId = (Arguments[0] as ColorArgument)?.Deserialize(args[0]);
        if (colorId == null) return;
        foreach (var player in PlayerControl.AllPlayerControls)
        {
            player.RpcSetColor((byte)colorId);
        }
    }
}