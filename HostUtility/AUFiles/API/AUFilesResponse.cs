using System;

namespace HostUtility.AUFiles.API;

// ReSharper disable InconsistentNaming
[Serializable]
public class AUFilesResponse
{
    public AUFilesEntry[] entries;
    public int totalCount;
    public int page;
    public int pageSize;
    public int totalPages;
}