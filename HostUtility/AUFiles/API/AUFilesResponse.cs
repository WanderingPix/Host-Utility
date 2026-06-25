using System;

namespace HostUtility.AUFiles.API;

// ReSharper disable InconsistentNaming
[Serializable]
public class AUFilesResponse
{
    public AUFilesEntry[] entries { get; set; }
    public int totalCount { get; set; }
    public int page { get; set; }
    public int pageSize { get; set; }
    public int totalPages { get; set; }
}