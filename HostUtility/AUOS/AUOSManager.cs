using System;
using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Text.Json;
using HostUtility.AUOS.API;
using Reactor.Utilities;
using UnityEngine;
using UnityEngine.Networking;

namespace HostUtility.AUOS;

public static class AuosManager
{
    public static AUOSResponse Data;

    private static IEnumerator CoInitialize()
    {
        var plugin = PluginSingleton<HostUtilityPlugin>.Instance;
        plugin.Log.LogInfo("Began request to AUOS API for player registry.");

        var request = UnityWebRequest.Get("https://orftdpwzariqxnovqgtu.supabase.co/rest/v1/players");
        request.SetRequestHeader("apikey", "sb_publishable_bcpRUHL2FoycOrKGPJ2FUg_9v1gFsk2");
        request.SetRequestHeader("Accept", "application/json");
        yield return request.SendWebRequest();

        if (request.result != UnityWebRequest.Result.Success)
        {
            plugin.Log.LogError($"Failed to fetch AUOS registry: {request.error}");
            yield break;
        }

        var json = request.downloadHandler.text;
        var list = JsonSerializer.Deserialize<List<AUOSEntry>>(json);
        Data = new()
        {
            entries = list.ToArray()
        };
        plugin.Log.LogInfo($"Fetched {Data?.entries?.Length ?? 0} entries from AUOS API.");
        File.WriteAllText(GetDataPath(), JsonSerializer.Serialize(Data));
        plugin.Log.LogInfo($"Cached the AUOS data in {GetDataPath()}.");
        plugin.AuFilesLastFetchTime.Value = DateTime.Today.ToString(CultureInfo.CurrentCulture);
    }

    private static string GetDataPath()
    {
        return Path.Combine(
            (OperatingSystem.IsAndroid()
                ? Environment.GetEnvironmentVariable("STAR_DATA_PATH")
                : Application.persistentDataPath) ?? "", "auos.json");
    }
    public static void Initialize()
    {
        var plugin = PluginSingleton<HostUtilityPlugin>.Instance;
        var dataPath = GetDataPath();

        bool fetchedToday = plugin.AuOsLastFetchTime.Value != string.Empty
                            && DateTime.TryParse(plugin.AuOsLastFetchTime.Value, CultureInfo.CurrentCulture, DateTimeStyles.None, out var lastFetch)
                            && lastFetch.Date == DateTime.Today;

        if (fetchedToday && File.Exists(dataPath))
        {
            try
            {
                Data = JsonSerializer.Deserialize<AUOSResponse>(File.ReadAllText(dataPath));
                plugin.Log.LogInfo($"Loaded {Data?.entries?.Length ?? 0} entries locally from AUOS API.");
                return;
            }
            catch (Exception ex)
            {
                plugin.Log.LogWarning($"Cached AUOS data was unreadable, refetching: {ex.Message}");
            }
        }

        Coroutines.Start(CoInitialize());
    }
}