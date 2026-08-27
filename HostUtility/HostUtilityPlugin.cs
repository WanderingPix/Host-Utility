using System;
using System.Net.Http;
using BepInEx;
using BepInEx.Configuration;
using BepInEx.Unity.IL2CPP;
using HarmonyLib;
using HostUtility.AUFiles;
using HostUtility.AUOS;
using HostUtility.PlayerReporting;
using Reactor;
using Reactor.Networking;
using Reactor.Networking.Attributes;
using Reactor.Utilities;
using UnityEngine;

namespace HostUtility;

[BepInAutoPlugin("com.missingpixel.hostutility", "Host Utility", "1.4.0")]
[BepInProcess("Among Us.exe")]
[BepInDependency(ReactorPlugin.Id)]
public partial class HostUtilityPlugin : BasePlugin
{
    public ConfigEntry<int> MinLevel;
    public ConfigEntry<int> GameStartCountdownTime;
    public ConfigEntry<bool> BanInappropriateNames;
    public ConfigEntry<bool> BanInappropriateMessages;
    public ConfigEntry<bool> KickSuspectedPlayers;
    public ConfigEntry<bool> CheckMessageCooldowns;
    public ConfigEntry<bool> KickSuspectedBots;
    public ConfigEntry<bool> ShowPlayerIDs;
    public ConfigEntry<bool> ShowPlayerPlatforms;
    public ConfigEntry<string> AuFilesLastFetchTime;
    public ConfigEntry<string> AuOsLastFetchTime;
    public ConfigEntry<int> MinHonor;
    
    public static AssetBundle Bundle;
    public Harmony Harmony { get; } = new(Id);
    public override void Load()
    {
        BanWords.Initialize();
        Harmony.PatchAll();
        ChatCommandsManager.Initialize();
        Bundle = AssetBundleManager.Load("hostutil");
        
        MinLevel = Config.Bind<int>("Join Conditions", "Minimum Level", 0);
        MinHonor = Config.Bind<int>("Join Conditions", "Minimum Honor", 20);
        BanInappropriateNames = Config.Bind<bool>("Join Conditions", "Ban Inappropriate Names", true);
        BanInappropriateMessages = Config.Bind<bool>("Join Conditions", "Ban Inappropriate Messages", true);
        KickSuspectedPlayers = Config.Bind<bool>("Join Conditions", "Kick Suspected E-Daters and PDFs", true);
        KickSuspectedBots = Config.Bind<bool>("Join Conditions", "Kick Suspected Bots", true);
        
        CheckMessageCooldowns = Config.Bind<bool>("Anticheat", "Force message cooldowns", true);
        
        GameStartCountdownTime = Config.Bind("Game", "Game Start Countdown Time", 5);
        
        ShowPlayerIDs = Config.Bind<bool>("Advanced", "Show Player IDs", false);
        ShowPlayerPlatforms = Config.Bind<bool>("Advanced", "Show Player Platforms", false);
        
        AuFilesLastFetchTime = Config.Bind<string>("AU Files", "Last time fetched", "");
        AuOsLastFetchTime = Config.Bind<string>("AUOS", "Last time fetched", "");
        AUFilesManager.Initialize();
        AuosManager.Initialize();
        ReactorCredits.Register<HostUtilityPlugin>(_ => true);
        Log.LogInfo("Host Utility loaded successfully! :D");
    }
}