/*using System.Collections.Generic;
using System.Linq;
using AmongUs.GameOptions;
using UnityEngine;

namespace HostUtility.Commands;

public class PlayerCapCommand : ChatCommand
{
    public override string Command => "playercap";

    public override string Description => "Set the lobby's player cap to a new value, minimum is 4, max is 255.";

    public override List<CommandArgument> Arguments => new(new []
    {
        new CommandArgument(CommandArgument.ArgumentType.Int, "New Cap")
    });

    public override void Execute(string[] args)
    {
        int.TryParse(args[0], out int val);
        val = Mathf.Clamp(val, 4, 255);
        GameOptionsManager.Instance.CurrentGameOptions.SetInt(Int32OptionNames.MaxPlayers, val);
    }
}*/