using System;
using BepInEx;
using BepInEx.Configuration;
using BepInEx.Unity.IL2CPP;
using HarmonyLib;
using HostUtility.BanListAPI;
using HostUtility.Components;
using HostUtility.Components.UI;
using HostUtility.GameSettings;
using Il2CppInterop.Runtime.Injection;
using Il2CppSystem.Runtime.InteropServices;
using UnityEngine;

namespace HostUtility;

[BepInAutoPlugin("com.missingpixel.hostutility", "Host Utility", "2.2.0")]
[BepInProcess("Among Us.exe")]
[BepInDependency("000.nebula.compat", BepInDependency.DependencyFlags.SoftDependency)]
public partial class HostUtilityPlugin : BasePlugin
{
    public ConfigEntry<bool> UseHostAuthority;
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
        ClassInjector.RegisterTypeInIl2Cpp<PaginationBehaviour>();
        ClassInjector.RegisterTypeInIl2Cpp<RulesetEditTab>();
        ChatCommandsManager.Initialize();
        HostUtilityAssets.Initialize();
        
        UseHostAuthority = Config.Bind<bool>("Advanced", "Use Host Authority", true);
        
        GameSettingsManager.Initialize();
        HostUtilityDirectories.InitializeDirectories();
        Log.LogInfo("Host Utility loaded successfully! :D");
    }

    public static HostUtilityPlugin Instance { get; set; }
}