using System.Collections.Generic;
using HostUtility.GameSettings.OptionTypes;
using UnityEngine;

namespace HostUtility.GameSettings;

public class ModerationTab : GameSettingsTab
{
    public override string Name => "Moderation";
    public override List<OptionGroup> OptionGroups => [
        new OptionGroup()
        {
            Name = "Join Conditions",
            Options = [
                new ModdedNumberOption("Player Level Cap", 0, 0, 100, 5),
                
            ]
        }
    ];
}