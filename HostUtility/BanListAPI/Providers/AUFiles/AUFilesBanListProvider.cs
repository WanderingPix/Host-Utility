using System;
using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text.Json;
using BepInEx.Configuration;
using HostUtility.AUFiles.API;
using HostUtility.BanListAPI.Providers.AUFiles.API;
using InnerNet;
using Reactor.Utilities;
using UnityEngine;
using UnityEngine.Networking;

namespace HostUtility.BanListAPI.Providers.AUFiles;

public class AuFilesBanListProvider : BanListProvider
{
    public AUFilesResponse Data;

    public override string Name => "AUFiles";
    public override string Owner => "Bonked_TNT"; 

    public override void LoadFromCache()
    {
        Data = JsonSerializer.Deserialize<AUFilesResponse>(File.ReadAllText(GetCachedDataPath()));
        Logger.GlobalInstance.Info($"Loaded {Data?.entries?.Length ?? 0} entries locally from AUFiles API.");
        State = ProviderState.Initialized;
        return;
    }

    public override IEnumerator CoLoadFromCloud()
    {
        var plugin = PluginSingleton<HostUtilityPlugin>.Instance;
        plugin.Log.LogInfo("Began request to AUFiles API for player registry.");

        var request = UnityWebRequest.Get(
            "https://au.tntaddict.net/api/aufiles?paged=0&page=1&pageSize=600&sortBy=newest");
        yield return request.SendWebRequest();

        if (request.result != UnityWebRequest.Result.Success)
        {
            plugin.Log.LogError($"Failed to fetch AUFiles registry: {request.error}");
            State = ProviderState.Failed;
            yield break;
        }

        var json = request.downloadHandler.text;
        var list = JsonSerializer.Deserialize<List<AUFilesEntry>>(json);
        Data = new()
        {
            entries = list.ToArray(),
            page = -1,
            pageSize = -1,
            totalCount =  list.Count,
            totalPages = 1
        };
        plugin.Log.LogInfo($"Fetched {Data?.entries?.Length ?? 0} entries from AUFiles API.");
        File.WriteAllText(GetCachedDataPath(), JsonSerializer.Serialize(Data));
        plugin.Log.LogInfo($"Cached the AUFiles data in {GetCachedDataPath()}.");
        State = ProviderState.Initialized;
        LastFetchTime.Value = DateTime.Today.ToString(CultureInfo.CurrentCulture);
    }

    public override void Initialize()
    {
        State = ProviderState.Uninitialized;
        base.Initialize();
    }

    public override int GetSize()
    {
        return Data.totalCount;
    }

    public override bool IsTargetOnBanList(ClientData client, out string banReason)
    {
        banReason = "Suspected Pedophile / E-Dater";
        if (client.ProductUserId == "") return false;
        return Data.entries.Count(x => x.puid == client.ProductUserId) > 0;
    }
}