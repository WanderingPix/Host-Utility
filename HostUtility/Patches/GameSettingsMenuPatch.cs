using System;
using System.Linq;
using AmongUs.GameOptions;
using HarmonyLib;
using Reactor.Utilities;
using TMPro;
using UnityEngine;

namespace HostUtility.Patches;

[HarmonyPatch(typeof(GameOptionsMenu))]
public class GameSettingsMenuPatch
{
    [HarmonyPatch(nameof(GameOptionsMenu.CreateSettings))]
    [HarmonyPostfix]
    public static void GameSettingMenu_Start_Postfix(GameOptionsMenu __instance)
    {
        var plugin = PluginSingleton<HostUtilityPlugin>.Instance;
        float y = GetLowestActiveY(__instance);
        
        //CreateHeader(__instance, "Lobby", ref y);
        //CreateInt(__instance, "Max Player Count", GameOptionsManager.Instance.CurrentGameOptions.MaxPlayers, 1, new FloatRange(4, 15), v => GameOptionsManager.Instance.CurrentGameOptions.SetInt(Int32OptionNames.MaxPlayers, (int)v), ref y);
        CreateHeader(__instance, "Moderation", ref y);
        CreateToggle(__instance, "Ban Inappropriate Names", plugin.BanInappropriateNames.Value, b => plugin.BanInappropriateNames.Value = b, ref y);
        CreateToggle(__instance, "Ban Inappropriate Messages", plugin.BanInappropriateMessages.Value, b => plugin.BanInappropriateMessages.Value = b, ref y);
        CreateToggle(__instance, "Kick Suspected E-Daters and PDFs", plugin.KickSuspectedPlayers.Value, b => plugin.KickSuspectedPlayers.Value = b, ref y);

        CreateHeader(__instance, "Join Conditions", ref y);
        CreateInt(__instance, "Minimum Level", plugin.MinLevel.Value, 5, new FloatRange(0, 100), i => plugin.MinLevel.Value = (int)i, ref y);

        CreateHeader(__instance, "Miscellaneous", ref y);
        CreateInt(__instance, "Game Start Countdown", plugin.GameStartCountdownTime.Value, 1, new FloatRange(0, 10), i => plugin.GameStartCountdownTime.Value = (int)i, ref y);

        FixScroller(__instance, y);
    }

    private static float GetLowestActiveY(GameOptionsMenu __instance)
    {
        var active = __instance.Children
            .ToArray().Where(c => c != null && c.gameObject.activeSelf)
            .ToArray();

        if (active.Length == 0)
        {
            // Fallback: nothing active, don't crash — use whatever is last.
            return __instance.Children.ToArray().Last().transform.localPosition.y;
        }

        return active.Min(c => c.transform.localPosition.y);
    }

    private static void CreateToggle(GameOptionsMenu __instance, string title, bool defaultValue, Action<bool> callback, ref float y)
    {
        y -= 0.45f;
        var optionBehaviour1 = UnityEngine.Object.Instantiate<ToggleOption>(__instance.checkboxOrigin, Vector3.zero, Quaternion.identity, __instance.settingsContainer);
        optionBehaviour1.transform.localPosition = new Vector3(0.952f, y, -2f);
        optionBehaviour1.SetClickMask(__instance.ButtonClickMask);
        foreach (var rend in optionBehaviour1.GetComponentsInChildren<SpriteRenderer>(true))
        {
            rend.color = ThemeColor;
            rend.maskInteraction = SpriteMaskInteraction.VisibleInsideMask;
        }

        optionBehaviour1.CheckMark.color = Color.white;
        
        optionBehaviour1.StartCoroutine(Effects.ActionAfterDelay(0.05f, new System.Action(() => optionBehaviour1.TitleText.text = title)));
        optionBehaviour1.CheckMark.enabled = defaultValue;
        optionBehaviour1.buttons[0].OnClick = new();
        optionBehaviour1.buttons[0].OnClick.AddListener(new System.Action(() =>
        {
            optionBehaviour1.CheckMark.enabled = !optionBehaviour1.CheckMark.enabled;
            callback.Invoke(optionBehaviour1.CheckMark.enabled);
        }));
        __instance.Children.Add(optionBehaviour1);
    }

    private static readonly Color ThemeColor = Color.Lerp(Color.white, new Color(0.1f, 0.1f, 0.5f), 0.4f);
    private static void CreateInt(GameOptionsMenu __instance, string title, int defaultValue, float increment, FloatRange range,
        Action<float> callback, ref float y)
    {
        y -= 0.45f;
        var optionBehaviour1 = UnityEngine.Object.Instantiate(__instance.numberOptionOrigin, Vector3.zero, Quaternion.identity, __instance.settingsContainer);
        optionBehaviour1.transform.localPosition = new Vector3(0.952f, y, -2f);
        optionBehaviour1.SetClickMask(__instance.ButtonClickMask);
        foreach (var rend in optionBehaviour1.GetComponentsInChildren<SpriteRenderer>(true))
        {
            rend.color = ThemeColor;
            rend.maskInteraction = SpriteMaskInteraction.VisibleInsideMask;
        }
        optionBehaviour1.StartCoroutine(Effects.ActionAfterDelay(0.05f, new System.Action(() => optionBehaviour1.TitleText.text = title)));
        optionBehaviour1.ValidRange = range;
        optionBehaviour1.Value = defaultValue;
        optionBehaviour1.ValueText.text = optionBehaviour1.Value.ToString();
        optionBehaviour1.Increment = increment;
        optionBehaviour1.intOptionName = Int32OptionNames.Invalid;
        optionBehaviour1.floatOptionName = FloatOptionNames.Invalid;
        
        optionBehaviour1.PlusBtn.interactableColor = ThemeColor;
        optionBehaviour1.StartCoroutine(Effects.ActionAfterDelay(0.05f, new System.Action(() =>
        {
            optionBehaviour1.MinusBtn.SetInteractable(true);
            optionBehaviour1.PlusBtn.SetInteractable(true);
        })));
        optionBehaviour1.MinusBtn.interactableColor = ThemeColor;
        optionBehaviour1.buttons[0].OnClick = new();
        optionBehaviour1.buttons[0].OnClick.AddListener(new System.Action(() =>
        {
            optionBehaviour1.Value -= optionBehaviour1.Increment;
            optionBehaviour1.Value = Mathf.Clamp(optionBehaviour1.Value, optionBehaviour1.ValidRange.min, optionBehaviour1.ValidRange.max);
            optionBehaviour1.ValueText.text = optionBehaviour1.Value.ToString();
            callback.Invoke(optionBehaviour1.Value);
            optionBehaviour1.PlusBtn.SetInteractable(true);
            optionBehaviour1.MinusBtn.SetInteractable(true);
        }));
        optionBehaviour1.buttons[1].OnClick = new();
        optionBehaviour1.buttons[1].OnClick.AddListener(new System.Action(() =>
        {
            optionBehaviour1.Value += optionBehaviour1.Increment;
            optionBehaviour1.Value = Mathf.Clamp(optionBehaviour1.Value, optionBehaviour1.ValidRange.min, optionBehaviour1.ValidRange.max);
            optionBehaviour1.ValueText.text = optionBehaviour1.Value.ToString();
            callback.Invoke(optionBehaviour1.Value);
        }));
        __instance.Children.Add(optionBehaviour1);
    }

    private static void CreateHeader(GameOptionsMenu __instance, string title, ref float y)
    {
        y -= 0.63f;
        CategoryHeaderMasked categoryHeaderMasked = UnityEngine.Object.Instantiate<CategoryHeaderMasked>(__instance.categoryHeaderOrigin, Vector3.zero, Quaternion.identity, __instance.settingsContainer);
        categoryHeaderMasked.Title.text = title;
        categoryHeaderMasked.transform.localScale = Vector3.one * 0.63f;
        categoryHeaderMasked.transform.localPosition = new Vector3(-0.903f, y, -2f);
        int maskLayer = 20;
        categoryHeaderMasked.Background.color = Color.Lerp(categoryHeaderMasked.Background.color, new Color(0.1f, 0.1f, 0.5f), 0.4f);
        categoryHeaderMasked.Background.material.SetInt(PlayerMaterial.MaskLayer, maskLayer);
        if (categoryHeaderMasked.Divider != null)
            categoryHeaderMasked.Divider.material.SetInt(PlayerMaterial.MaskLayer, maskLayer);
        categoryHeaderMasked.Title.fontMaterial.SetFloat("_StencilComp", 3f);
        categoryHeaderMasked.Title.fontMaterial.SetFloat("_Stencil", (float) maskLayer);
        y -= 0.15f;
    }

    private static void FixScroller(GameOptionsMenu __instance, float y)
    {
        __instance.scrollBar.SetYBoundsMax((float) (-(double) y - 1.649999976158142));
    }
}