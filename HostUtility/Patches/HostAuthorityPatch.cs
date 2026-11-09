using HarmonyLib;

namespace HostUtility.Patches;

[HarmonyPatch]
public class HostAuthorityPatch
{
    [HarmonyPatch(typeof(Constants), nameof(Constants.GetBroadcastVersion))]
    internal static class ModdedProtocolPatch
    {
        public static void Postfix(ref int __result)
        {
            if (AmongUsClient.Instance.NetworkMode == NetworkModes.OnlineGame && HostUtilityPlugin.Instance.UseHostAuthority.Value)
            {
                var revision = __result % 50;
                if (revision < 25)
                {
                    __result += 25;
                }
            }
        }
    }
}