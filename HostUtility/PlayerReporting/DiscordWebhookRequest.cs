using System;
using System.Collections.Generic;

namespace HostUtility.PlayerReporting;

[Serializable]
public class DiscordWebhookRequest
{ 
    public string content { get; set; }
    public string username { get; set; }
    public string avatar_url { get; set; }
    public bool tts { get; set; }
    public List<DiscordEmbed> embeds { get; set; }
    public int? flags { get; set; }
}
[Serializable]
public class DiscordEmbed
{
    public string title { get; set; }
    public string type { get; set; } = "rich";
    public string description { get; set; }
    public string url { get; set; }
    public string timestamp { get; set; } // ISO8601, e.g. DateTime.UtcNow.ToString("o")
    public int? color { get; set; } // decimal color value, e.g. 0x00FF00 = 65280
    public EmbedFooter footer { get; set; }
    public EmbedImage image { get; set; }
    public EmbedImage thumbnail { get; set; }
    public EmbedAuthor author { get; set; }
    public List<EmbedField> fields { get; set; }
}
[Serializable]
public class EmbedFooter
{
    public string text { get; set; }
    public string icon_url { get; set; }
}
[Serializable]
public class EmbedImage
{
    public string url { get; set; }
}
[Serializable]
public class EmbedAuthor
{
    public string name { get; set; }
    public string url { get; set; }
    public string icon_url { get; set; }
}
[Serializable]
public class EmbedField
{
    public string name { get; set; }
    public string value { get; set; }
    public bool inline { get; set; }
}