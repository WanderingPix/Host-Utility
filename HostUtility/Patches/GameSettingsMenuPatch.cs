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
    public static List<GameObject> Menus = new();
    [HarmonyPatch(nameof(GameSettingMenu.Start))]
    [HarmonyPostfix]
    public static void GameSettingMenu_Start_Postfix(GameSettingMenu __instance)
    {
        //FixScroller(__instance, y);
        Menus = [
            __instance.GameSettingsTab.gameObject,
            __instance.PresetsTab.gameObject,
            __instance.RoleSettingsTab.gameObject
        ];
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
        prevBtn.gameObject.name = "PrevBtn";
        prevBtn.buttonText.transform.localPosition = new Vector3(0.025f, 0.05f, -5f);
        prevBtn.buttonText.alignment = TextAlignmentOptions.Center;

        var labelText = Object.Instantiate(__instance.MenuDescriptionText, leftPanel.transform);
        Object.Destroy(labelText.GetComponent<TextTranslatorTMP>());
        labelText.transform.localPosition = new Vector3(-3.35f, -2.3f, -5f);
        labelText.m_maxFontSize = 2;
        labelText.alignment = TextAlignmentOptions.Center;
        paginationBehaviour.text = labelText;

        var nextBtn = Object.Instantiate(prevBtn, leftPanel.transform);
        nextBtn.transform.localPosition = new Vector3(-2f, -2.2f, -5);
        nextBtn.buttonText.text = "▶";
        nextBtn.OnClick = new Button.ButtonClickedEvent();
        nextBtn.OnClick.AddListener(new Action(paginationBehaviour.NextPage));
        
        paginationBehaviour.pages.Add(firstPage);
        
        List<Vector3> buttonPositions = [ 
            __instance.GamePresetsButton.transform.position, 
            __instance.GameSettingsButton.transform.position,
            __instance.RoleSettingsButton.transform.position
        ];
        
        var menuBase = Object.Instantiate(__instance.GameSettingsTab, __instance.GameSettingsTab.transform.parent, true);
        for (int childIndex = 0; childIndex < menuBase.settingsContainer.childCount; childIndex++)
        {
            Object.Destroy(menuBase.transform.GetChild(childIndex).gameObject);
        }
        menuBase.enabled = false;
        menuBase.gameObject.SetActive(false);
        int i = 0;
        GameObject currentPage = null!;
        foreach (var tab in GameSettingsManager.Tabs)
        {
            if (i == 0)
            {
                currentPage?.gameObject.SetActive(false);
                currentPage = new GameObject("Page " + paginationBehaviour.pages.Count);
                currentPage.transform.SetParent(leftPanel.transform);
                paginationBehaviour.pages.Add(currentPage);
            }
            var button = Object.Instantiate(__instance.GamePresetsButton, currentPage.transform);
            Object.Destroy(button.buttonText.GetComponent<TextTranslatorTMP>());
            button.buttonText?.text = tab.Name;
            button.transform.position = buttonPositions[i];
            button.OnClick = new Button.ButtonClickedEvent();

            var menu = Object.Instantiate(menuBase, menuBase.transform.parent, true);
            menu.gameObject.SetActive(false);
            Object.Destroy(menu.MapPicker.gameObject);
            menu.Children = new();
            Menus.Add(menu.gameObject);
            tab.CreateSettings(menu, menu.settingsContainer, out float y);
            FixScroller(menu.scrollBar, y);
            
            button.OnClick.AddListener(new Action(() =>
            {
                foreach (var menu2 in Menus)
                {
                    menu2.SetActive(false);
                }
                menu.gameObject.SetActive(true);
            }));
            if (i >= 3)
            {
                i = 0;
            }
            i++;
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

    private static void FixScroller(Scroller scroller, float y)
    {
        scroller.SetYBoundsMax(- y - 1.65f);
    }
}