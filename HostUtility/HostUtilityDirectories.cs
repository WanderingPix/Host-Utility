using System;
using System.IO;
using BepInEx;

namespace HostUtility;

public static class HostUtilityDirectories
{
    public static string GetDataPath()
    {
        return OperatingSystem.IsAndroid() || OperatingSystem.IsIOS() ? Environment.GetEnvironmentVariable("STAR_DATA_PATH") : Paths.BepInExRootPath;
    }
    public static string RulesetsPath => Path.Combine(GetDataPath(), "Rulesets");

    public static void InitializeDirectories()
    {
        if (!Directory.Exists(RulesetsPath))
        {
            Directory.CreateDirectory(RulesetsPath);
        }
    }
}