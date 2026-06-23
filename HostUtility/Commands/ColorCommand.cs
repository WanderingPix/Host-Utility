using System.Collections.Generic;
using System.Linq;

namespace HostUtility.Commands;

public class ColorCommand : ChatCommand
{
    public override string Command => "color";

    public override string Description => "Sets a color for a certain player.";

    public override List<CommandArgument> Arguments => new(new []
    {
        new CommandArgument(CommandArgument.ArgumentType.Player, "PlayerName"),
        new CommandArgument(CommandArgument.ArgumentType.Color, "Color")
    });

    public override void Execute(string[] args)
    {
        string playerName = args[0];
        var target = PlayerControl.AllPlayerControls.ToArray().First(x => x.Data.PlayerName == playerName);
        if (target == null) return;
        string colorName = args[1];
        var name = Palette.ColorNames.First(x => TranslationController.Instance.GetString(x).ToLower() == colorName.ToLower());
        var index = Palette.ColorNames.IndexOf(name);
        
        target.RpcSetColor((byte)index);
    }
}