using System.Collections.Generic;

namespace HostUtility.BanListAPI.Providers.MAUL.API;

public class MAULData
{
    public List<MAULEntry> entries { get; set; }
    public int totalCount => entries.Count;
}