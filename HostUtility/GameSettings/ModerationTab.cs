using System.Collections.Generic;
using HostUtility.BanListAPI;
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
                PlayerLevelCap
            ]
        },
        new OptionGroup()
        {
            Name = "Filters",
            Options = [
                BanInappropriateNames,
                BanInappropriateMessages,
                KickSuspectedBots
            ]
        },
        new OptionGroup()
        {
            Name = "Toxic Behaviours",
            Options = [
                DisableR1ButtonPresses
            ]
        }
    ];

    public override void CreateSettings(GameOptionsMenu gameOptionsMenu, Transform parentTransform, out float y)
    {
        base.CreateSettings(gameOptionsMenu, parentTransform, out y);
        foreach (var banList in BanListManager.Providers)
        {
            var group = new OptionGroup()
            {
                Name = banList.Name + " Ban List",
                Options = banList.Options
            };
            group.Create(gameOptionsMenu, parentTransform, ref y);
        }
    }

    public ModdedBoolOption DisableR1ButtonPresses = new ModdedBoolOption("Disable R1 Button Presses", false);
    public ModdedBoolOption BanInappropriateNames = new ModdedBoolOption("Ban Inappropriate Names", false);
    public ModdedBoolOption BanInappropriateMessages = new ModdedBoolOption("Ban Inappropriate Messages", false);
    public ModdedBoolOption KickSuspectedBots = new ModdedBoolOption("Kick Suspected Bots", false);
    public ModdedNumberOption PlayerLevelCap = new ModdedNumberOption("Player Level Cap", 0, 0, 100, 5);
}