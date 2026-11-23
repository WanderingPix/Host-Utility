using System.Collections.Generic;
using HostUtility.Components;
using HostUtility.GameSettings.OptionTypes;
using UnityEngine;

namespace HostUtility.GameSettings;

public class AdvancedTab : GameSettingsTab
{
    public override string Name => "Advanced";
    public override List<OptionGroup> OptionGroups => [
        new OptionGroup()
        {
            Name = "Player Information",
            Options = [
                ShowPlayerIDs,
                ShowPlayerLevels,
                ShowPlayerPlatforms
            ]
        },
        
        new OptionGroup()
        {
            Name = "Game Flow",
            Options = [
                DisableGameEnd,
                GameStartCountdown
            ]
        }
    ];
    public ModdedBoolOption ShowPlayerIDs = 
        new ModdedBoolOption("Show Player IDs", false)
        {
            OnValueChanged = value =>
            {
                foreach (var icon in PlayerIconsBehaviour.AllIcons)
                {
                    icon.IdIcon.SetActive(value);
                }
            }
        };
    public ModdedBoolOption ShowPlayerLevels = 
        new ModdedBoolOption("Show Player Levels", false)
        {
            OnValueChanged = value =>
            {
                foreach (var icon in PlayerIconsBehaviour.AllIcons)
                {
                    icon.LevelIcon.SetActive(value);
                }
            }
        };
    public ModdedBoolOption ShowPlayerPlatforms = 
        new ModdedBoolOption("Show Player Platforms", false)
        {
            OnValueChanged = value =>
            {
                foreach (var icon in PlayerIconsBehaviour.AllIcons)
                {
                    icon.PlatformIcon.SetActive(value);
                }
            }
        };
    public ModdedBoolOption DisableGameEnd = 
        new ModdedBoolOption("Disable Game End", false);
    public ModdedNumberOption GameStartCountdown = 
        new ModdedNumberOption("Game Start Countdown", 5, 0, 25, 5, "s");
}