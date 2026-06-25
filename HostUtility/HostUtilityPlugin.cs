using System;
using System.Net.Http;
using BepInEx;
using BepInEx.Configuration;
using BepInEx.Unity.IL2CPP;
using HarmonyLib;
using HostUtility.AUFiles;
using Reactor;
using Reactor.Utilities;

namespace HostUtility;

[BepInAutoPlugin("com.missingpixel.hostutility", "Host Utility", "1.1.1")]
[BepInProcess("Among Us.exe")]
[BepInDependency(ReactorPlugin.Id)]
public partial class HostUtilityPlugin : BasePlugin
{
    public ConfigEntry<int> MinLevel;
    public ConfigEntry<int> GameStartCountdownTime;
    public ConfigEntry<bool> BanInappropriateNames;
    public ConfigEntry<bool> BanInappropriateMessages;
    public ConfigEntry<bool> KickSuspectedPlayers;
    public ConfigEntry<string> LastFetchTime;
    public Harmony Harmony { get; } = new(Id);
    public override void Load()
    {
        BanWords.Initialize();
        Harmony.PatchAll();
        ChatCommandsManager.Initialize();
        ReactorCredits.Register<HostUtilityPlugin>(_ => true);
        Log.LogInfo("Host Utility loaded successfully! :D");
        MinLevel = Config.Bind<int>("Join Conditions", "Minimum Level", 0);
        BanInappropriateNames = Config.Bind<bool>("Join Conditions", "Ban Inappropriate Names", true);
        BanInappropriateMessages = Config.Bind<bool>("Join Conditions", "Ban Inappropriate Messages", true);
        KickSuspectedPlayers = Config.Bind<bool>("Join Conditions", "Kick Suspected E-Daters and PDFs", true);
        GameStartCountdownTime = Config.Bind("Game", "Game Start Countdown Time", 5);
        LastFetchTime = Config.Bind<string>("AU Files", "Last time fetched", "");
        AUFilesManager.Initialize();
    }
}