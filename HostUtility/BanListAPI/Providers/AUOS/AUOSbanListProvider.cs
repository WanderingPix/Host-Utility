using System;
using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text.Json;
using BepInEx.Configuration;
using HostUtility.AUOS.API;
using InnerNet;
using Reactor.Utilities;
using UnityEngine;
using UnityEngine.Networking;

namespace HostUtility.BanListAPI.Providers.AUOS;

public class AUOSbanListProvider : BanListProvider
{
    public static AUOSResponse Data;

    public override string Name => "Among Us Online Safety";
    public override string Owner => "WanderingPixel, CloneC";
    public ConfigEntry<float> MinimumHonor;
    public override void Initialize()
    {
        MinimumHonor =
            PluginSingleton<HostUtilityPlugin>.Instance.Config.Bind("Ban List Providers - " + Name, "Minimum Honor", 40f);
        Settings.Add(MinimumHonor);
        base.Initialize();
    }

    public override IEnumerator CoLoadFromCloud()
    {
        State = ProviderState.Uninitialized;
        var plugin = PluginSingleton<HostUtilityPlugin>.Instance;
        plugin.Log.LogInfo("Began request to AUOS API for player registry.");

        var request = UnityWebRequest.Get("https://orftdpwzariqxnovqgtu.supabase.co/rest/v1/players");
        request.SetRequestHeader("apikey", "sb_publishable_bcpRUHL2FoycOrKGPJ2FUg_9v1gFsk2");
        request.SetRequestHeader("Accept", "application/json");
        yield return request.SendWebRequest();

        if (request.result != UnityWebRequest.Result.Success)
        {
            plugin.Log.LogError($"Failed to fetch AUOS registry: {request.error}");
            State = ProviderState.Failed;
            yield break;
        }

        var json = request.downloadHandler.text;
        var list = JsonSerializer.Deserialize<List<AUOSEntry>>(json);
        Data = new()
        {
            entries = list.ToArray()
        };
        plugin.Log.LogInfo($"Fetched {Data?.entries?.Length ?? 0} entries from AUOS API.");
        var cachedDataPath = GetCachedDataPath();
        File.WriteAllText(cachedDataPath, JsonSerializer.Serialize(Data));
        plugin.Log.LogInfo($"Cached the AUOS data in {cachedDataPath}.");
        State = ProviderState.Initialized;
        LastFetchTime.Value = DateTime.Today.ToString(CultureInfo.CurrentCulture);
    }


    public override void LoadFromCache()
    {
        Data = JsonSerializer.Deserialize<AUOSResponse>(File.ReadAllText(GetCachedDataPath()));
        Logger.GlobalInstance.Info($"Loaded {Data?.entries?.Length ?? 0} entries locally from AUOS API.");
        State = ProviderState.Initialized;
    }
    
    public override int GetSize()
    {
        return Data.totalCount;
    }

    public override bool IsTargetOnBanList(ClientData client, out string banReason)
    {
        banReason = "";
        bool isInDatabase = Data.entries.Count(x => x.Puid == client.ProductUserId) > 0;
        if (!isInDatabase) return false;
        var entry = Data.entries.First(x => x.Puid == client.ProductUserId);
        return entry.Score < MinimumHonor.Value;
    }
}