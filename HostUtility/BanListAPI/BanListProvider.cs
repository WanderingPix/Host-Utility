using System;
using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using BepInEx.Configuration;
using HostUtility.AUOS.API;
using InnerNet;
using Reactor.Utilities;
using UnityEngine;

namespace HostUtility.BanListAPI;

public abstract class BanListProvider
{
    public abstract string Name { get; }
    public abstract string Owner { get; }
    public ProviderState State { get; set; } = ProviderState.Uninitialized;
    public ConfigEntry<string> LastFetchTime { get; private set; }
    public ConfigEntry<bool> IsEnabled { get; private set; }
    public virtual List<ConfigEntryBase> Settings { get; set; } = [];
    public string GetCachedDataPath()
    {
        return Path.Combine(
            (OperatingSystem.IsAndroid()
                ? Environment.GetEnvironmentVariable("STAR_DATA_PATH")
                : Application.persistentDataPath) ?? "", Name + ".json");
    }

    public virtual void Initialize()
    {
        LastFetchTime =
            PluginSingleton<HostUtilityPlugin>.Instance.Config.Bind("Ban List Providers - " + Name, "Last Fetch Time",
                "");
        IsEnabled = PluginSingleton<HostUtilityPlugin>.Instance.Config.Bind("Ban List Providers - " + Name,
            "Is Enabled", true);
        var dataPath = GetCachedDataPath();

        bool fetchedToday = LastFetchTime.Value != string.Empty
                            && DateTime.TryParse(LastFetchTime.Value, CultureInfo.CurrentCulture, DateTimeStyles.None,
                                out var lastFetch)
                            && lastFetch.Date == DateTime.Today;

        if (fetchedToday && File.Exists(dataPath))
        {
            Logger.GlobalInstance.Info($"User fetched {Name} BanList data from the cloud today, using local copy...");
            try
            {
                LoadFromCache();
                return;
            }
            catch (Exception ex)
            {
                Logger.GlobalInstance.Warning($"Cached {Name} BanList data was unreadable, refetching: {ex.Message}");
            }
        }

        Coroutines.Start(CoLoadFromCloud());
    }

    public abstract void LoadFromCache();
    public abstract IEnumerator CoLoadFromCloud();

    public abstract int GetSize();

    public abstract bool IsTargetOnBanList(ClientData client, out string banReason);
}

public enum ProviderState
{
    Uninitialized,
    Initialized,
    Failed
}
