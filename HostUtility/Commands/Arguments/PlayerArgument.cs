using System;
using System.Linq;
using HostUtility.Components.UI;

namespace HostUtility.Commands.Arguments;

public class PlayerArgument : CommandArgument<PlayerControl>
{
    public PlayerArgument(string name, Predicate<PlayerControl> isPlayerValid)
    {
        ArgumentName = name;
        IsPlayerValid = isPlayerValid;
    }

    public override string Serialize(PlayerControl value)
    {
        return value.Data.PlayerName;
    }

    public Predicate<PlayerControl> IsPlayerValid;

    public override PlayerControl Deserialize(string value)
    {
        return PlayerControl.AllPlayerControls.ToArray().First(x => x.Data.PlayerName == value);
    }

    public override void CreateOptions(CommandArgumentsMenu menu)
    {
        foreach (var player in PlayerControl.AllPlayerControls.ToArray().Where(IsPlayerValid.Invoke))
        {
            menu.CreateOption(player.Data.PlayerName, Serialize(player), CommandArgumentsMenu.ArgumentSpriteTypes.Player, true, player.cosmetics.ColorId);
        }
    }
}