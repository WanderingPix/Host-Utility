using System;
using System.IO;
using BepInEx;

namespace HostUtility;

public static class HostUtilityDirectories
{
    private static string GetDataPath()
    {
        return Path.Combine((OperatingSystem.IsAndroid() || OperatingSystem.IsIOS()
            ? Environment.GetEnvironmentVariable("STAR_DATA_PATH")
            : Paths.GameRootPath)!, "Host Utility");
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