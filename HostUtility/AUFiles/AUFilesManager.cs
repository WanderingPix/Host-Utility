using System;
using System.Collections;
using System.Collections.Generic;
using System.Net.Http;
using System.Net.Http.Json;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.Threading.Tasks;
using HostUtility.AUFiles.API;
using Reactor.Utilities;
using UnityEngine;
using UnityEngine.Networking;

namespace HostUtility.AUFiles;

public static class AUFilesManager
{
    public static AUFilesResponse Data;

    public static IEnumerator Initialize()
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
        plugin.Log.LogInfo($"Loaded {Data?.entries?.Length ?? 0} entries from AUFiles API.");
    }
}