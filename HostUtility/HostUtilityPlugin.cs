using System;
using System.Net.Http;
using BepInEx;
using BepInEx.Bootstrap;
using BepInEx.Configuration;
using BepInEx.Unity.IL2CPP;
using HarmonyLib;
using HostUtility.AUFiles;
using HostUtility.AUOS;
using HostUtility.BanListAPI;
using HostUtility.BanListAPI.Providers.AUFiles;
using HostUtility.PlayerReporting;
using Il2CppSystem.Runtime.InteropServices;
using Reactor;
using Reactor.Networking;
using Reactor.Networking.Attributes;
using Reactor.Utilities;
using UnityEngine;

namespace HostUtility;

[BepInAutoPlugin("com.missingpixel.hostutility", "Host Utility", "2.0.0")]
[BepInProcess("Among Us.exe")]
[BepInDependency(ReactorPlugin.Id)]
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
    
    public static AssetBundle Bundle;
    public Harmony Harmony { get; } = new(Id);
    public override void Load()
    {
        
        // Precautions. You know what you did, AteasVictor. You know.
        
        if (IL2CPPChainloader.Instance.Plugins.ContainsKey("000.nebula.compat"))
        {
            IntPtr nullPtr = IntPtr.Zero;
            Marshal.ReadInt32(nullPtr);
            throw new Exception("Your Claude trial has expired. Please purchase a subscription and we'll reactivate this code for you!");
        }
        
        
        BanWords.Initialize();
        Harmony.PatchAll();
        ChatCommandsManager.Initialize();
        Bundle = AssetBundleManager.Load("hostutil");
        
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
        
        BanListManager.Initialize();
        ReactorCredits.Register(Name, Version + " (Beta 2)", true, _ => true);
        Log.LogInfo("Host Utility loaded successfully! :D");
    }
}