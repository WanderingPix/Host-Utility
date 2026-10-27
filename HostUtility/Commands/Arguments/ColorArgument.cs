using System;
using System.Linq;
using HostUtility.Components.UI;

namespace HostUtility.Commands.Arguments;

public class ColorArgument : CommandArgument<int>
{
    public ColorArgument(string name)
    {
        ArgumentName = name;
    }
    public override string Serialize(int value)
    {
        return value.ToString();
    }

    public override int Deserialize(string value)
    {
        return int.Parse(value);
    }

    public override void CreateOptions(CommandArgumentsMenu menu)
    {
        int i = 0;
        foreach (var color in Palette.ColorNames)
        {
            menu.CreateOption(color.ToString().Replace("Color", ""), Serialize(i), CommandArgumentsMenu.ArgumentSpriteTypes.Player, true, i);
            i++;
        }
    }
}