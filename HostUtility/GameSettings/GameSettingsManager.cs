using System;
using System.Collections.Generic;
using System.Linq;
using BepInEx.Configuration;
using HostUtility.GameSettings.OptionTypes;

namespace HostUtility.GameSettings;

public static class GameSettingsManager
{
    public static List<GameSettingsTab> Tabs = [];
    public static void Initialize()
    {
        var assembly = typeof(GameSettingsManager).Assembly;
        foreach (var type in assembly.GetTypes().Where(x => x.IsSubclassOf(typeof(GameSettingsTab)) && !x.IsAbstract))
        {
            var tab = Activator.CreateInstance(type) as GameSettingsTab;
            if (tab == null) continue;
            Tabs.Add(tab);
        }
    }
}