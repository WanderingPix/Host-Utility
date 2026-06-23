using System;

namespace HostUtility.AUFiles.API;

// ReSharper disable UnusedMember.Global
// ReSharper disable InconsistentNaming
[Serializable]
public class AUFilesEntry
{
    public string id { get; set; }
    public string friend_code { get; set; }
    public string puid { get; set; }
    public string pform_id { get; set; }
    public string pform_type { get; set; }
    public string banned { get; set; }
    public string created_at { get; set; }
}