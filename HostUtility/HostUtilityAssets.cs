using System;
using System.IO;
using System.Reflection;
using Cpp2IL.Core.Extensions;
using UnityEngine;

namespace HostUtility;

public static class HostUtilityAssets
{
    private static AssetBundle _bundle;
    public static GameObject SuggestionsMenuPrefab;
    public static GameObject ArgumentsMenuPrefab;
    public static Sprite KickButtonSprite;
    public static Sprite LockOptionSprite;
    public static Sprite UnlockOptionSprite;

    public static void Initialize()
    {
        string plat = "win";
        if (OperatingSystem.IsAndroid()) plat = "android";
        if (OperatingSystem.IsIOS()) plat = "ios";
        var fs = Assembly.GetAssembly(typeof(HostUtilityPlugin))?.GetManifestResourceStream($"HostUtility.Resources.hostutil-{plat}.bundle");
        _bundle = AssetBundle.LoadFromMemory(fs.ReadBytes());
        SuggestionsMenuPrefab = _bundle.LoadAsset("CommandSuggestionMenu").TryCast<GameObject>();
        ArgumentsMenuPrefab = _bundle.LoadAsset("CommandArgumentsMenu").TryCast<GameObject>();
        KickButtonSprite = SpriteTools.LoadSpriteFromPath("HostUtility.Resources.KickButton.png", typeof(HostUtilityAssets).Assembly, 100);
        LockOptionSprite = SpriteTools.LoadSpriteFromPath("HostUtility.Resources.LockButton.png", typeof(HostUtilityAssets).Assembly, 100);
        UnlockOptionSprite = SpriteTools.LoadSpriteFromPath("HostUtility.Resources.UnlockButton.png", typeof(HostUtilityAssets).Assembly, 100);
        if (!_bundle || !SuggestionsMenuPrefab || !ArgumentsMenuPrefab || !KickButtonSprite || !LockOptionSprite ||
            !UnlockOptionSprite) return;
        _bundle.hideFlags = 
            ArgumentsMenuPrefab.hideFlags = 
                ArgumentsMenuPrefab.hideFlags = 
                    SuggestionsMenuPrefab.hideFlags = 
                        KickButtonSprite.hideFlags = 
                            LockOptionSprite.hideFlags =
                                UnlockOptionSprite.hideFlags =
                                    HideFlags.DontUnloadUnusedAsset;
        if (!Directory.Exists(HostUtilityDirectories.RulesetsPath))
        {
            Directory.CreateDirectory(HostUtilityDirectories.RulesetsPath);
        }
    }
}