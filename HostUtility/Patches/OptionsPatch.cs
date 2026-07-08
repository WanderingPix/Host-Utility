using AmongUs.GameOptions;
using HarmonyLib;

namespace HostUtility.Patches;

[HarmonyPatch(typeof(NumberOption))]
public class OptionsPatch
{
    [HarmonyPatch(nameof(NumberOption.Start))]
    [HarmonyPatch(nameof(NumberOption.Initialize))]
    [HarmonyPostfix]
    public static void NumberOption_Initialize_Postfix(NumberOption __instance)
    {
        __instance.StartCoroutine(Effects.ActionAfterDelay(0.1f, new System.Action(() =>
        {
            if (__instance.intOptionName is Int32OptionNames.Invalid) return;
            if (__instance.intOptionName is Int32OptionNames.MaxImpostors or Int32OptionNames.NumImpostors) return;
            __instance.ValidRange = new FloatRange(0, float.MaxValue);
        })));
    }
}