using System;
using System.Reflection;
using Cpp2IL.Core.Extensions;
using UnityEngine;

namespace HostUtility;

public static class HostUtilityAssets
{
    private static AssetBundle _bundle;
    public static GameObject SuggestionsMenuPrefab;
    public static GameObject ArgumentsMenuPrefab;

    public static void Initialize()
    {
        string plat = "win";
        if (OperatingSystem.IsAndroid()) plat = "android";
        if (OperatingSystem.IsIOS()) plat = "ios";
        var fs = Assembly.GetAssembly(typeof(HostUtilityPlugin))?.GetManifestResourceStream($"HostUtility.Resources.hostutil-{plat}.bundle");
        _bundle = AssetBundle.LoadFromMemory(fs.ReadBytes());
        SuggestionsMenuPrefab = _bundle.LoadAsset("CommandSuggestionMenu").TryCast<GameObject>();
        ArgumentsMenuPrefab = _bundle.LoadAsset("CommandArgumentsMenu").TryCast<GameObject>();
        SuggestionsMenuPrefab?.hideFlags = HideFlags.DontUnloadUnusedAsset;
        ArgumentsMenuPrefab?.hideFlags = HideFlags.DontUnloadUnusedAsset;
        _bundle.hideFlags = HideFlags.DontUnloadUnusedAsset;
    }
}