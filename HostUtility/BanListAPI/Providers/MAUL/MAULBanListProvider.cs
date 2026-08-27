using System;
using System.Collections;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text.Json;
using HarmonyLib;
using HostUtility.BanListAPI.Providers.AUFiles.API;
using HostUtility.BanListAPI.Providers.MAUL.API;
using InnerNet;
using Reactor.Utilities;
using UnityEngine.Networking;

namespace HostUtility.BanListAPI.Providers.MAUL;

// ReSharper disable once InconsistentNaming
public class MAULBanListProvider : BanListProvider
{
    public override string Name => "MAUL";
    public override string Owner => "The MAUL Server Staff";
    public MAULData Data { get; set; }
    public override void LoadFromCache()
    {
        Data = JsonSerializer.Deserialize<MAULData>(File.ReadAllText(GetCachedDataPath()));
        Logger.GlobalInstance.Info($"Loaded {Data?.entries?.Count ?? 0} entries locally from MAUL BanList.");
        State = ProviderState.Initialized;
    }

    public override IEnumerator CoLoadFromCloud()
    {
        var plugin = PluginSingleton<HostUtilityPlugin>.Instance;
        plugin.Log.LogInfo("Began request to github for MAUL player banlist.");

        var request = UnityWebRequest.Get(
            "https://raw.githubusercontent.com/Sarhadactyl/MAULweb/refs/heads/main/BanList.txt");
        yield return request.SendWebRequest();

        if (request.result != UnityWebRequest.Result.Success)
        {
            plugin.Log.LogError($"Failed to fetch MAUL banlist: {request.error}");
            State = ProviderState.Failed;
            yield break;
        }

        Data = new();
        Data.entries = [];
        var text = request.downloadHandler.text;
        foreach (var line in text.Split("\n"))
        {
            var splits = line.Split(',');
            var entry = new MAULEntry()
            {
                FriendCode = splits[0],
                Reason = splits[2],
            };
            Data.entries.Add(entry);
        }
        plugin.Log.LogInfo($"Fetched {Data?.entries?.Count ?? 0} entries from MAUL's BanList.");
        File.WriteAllText(GetCachedDataPath(), JsonSerializer.Serialize(Data));
        plugin.Log.LogInfo($"Cached the MAUL data in {GetCachedDataPath()}.");
        State = ProviderState.Initialized;
        LastFetchTime.Value = DateTime.Today.ToString(CultureInfo.CurrentCulture);
    }

    public override int GetSize()
    {
        return Data.totalCount;
    }

    public override bool IsTargetOnBanList(ClientData client, out string banReason)
    {
        if (Data.entries.Count(x => x.FriendCode == client.FriendCode) > 0)
        {
            banReason = string.Empty;
            return false;
        }

        var entry = Data.entries.First(x => x.FriendCode == client.FriendCode);
        banReason = entry.Reason;
        return true;
    }
}