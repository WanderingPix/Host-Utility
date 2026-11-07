using System;
using System.Collections.Generic;
using System.Linq;
using AmongUs.GameOptions;
using BepInEx.Configuration;
using HarmonyLib;
using HostUtility.BanListAPI;
using HostUtility.Components;
using HostUtility.Components.UI;
using HostUtility.GameSettings;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using Object = UnityEngine.Object;

namespace HostUtility.Patches;

[HarmonyPatch(typeof(GameSettingMenu))]
public class GameSettingsMenuPatch
{
    [HarmonyPatch(nameof(GameSettingMenu.Start))]
    [HarmonyPostfix]
    public static void GameSettingMenu_Start_Postfix(GameSettingMenu __instance)
    {
        //var plugin = HostUtilityPlugin.Instance;
        //float y = GetLowestActiveY(__instance);
        //
        //CreateHeader(__instance, "Anti Cheat", ref y);
        //CreateToggle(__instance, "Check for message cooldowns", plugin.CheckMessageCooldowns.Value, b => plugin.CheckMessageCooldowns.Value = b, ref y);
        //
        //CreateHeader(__instance, "Moderation", ref y);
        //CreateToggle(__instance, "Ban Inappropriate Names", plugin.BanInappropriateNames.Value, b => plugin.BanInappropriateNames.Value = b, ref y);
        //CreateToggle(__instance, "Ban Inappropriate Messages", plugin.BanInappropriateMessages.Value,
        //    b => plugin.BanInappropriateMessages.Value = b, ref y);
        //CreateToggle(__instance, "Kick Suspected Bots", plugin.KickSuspectedBots.Value, b => plugin.KickSuspectedBots.Value = b, ref y);
        //
        //CreateHeader(__instance, "Join Conditions", ref y);
        //CreateInt(__instance, "Minimum Level", plugin.MinLevel.Value, 5, new FloatRange(0, 100), i => plugin.MinLevel.Value = (int)i, ref y);
        //
        //CreateHeader(__instance, "Ban Lists", ref y);
        //foreach (var banList in BanListManager.Providers)
        //{
        //    CreateHeader(__instance, $"{banList.Name}\n<size=75%>({banList.Owner})</size>", ref y, true);
        //    CreateToggle(__instance, "Active", banList.IsEnabled.Value,  b => banList.IsEnabled.Value = b, ref y,true);
        //    foreach (var configEntryBase in banList.Settings)
        //    {
        //        if (configEntryBase is ConfigEntry<bool> boolEntry)
        //        {
        //            CreateToggle(__instance, boolEntry.Definition.Key, boolEntry.Value, b => boolEntry.Value = b, ref y, true);
        //        }
        //        
        //        else if (configEntryBase is ConfigEntry<float> floatEntry)
        //        {
        //            CreateInt(__instance, floatEntry.Definition.Key, (int)floatEntry.Value, 5, new FloatRange(0, 100), v => floatEntry.Value = v, ref y, true);
        //        }
        //    }
        //}
        //
        //CreateHeader(__instance, "Miscellaneous", ref y);
        //CreateInt(__instance, "Game Start Countdown", plugin.GameStartCountdownTime.Value, 1, new FloatRange(0, 10), i => plugin.GameStartCountdownTime.Value = (int)i, ref y);
        //CreateToggle(__instance, "Disable R1 Button presses", plugin.DisableR1Meetings.Value, b => plugin.DisableR1Meetings.Value = b, ref y);
        //
        //CreateHeader(__instance, "Advanced", ref y);
        //CreateToggle(__instance, "Show Player Platforms", plugin.ShowPlayerPlatforms.Value, b =>
        //{
        //    plugin.ShowPlayerPlatforms.Value = b;
        //    foreach (var icon in PlayerIconsBehaviour.AllIcons)
        //    {
        //        icon.PlatformIcon.SetActive(b);
        //    }
        //}, ref y);
        //CreateToggle(__instance, "Show Player IDs", plugin.ShowPlayerIDs.Value, b =>
        //{
        //    plugin.ShowPlayerIDs.Value = b;
        //    foreach (var icon in PlayerIconsBehaviour.AllIcons)
        //    {
        //        icon.IdIcon.SetActive(b);
        //    }
        //}, ref y);
        //
        //CreateToggle(__instance, "Show Player Levels", plugin.ShowPlayerLevels.Value, b =>
        //{
        //    plugin.ShowPlayerLevels.Value = b;
        //    foreach (var icon in PlayerIconsBehaviour.AllIcons)
        //    {
        //        icon.LevelIcon.SetActive(b);
        //    }
        //}, ref y);
        //
        //var action = new Action<bool>(b =>
        //{
        //    plugin.GameEndEnabled.Value = b;
        //    GameManager.Instance.gameObject.SetActive(b);
        //    if (!b)
        //        HudManager.Instance.Notifier.AddDisconnectMessage(
        //            "<b>Game End has been DISABLED</b>, the game will not end normally.");
        //});
        //action.Invoke(plugin.GameEndEnabled.Value);
        //CreateToggle(__instance, "<color=red>Game End Behaviour</color>", plugin.GameEndEnabled.Value, action, ref y);
        //
        //FixScroller(__instance, y);
        
        var leftPanel = __instance.transform.Find("LeftPanel").gameObject;
        var firstPage = new GameObject("Page 1");
        firstPage.transform.parent = leftPanel.transform;
        firstPage.transform.localPosition = Vector3.zero;
        __instance.GamePresetsButton.transform.SetParent(firstPage.transform, true);
        __instance.GameSettingsButton.transform.SetParent(firstPage.transform, true);
        __instance.RoleSettingsButton.transform.SetParent(firstPage.transform, true);

        var paginationBehaviour = leftPanel.AddComponent<PaginationBehaviour>();
        var prevBtn = Object.Instantiate(__instance.GamePresetsButton, paginationBehaviour.transform);
        prevBtn.transform.localPosition = new Vector3(-3.775f, -2.2f, -5);
        prevBtn.transform.localScale = Vector3.one;
        prevBtn.OnClick = new Button.ButtonClickedEvent();
        prevBtn.OnClick.AddListener(new Action(paginationBehaviour.NextPage));
        prevBtn.activeSprites.GetComponent<SpriteRenderer>().size = prevBtn.inactiveSprites.GetComponent<SpriteRenderer>().size = prevBtn.GetComponent<BoxCollider2D>().size = new Vector2(1f, 0.35f);
        Object.Destroy(prevBtn.buttonText.GetComponent<TextTranslatorTMP>());
        prevBtn.buttonText.text = "◀";
        prevBtn.buttonText.m_maxFontSize = 2f;
        prevBtn.buttonText.transform.localPosition = new Vector3(0.025f, 0.05f, -5f);
        prevBtn.buttonText.alignment = TextAlignmentOptions.Center;

        var labelText = Object.Instantiate(__instance.MenuDescriptionText, leftPanel.transform);
        labelText.transform.localPosition = new Vector3(-3.35f, -2.3f, -5f);
        labelText.m_maxFontSize = 2;
        paginationBehaviour.text = labelText;

        var nextBtn = Object.Instantiate(prevBtn, leftPanel.transform);
        nextBtn.transform.position = new Vector3(-2f, -2.2f, -5);
        nextBtn.buttonText.text = "▶";
        nextBtn.OnClick = new Button.ButtonClickedEvent();
        nextBtn.OnClick.AddListener(new Action(paginationBehaviour.NextPage));
        
        paginationBehaviour.pages.Add(firstPage);
        
        List<Vector3> buttonPositions = [ 
            __instance.GamePresetsButton.transform.position, 
            __instance.GameSettingsButton.transform.position,
            __instance.RoleSettingsButton.transform.position
        ];

        int i = 0;
        GameObject currentPage = null!;
        foreach (var tab in GameSettingsManager.Tabs)
        {
            if (i == 0)
            {
                currentPage = new GameObject("Page " + paginationBehaviour.pages.Count);
                currentPage.transform.SetParent(leftPanel.transform);
                paginationBehaviour.pages.Add(currentPage);
            }
            var button = Object.Instantiate(__instance.GamePresetsButton, currentPage.transform);
            button.buttonText?.text = tab.Name;
            button.transform.position = buttonPositions[i];
            //TODO Color and Name, and click behaviour
            i++;
            if (i <= 3)
            {
                i = 0;
            }
        }
    }

    private static float GetLowestActiveY(GameOptionsMenu __instance)
    {
        var active = __instance.Children
            .ToArray().Where(c => c != null && c.gameObject.activeSelf)
            .ToArray();

        if (active.Length == 0)
        {
            return __instance.Children.ToArray().Last().transform.localPosition.y;
        }

        return active.Min(c => c.transform.localPosition.y);
    }

    private static void CreateToggle(GameOptionsMenu __instance, string title, bool defaultValue, Action<bool> callback, ref float y, bool isSubcategory = false)
    {
        y -= 0.45f;
        var optionBehaviour1 = UnityEngine.Object.Instantiate<ToggleOption>(__instance.checkboxOrigin, Vector3.zero, Quaternion.identity, __instance.settingsContainer);
        optionBehaviour1.transform.localPosition = new Vector3(!isSubcategory ? 0.952f : 1.2f, y, -2f);
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
        Action<float> callback, ref float y, bool isSubcategory = false)
    {
        y -= 0.45f;
        var optionBehaviour1 = UnityEngine.Object.Instantiate(__instance.numberOptionOrigin, Vector3.zero, Quaternion.identity, __instance.settingsContainer);
        optionBehaviour1.transform.localPosition = new Vector3(!isSubcategory ? 0.952f : 1.2f, y, -2f);
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

    private static void CreateHeader(GameOptionsMenu __instance, string title, ref float y, bool isSubcategory = false)
    {
        y -= 0.63f;
        CategoryHeaderMasked categoryHeaderMasked = UnityEngine.Object.Instantiate<CategoryHeaderMasked>(__instance.categoryHeaderOrigin, Vector3.zero, Quaternion.identity, __instance.settingsContainer);
        categoryHeaderMasked.Title.text = title;
        categoryHeaderMasked.transform.localScale = Vector3.one * 0.63f;
        categoryHeaderMasked.transform.localPosition = new Vector3(!isSubcategory ? -0.903f : -0.6f, y, -2f);
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