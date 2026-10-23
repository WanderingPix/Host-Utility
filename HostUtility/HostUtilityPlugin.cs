using System;
using BepInEx;
using BepInEx.Configuration;
using BepInEx.Unity.IL2CPP;
using HarmonyLib;
using HostUtility.BanListAPI;
using HostUtility.Components;
using HostUtility.Components.UI;
using Il2CppInterop.Runtime.Injection;
using Il2CppSystem.Runtime.InteropServices;
using UnityEngine;

namespace HostUtility;

[BepInAutoPlugin("com.missingpixel.hostutility", "Host Utility", "2.0.1")]
[BepInProcess("Among Us.exe")]
[BepInDependency("000.nebula.compat", BepInDependency.DependencyFlags.SoftDependency)]
public partial class HostUtilityPlugin : BasePlugin
{
    public ConfigEntry<int> MinLevel;
    public ConfigEntry<int> GameStartCountdownTime;
    public ConfigEntry<bool> BanInappropriateNames;
    public ConfigEntry<bool> BanInappropriateMessages;
    public ConfigEntry<bool> CheckMessageCooldowns;
    public ConfigEntry<bool> KickSuspectedBots;
    public ConfigEntry<bool> ShowPlayerIDs;
    public ConfigEntry<bool> ShowPlayerPlatforms;
    public ConfigEntry<bool> ShowPlayerLevels;
    public ConfigEntry<bool> GameEndEnabled;
    
    public static AssetBundle Bundle;
    public Harmony Harmony { get; } = new(Id);
    public override void Load()
    {
        Instance = this;
        // Precautions. You know what you did, AteasVictor. You know.
        
        if (IL2CPPChainloader.Instance.Plugins.ContainsKey("000.nebula.compat"))
        {
            IntPtr nullPtr = IntPtr.Zero;
            Marshal.ReadInt32(nullPtr);
            throw new Exception("Your Claude trial has expired. Please purchase a subscription and we'll reactivate this code for you!");
        }
        
        BanWords.Initialize();
        Harmony.PatchAll();
        
        ClassInjector.RegisterTypeInIl2Cpp<PlayerIconsBehaviour>();
        ClassInjector.RegisterTypeInIl2Cpp<TrackingDataBehaviour>();
        ClassInjector.RegisterTypeInIl2Cpp<CommandArgumentsMenu>();
        ClassInjector.RegisterTypeInIl2Cpp<CommandSuggestionMenu>();
        ChatCommandsManager.Initialize();
        HostUtilityAssets.Initialize();
        
        MinLevel = Config.Bind<int>("Join Conditions", "Minimum Level", 0);
        BanInappropriateNames = Config.Bind<bool>("Join Conditions", "Ban Inappropriate Names", true);
        BanInappropriateMessages = Config.Bind<bool>("Join Conditions", "Ban Inappropriate Messages", true);
        KickSuspectedBots = Config.Bind<bool>("Join Conditions", "Kick Suspected Bots", true);
        
        CheckMessageCooldowns = Config.Bind<bool>("Anticheat", "Force message cooldowns", true);
        
        GameStartCountdownTime = Config.Bind("Game", "Game Start Countdown Time", 5);
        
        ShowPlayerIDs = Config.Bind<bool>("Advanced", "Show Player IDs", false);
        ShowPlayerPlatforms = Config.Bind<bool>("Advanced", "Show Player Platforms", true);
        ShowPlayerPlatforms = Config.Bind<bool>("Advanced", "Show Player Platforms", true);
        ShowPlayerLevels = Config.Bind<bool>("Advanced", "Show Player Levels", true);
        GameEndEnabled = Config.Bind<bool>("Advanced", "Game End", true);
        
        BanListManager.Initialize();
        Log.LogInfo("Host Utility loaded successfully! :D");
    }

    public static HostUtilityPlugin Instance { get; set; }
}