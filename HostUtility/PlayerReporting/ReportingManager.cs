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
        var reporterData = new PlayerReportData
        {
            Name = PlayerControl.LocalPlayer.Data.PlayerName,
            Puid = PlayerControl.LocalPlayer.Data.Puid,
            FriendCode = PlayerControl.LocalPlayer.Data.FriendCode,
            MessageLogs = TrackingDataBehaviour.Local.chatMessages
        };
        var reportedData = new PlayerReportData
        {
            Name = target.Data.PlayerName,
            Puid = target.Data.Puid,
            FriendCode = target.Data.FriendCode,
            MessageLogs = target.gameObject.GetComponent<TrackingDataBehaviour>().chatMessages
        };
        Coroutines.Start(CoSendMessage(reporterData, reportedData));
    }
    private static IEnumerator CoSendMessage(PlayerReportData reporter, PlayerReportData reported)
    {
        string hookJson;
        try
        {
            string reporterMessageHistory = BuildMessageHistory(reporter.MessageLogs);
            string reportedMessageHistory = BuildMessageHistory(reported.MessageLogs);

            var hookRequest = new DiscordWebhookRequest()
            {
                embeds =
                [
                    new DiscordEmbed()
                    {
                        title = reporter.Name + " (Reporter)",
                        description = $"Name: {reporter.Name}\n" +
                                      $"Friend Code: {reporter.FriendCode}\n" +
                                      $"PUID: {reporter.Puid}\n" +
                                      $"Message Logs: \n{reporterMessageHistory}",
                        author = new EmbedAuthor() { name = "Player Reports" }
                    },
                    new DiscordEmbed()
                    {
                        title = reported.Name + " (Reported)",
                        description = $"Name: {reported.Name}\n" +
                                      $"Friend Code: {reported.FriendCode}\n" +
                                      $"PUID: {reported.Puid}\n" +
                                      $"Message Logs: \n{reportedMessageHistory}",
                        author = new EmbedAuthor() { name = "Player Reports" }
                    }
                ]
            };
            hookJson = JsonSerializer.Serialize(hookRequest);
        }
        catch (Exception ex)
        {
            Logger.GlobalInstance.Error($"Failed to build report payload: {ex}");
            yield break;
        }

        var bodyRaw = System.Text.Encoding.UTF8.GetBytes(hookJson);
        var request = new UnityWebRequest("CENSORED", "POST");
        request.uploadHandler = new UploadHandlerRaw(bodyRaw);
        request.downloadHandler = new DownloadHandlerBuffer();
        request.SetRequestHeader("Content-Type", "application/json");

        yield return request.SendWebRequest();

        if (request.result != UnityWebRequest.Result.Success)
            Logger.GlobalInstance.Error($"Discord webhook failed: {request.error} | Response: {request.downloadHandler.text}");
        else
            Logger.GlobalInstance.Info("Discord webhook sent successfully.");
    }

    private static string BuildMessageHistory(IEnumerable<string> messages)
    {
        if (messages == null) return "```\n(none)\n```";
        var sb = new StringBuilder("```\n");
        foreach (var m in messages) sb.Append($"    > {m}\n");
        sb.Append("```");
        return sb.ToString();
    }
    public static IEnumerator CoSendMessageTEST()
    {
        var hookRequest = new DiscordWebhookRequest()
        {
            content = "Hello from amog us"
        };

        var json = JsonSerializer.Serialize(hookRequest);
        var bodyRaw = System.Text.Encoding.UTF8.GetBytes(json);

        var request = new UnityWebRequest(
            "https://discord.com/api/webhooks/1527613882741297243/1MNN9ywfgnF7m2vVLY8LkeO-GvDgsaWcjuOgfW8roGYklT-ewNZt4nJMTZabHrWRwGgv",
            "POST");
        request.uploadHandler = new UploadHandlerRaw(bodyRaw);
        request.downloadHandler = new DownloadHandlerBuffer();
        request.SetRequestHeader("Content-Type", "application/json");

        yield return request.SendWebRequest();

        if (request.result != UnityWebRequest.Result.Success)
        {
            Logger.GlobalInstance.Error($"Discord webhook failed: {request.error} | Response: {request.downloadHandler.text}");
        }
        else
        {
            Logger.GlobalInstance.Info("Discord webhook sent successfully.");
        }
    }
}