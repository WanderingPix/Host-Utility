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
            var options = banList.Options;
            if (!options.Contains(banList.IsEnabled)) options.Add(banList.IsEnabled);
            var group = new OptionGroup()
            {
                Name = banList.Name + " Ban List",
                Options = options
            };
            group.Create(gameOptionsMenu, parentTransform, ref y);
        }
    }

    public ModdedBoolOption DisableR1ButtonPresses = new ModdedBoolOption("Disable R1 Button Presses", false);
    public ModdedBoolOption BanInappropriateNames = new ModdedBoolOption("Ban Inappropriate Names", false);
    public ModdedBoolOption BanInappropriateMessages = new ModdedBoolOption("Ban Inappropriate Messages", false);
    public ModdedBoolOption KickSuspectedBots = new ModdedBoolOption("Kick Suspected Bots", false);
    public ModdedFloatRangeOption PlayerLevelCap = new ModdedFloatRangeOption()
    {
        Name = "Player Level Range",
        Range = new FloatRange(0, 100),
        DefaultValue = new FloatRange(0, 100),
        MinDistance = 5,
        RoundToInt = true
    };
}