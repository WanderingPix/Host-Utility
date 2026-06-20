using BepInEx;
using BepInEx.Configuration;
using BepInEx.Unity.IL2CPP;
using HarmonyLib;
using Reactor;
using Reactor.Utilities;

namespace HostUtility;

[BepInAutoPlugin]
[BepInProcess("Among Us.exe")]
[BepInDependency(ReactorPlugin.Id)]
public partial class HostUtilityPlugin : BasePlugin
{
    public ConfigEntry<int> MinLevel;
    public ConfigEntry<bool> BanInappropriateNames;
    public ConfigEntry<bool> BanInappropriateMessages;
    public Harmony Harmony { get; } = new(Id);
    public override void Load()
    {
        BanWords.Initialize();
        Harmony.PatchAll();
        ChatCommandsManager.Initialize();
        Log.LogInfo("Host Utility loaded successfully! :D");
        MinLevel = Config.Bind<int>("Join Conditions", "Minimum Level", 0);
        BanInappropriateNames = Config.Bind<bool>("Join Conditions", "Ban Inappropriate Names", true);
        BanInappropriateMessages = Config.Bind<bool>("Join Conditions", "Ban Inappropriate Messages", true);
    }
}