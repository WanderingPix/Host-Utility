using System;
using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Net.Http;
using System.Net.Http.Json;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.Threading.Tasks;
using BepInEx;
using HostUtility.AUFiles.API;
using Reactor.Utilities;
using UnityEngine;
using UnityEngine.Networking;

namespace HostUtility.AUFiles;

public static class AUFilesManager
{
    public static AUFilesResponse Data;

    private static IEnumerator CoInitialize()
    {
        var plugin = PluginSingleton<HostUtilityPlugin>.Instance;
        plugin.Log.LogInfo("Began request to AUFiles API for player registry.");

        var request = UnityWebRequest.Get(
            "https://au.tntaddict.net/api/aufiles?paged=0&page=1&pageSize=600&sortBy=newest");
        yield return request.SendWebRequest();

        if (request.result != UnityWebRequest.Result.Success)
        {
            plugin.Log.LogError($"Failed to fetch AUFiles registry: {request.error}");
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
        File.WriteAllText(GetDataPath(), JsonSerializer.Serialize(Data));
        plugin.Log.LogInfo($"Cached the AUFiles data in {GetDataPath()}.");
        plugin.LastFetchTime.Value = DateTime.Today.ToString(CultureInfo.CurrentCulture);
    }

    private static string GetDataPath()
    {
        return Path.Combine(
            (OperatingSystem.IsAndroid()
                ? Environment.GetEnvironmentVariable("STAR_DATA_PATH")
                : Application.persistentDataPath) ?? "", "aufiles.json");
    }
    public static void Initialize()
    {
        var plugin = PluginSingleton<HostUtilityPlugin>.Instance;
        var dataPath = GetDataPath();

        bool fetchedToday = plugin.LastFetchTime.Value != string.Empty
                            && DateTime.TryParse(plugin.LastFetchTime.Value, CultureInfo.CurrentCulture, DateTimeStyles.None, out var lastFetch)
                            && lastFetch.Date == DateTime.Today;

        if (fetchedToday && File.Exists(dataPath))
        {
            try
            {
                Data = JsonSerializer.Deserialize<AUFilesResponse>(File.ReadAllText(dataPath));
                plugin.Log.LogInfo($"Loaded {Data?.entries?.Length ?? 0} entries locally from AUFiles API.");
                return;
            }
            catch (Exception ex)
            {
                plugin.Log.LogWarning($"Cached AUFiles data was unreadable, refetching: {ex.Message}");
            }
        }

        Coroutines.Start(CoInitialize());
    }
}