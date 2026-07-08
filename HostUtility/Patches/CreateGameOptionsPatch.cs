using AmongUs.GameOptions;
using HarmonyLib;

namespace HostUtility.Patches;

[HarmonyPatch(typeof(CreateGameOptions))]
public class CreateGameOptionsPatches
{
    [HarmonyPatch(nameof(CreateGameOptions.Start))]
    [HarmonyPostfix]
    public static void CreateGameOptions_Start_Postfix(CreateGameOptions __instance)
    {
        __instance.crossPlayButtons[0].transform.parent.gameObject.SetActive(true);
    }
}