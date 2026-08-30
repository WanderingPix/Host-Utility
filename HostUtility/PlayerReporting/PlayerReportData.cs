using System;
using System.Text.Json.Serialization;

namespace HostUtility.PlayerReporting;

[Serializable]
public class PlayerReportData
{
    [JsonPropertyName("reporterName")]
    public string ReporterName { get; set; }

    [JsonPropertyName("reporterFriendCode")]
    public string ReporterFriendCode { get; set; }

    [JsonPropertyName("reporterPuid")]
    public string ReporterPuid { get; set; }

    [JsonPropertyName("reporterLogs")]
    public string[] ReporterLogs { get; set; }
    
    [JsonPropertyName("reporterPlatform")]
    public string ReporterPlatform { get; set; }

    [JsonPropertyName("reportedName")]
    public string ReportedName { get; set; }

    [JsonPropertyName("reportedFriendCode")]
    public string ReportedFriendCode { get; set; }

    [JsonPropertyName("reportedPuid")]
    public string ReportedPuid { get; set; }

    [JsonPropertyName("reportedLogs")]
    public string[] ReportedLogs { get; set; }
    
    [JsonPropertyName("reportedPlatform")]
    public string ReportedPlatform { get; set; }
}