using System;
using System.Collections.Generic;
using System.Linq;
using BepInEx.Configuration;
using HostUtility.GameSettings.OptionTypes;

namespace HostUtility.GameSettings;

public static class SettingTabSingleton<T> where T : GameSettingsTab
{
    public static T Instance => GameSettingsManager.Tabs.FirstOrDefault(x => x is T) as T;
}