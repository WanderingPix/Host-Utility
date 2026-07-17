using System;
using System.Collections.Generic;

namespace HostUtility.PlayerReporting;

public class PlayerReportData
{
    public string Name { get; set; }
    public string Puid { get; set; }
    public string FriendCode { get; set; }
    public List<string> MessageLogs { get; set; }
}