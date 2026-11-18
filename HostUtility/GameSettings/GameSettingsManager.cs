using System;
using System.Collections.Generic;
using System.Linq;
using BepInEx.Configuration;
using HostUtility.GameSettings.OptionTypes;

namespace HostUtility.GameSettings;

public static class GameSettingsManager
{
    public static List<GameSettingsTab> Tabs = [
        new RulesetsTab(),
        new ModerationTab(),
        new AnticheatTab(),
        new PlayerManagementTab(),
        new AdvancedTab()
    ];
}