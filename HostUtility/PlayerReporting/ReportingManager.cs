using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using HostUtility.AUFiles.API;
using UnityEngine.Networking;
using System.Text.Json;
using HostUtility.Components;
using Reactor.Utilities;

namespace HostUtility.PlayerReporting;

public class ReportingManager
{
    public static void ReportPlayer(PlayerControl target)
    {
        var reportData = new PlayerReportData
        {
            ReporterName = PlayerControl.LocalPlayer.Data.PlayerName,
            ReporterPuid = PlayerControl.LocalPlayer.Data.Puid,
            ReporterFriendCode = PlayerControl.LocalPlayer.Data.FriendCode,
            ReporterLogs = TrackingDataBehaviour.Local.chatMessages.ToArray(),
            ReporterPlatform = AmongUsClient.Instance.GetClientFromCharacter(PlayerControl.LocalPlayer).PlatformData.PlatformName,
            ReportedName = target.Data.PlayerName,
            ReportedPuid = target.Data.Puid,
            ReportedFriendCode = target.Data.FriendCode,
            ReportedLogs = target.gameObject.GetComponent<TrackingDataBehaviour>().chatMessages.ToArray(),
            ReportedPlatform = AmongUsClient.Instance.GetClientFromCharacter(target).PlatformData.PlatformName,
        };
        Coroutines.Start(CoSendMessage(reportData));
    }
    private static IEnumerator CoSendMessage(PlayerReportData report)
    {
        var json = JsonSerializer.Serialize(report);
        var bodyRaw = Encoding.UTF8.GetBytes(json);

        var request = new UnityWebRequest("https://orftdpwzariqxnovqgtu.supabase.co/functions/v1/report-function-unfinished", "POST");
        request.uploadHandler = new UploadHandlerRaw(bodyRaw);
        request.downloadHandler = new DownloadHandlerBuffer();

        request.SetRequestHeader("apikey", "sb_publishable_bcpRUHL2FoycOrKGPJ2FUg_9v1gFsk2");
        request.SetRequestHeader("Content-Type", "application/json");
        request.SetRequestHeader("Accept", "application/json");
        request.SetRequestHeader("among-us-client-secret", "Ja1njfzioxr7GkiKl8XIXxHSbIspjeLipvzSyAYfzIc0VGAoeRMi4stnro3GArbw");
        yield return request.SendWebRequest();

        if (request.result == UnityWebRequest.Result.Success)
        {
            Logger.GlobalInstance.Info($"Report submitted successfully: {request.downloadHandler.text}");
        }
        else
        {
            Logger.GlobalInstance.Error($"Report submission failed: {request.error} — {request.downloadHandler.text}");
        }

        request.Dispose();
    }
}