using System.Collections.Generic;
using System.Linq;
using AmongUs.InnerNet.GameDataMessages;
using HarmonyLib;
using Hazel;
using HostUtility.AUFiles;
using HostUtility.BanListAPI;
using HostUtility.BanListAPI.Providers.AUFiles;
using HostUtility.Components;
using Reactor.Utilities;

namespace HostUtility.Patches;

[HarmonyPatch]
public class PlayerControlPatches
{
    [HarmonyPatch(typeof(PlayerControl), nameof(PlayerControl.Start))]
    [HarmonyPostfix]
    public static void PlayerControl_Start_Postfix(PlayerControl __instance)
    {
        __instance.gameObject.AddComponent<TrackingDataBehaviour>().myPlayer = __instance;
        if (!AmongUsClient.Instance.AmHost) return;
        var plugin = PluginSingleton<HostUtilityPlugin>.Instance;
        if (__instance == PlayerControl.LocalPlayer) return;
        __instance.StartCoroutine(Effects.ActionAfterDelay(1f, new System.Action(() =>
        {
            if (__instance == PlayerControl.LocalPlayer) return;
            if (plugin.ShowPlayerPlatforms.Value)
            {
                var platformName = AmongUsClient.Instance.GetClientFromCharacter(__instance).PlatformData.PlatformName;
                if (platformName == "112") platformName = "Starlight Mobile";
                if (platformName == "TESTNAME") platformName = "Unknown";
                __instance.cosmetics.nameText.text += $" ({platformName})";
            }
            if (plugin.ShowPlayerIDs.Value) __instance.cosmetics.nameText.text += $" (ID: {__instance.PlayerId})";
            
            if (BanWords.ContainsSwear(__instance.Data.PlayerName) && plugin.BanInappropriateNames.Value) AmongUsClient.Instance.KickWithReason(__instance.Data.ClientId, "Inappropriate username", "",true);
            if (BotNames.Names.Contains(__instance.Data.PlayerName) && plugin.BanInappropriateNames.Value) AmongUsClient.Instance.KickWithReason(__instance.Data.ClientId, "Bot Player", "",true);
            if (RemoveInvisibleCharacters(__instance.Data.PlayerName).Trim() == string.Empty) __instance.RpcSetName("Unknown Player");
            if (__instance.Data.PlayerLevel < plugin.MinLevel.Value) AmongUsClient.Instance.KickWithReason(__instance.Data.ClientId, "Low level", "",false);
            if (BanListManager.IsTargetOnBanList(AmongUsClient.Instance.GetClientFromCharacter(__instance), out string banReason, out string banListName)) AmongUsClient.Instance.KickWithReason(__instance.Data.ClientId, banReason, banListName, false);
            if (FriendsListManager.Instance.IsPlayerBlocked(AmongUsClient.Instance.GetClient(__instance.Data.ClientId).ProductUserId)) AmongUsClient.Instance.KickWithReason(__instance.Data.ClientId, "Blocked player", "", false);
        })));
    }

    public static readonly List<string> InvisibleCharacters = new List<string>
    {
        "\u0000", // NULL
        "\u0001", // START OF HEADING
        "\u0002", // START OF TEXT
        "\u0003", // END OF TEXT
        "\u0004", // END OF TRANSMISSION
        "\u0005", // ENQUIRY
        "\u0006", // ACKNOWLEDGE
        "\u0007", // BELL
        "\u0008", // BACKSPACE
        "\u000B", // LINE TABULATION (VERTICAL TAB)
        "\u000C", // FORM FEED
        "\u000E", // SHIFT OUT
        "\u000F", // SHIFT IN
        "\u0010", // DATA LINK ESCAPE
        "\u0011", // DEVICE CONTROL ONE
        "\u0012", // DEVICE CONTROL TWO
        "\u0013", // DEVICE CONTROL THREE
        "\u0014", // DEVICE CONTROL FOUR
        "\u0015", // NEGATIVE ACKNOWLEDGE
        "\u0016", // SYNCHRONOUS IDLE
        "\u0017", // END OF TRANSMISSION BLOCK
        "\u0018", // CANCEL
        "\u0019", // END OF MEDIUM
        "\u001A", // SUBSTITUTE
        "\u001B", // ESCAPE
        "\u001C", // FILE SEPARATOR
        "\u001D", // GROUP SEPARATOR
        "\u001E", // RECORD SEPARATOR
        "\u001F", // UNIT SEPARATOR
        "\u007F", // DELETE
        "\u00A0", // NO-BREAK SPACE
        "\u00AD", // SOFT HYPHEN
        "\u034F", // COMBINING GRAPHEME JOINER
        "\u061C", // ARABIC LETTER MARK
        "\u115F", // HANGUL CHOSEONG FILLER
        "\u1160", // HANGUL JUNGSEONG FILLER
        "\u17B4", // KHMER VOWEL INHERENT AQ
        "\u17B5", // KHMER VOWEL INHERENT AA
        "\u180B", // MONGOLIAN FREE VARIATION SELECTOR ONE
        "\u180C", // MONGOLIAN FREE VARIATION SELECTOR TWO
        "\u180D", // MONGOLIAN FREE VARIATION SELECTOR THREE
        "\u180E", // MONGOLIAN VOWEL SEPARATOR
        "\u200B", // ZERO WIDTH SPACE
        "\u200C", // ZERO WIDTH NON-JOINER
        "\u200D", // ZERO WIDTH JOINER
        "\u200E", // LEFT-TO-RIGHT MARK
        "\u200F", // RIGHT-TO-LEFT MARK
        "\u202A", // LEFT-TO-RIGHT EMBEDDING
        "\u202B", // RIGHT-TO-LEFT EMBEDDING
        "\u202C", // POP DIRECTIONAL FORMATTING
        "\u202D", // LEFT-TO-RIGHT OVERRIDE
        "\u202E", // RIGHT-TO-LEFT OVERRIDE
        "\u202F", // NARROW NO-BREAK SPACE
        "\u2060", // WORD JOINER
        "\u2061", // FUNCTION APPLICATION
        "\u2062", // INVISIBLE TIMES
        "\u2063", // INVISIBLE SEPARATOR
        "\u2064", // INVISIBLE PLUS
        "\u2066", // LEFT-TO-RIGHT ISOLATE
        "\u2067", // RIGHT-TO-LEFT ISOLATE
        "\u2068", // FIRST STRONG ISOLATE
        "\u2069", // POP DIRECTIONAL ISOLATE
        "\u206A", // INHIBIT SYMMETRIC SWAPPING
        "\u206B", // ACTIVATE SYMMETRIC SWAPPING
        "\u206C", // INHIBIT ARABIC FORM SHAPING
        "\u206D", // ACTIVATE ARABIC FORM SHAPING
        "\u206E", // NATIONAL DIGIT SHAPES
        "\u206F", // NOMINAL DIGIT SHAPES
        "\u3164", // HANGUL FILLER
        "\uFEFF", // ZERO WIDTH NO-BREAK SPACE / BOM
        "\uFFA0", // HALFWIDTH HANGUL FILLER
        "\uFFF9", // INTERLINEAR ANNOTATION ANCHOR
        "\uFFFA", // INTERLINEAR ANNOTATION SEPARATOR
        "\uFFFB", // INTERLINEAR ANNOTATION TERMINATOR
    };
    private static string RemoveInvisibleCharacters(string s)
    {
        string result = s;
        foreach (var c in InvisibleCharacters)
        {
            result = result.Replace(c, string.Empty);
        }

        return result;
    }
}