using System;
using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Reflection;
using BepInEx.Configuration;
using HostUtility.AUFiles.API;
using HostUtility.AUOS.API;
using InnerNet;
using Reactor.Utilities;
using UnityEngine;

namespace HostUtility.BanListAPI;

public static class BanListManager
{
    public static List<BanListProvider> Providers;
    public static void Initialize()
    {
        Providers = new();
        foreach (var type in Assembly.GetAssembly(typeof(BanListManager))?.GetTypes().Where(x => x.IsAssignableTo(typeof(BanListProvider)) && !x.IsAbstract)!)
        {
            var instance = Activator.CreateInstance(type) as BanListProvider;
            Providers.Add(instance);
            instance?.Initialize();
        }
    }

    public static string GetMainMenuInfoText()
    {
        string text = string.Empty;
        foreach (var failedProvider in Providers.Where(x => x.State == ProviderState.Failed))
        {
            text += $"{failedProvider.Name} Ban List Status: <color=red>ERROR</color>\n";
        }
        foreach (var initializingProvider in Providers.Where(x => x.State == ProviderState.Uninitialized))
        {
            text += $"{initializingProvider.Name} Ban List Status: <color=yellow>Initializing...</color>\n";
        }
        foreach (var initializedProvider in Providers.Where(x => x.State == ProviderState.Initialized))
        {
            text += $"{initializedProvider.Name} Ban List Status: <color=green>Initialized ({initializedProvider.GetSize()} Entries) </color>\n";
        }
        return text;
    }

    public static bool IsTargetOnBanList(ClientData client, out string banReason, out string banListName)
    {
        foreach (var banListProvider in Providers.Where(x => x.State == ProviderState.Initialized && x.IsEnabled.Value))
        {
            if (banListProvider.IsTargetOnBanList(client, out banReason))
            {
                banListName = banListProvider.Name;
                return true;
            }
        }
        banReason = string.Empty;
        banListName = string.Empty;
        return false;
    }
}
