using System;
using System.Linq;
using HostUtility.Components.UI;

namespace HostUtility.Commands.Arguments;

public class BoolArgument : CommandArgument<bool>
{
    public BoolArgument(string name)
    {
        ArgumentName = name;
    }

    public override string Serialize(bool value)
    {
        return value.ToString();
    }

    public override bool Deserialize(string value)
    {
        return bool.Parse(value);
    }

    public override void CreateOptions(CommandArgumentsMenu menu)
    {
        menu.CreateOption("True", true.ToString(), CommandArgumentsMenu.ArgumentSpriteTypes.Checkmark);
        menu.CreateOption("False", false.ToString(), CommandArgumentsMenu.ArgumentSpriteTypes.Cross);
    }
}