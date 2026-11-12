using System.Collections.Generic;
using HostUtility.GameSettings.OptionTypes;
using UnityEngine;

namespace HostUtility.GameSettings;

public class AnticheatTab : GameSettingsTab
{
    public override string Name => "Anticheat";
    public override List<OptionGroup> OptionGroups => [
        new OptionGroup()
        {
            Name = "Anti-Cheat",
            Options = [
                CheckCheatRPCs,
                RestrictMurderRPC,
                RestrictSetColorRPC,
                RestrictMeetingRPC,
                EnforceMessageCooldowns
            ]
        }
    ];
    public ModdedBoolOption RestrictMurderRPC = 
        new ModdedBoolOption("Restrict Murder RPC", false);
    public ModdedBoolOption RestrictSetColorRPC = 
        new ModdedBoolOption("Restrict SetColor RPC", false);
    public ModdedBoolOption RestrictMeetingRPC = 
        new ModdedBoolOption("Restrict Meeting RPC", false);
    public ModdedBoolOption CheckCheatRPCs = 
        new ModdedBoolOption("Check For Known Cheats", false);
    public ModdedBoolOption EnforceMessageCooldowns = 
        new ModdedBoolOption("Enforce For Known Cheats", false);
}