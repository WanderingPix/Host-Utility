using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using BepInEx.Unity.IL2CPP.Utils.Collections;
using HostUtility.Components.UI;
using HostUtility.GameSettings.OptionTypes;
using Il2CppInterop.Runtime.InteropTypes.Arrays;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using Object = UnityEngine.Object;

namespace HostUtility.GameSettings;

public class RulesetsTab : GameSettingsTab
{
    public override string Name => "Rulesets";
    public override List<OptionGroup> OptionGroups => [];
    private List<PassiveButton> buttons = new();
    public override void CreateSettings(GameOptionsMenu gameOptionsMenu, Transform parentTransform, out float y)
    {
        var gameSettingMenu = gameOptionsMenu.transform.parent.parent.GetComponent<GameSettingMenu>();
        y = -1;
        var bubble = Object.Instantiate(HudManager.Instance.Chat.chatBubblePool.Prefab, parentTransform).TryCast<ChatBubble>();
        if (bubble == null) return;
        bubble.Player.InitBody();
        bubble.Player.UpdateFromLocalPlayer(PlayerMaterial.MaskType.None);
        bubble.NameText.text = "Message Preview";
        bubble.TextArea.text = "Select A Ruleset to preview!";
        bubble.Background.size = new Vector2(3.25f, 4.125f);
        bubble.transform.localPosition = new Vector3(0.32f, 0.1f, -20);
        bubble.Player.transform.localPosition = new Vector3(1.1f, 1.5f, -1f);
        bubble.votedMark.enabled = bubble.Xmark.enabled = false;
        bubble.NameText.transform.localPosition = new Vector3(1.65f, 1.75f, 0);
        bubble.TextArea.transform.localPosition = new Vector3(0.8f, 1.2f, 0);
        bubble.maskLayer = 20;
        bubble.SetMaskLayer();
        bubble.transform.SetParent(parentTransform.parent, true);
        y = 1.9f;
        buttons.Clear();
        foreach (var file in Directory.GetFiles(HostUtilityDirectories.RulesetsPath))
        {
            var fileInfo = new FileInfo(file);
            var contents = File.ReadAllText(file);
            CreateRulesetButton(gameOptionsMenu, parentTransform, fileInfo.Name, () =>
            {
                RulesetManager.CurrentRuleSet = contents;
                foreach (var btn in buttons)
                {
                    btn.SelectButton(false);
                }
                bubble.TextArea.text = contents;
            }, ref y);
        }
        CreateRulesetButton(gameOptionsMenu, parentTransform, "New Rule Set", () =>
        {
            var editUi = new GameObject("EditUi").AddComponent<RulesetEditTab>();
            editUi.transform.SetParent(gameOptionsMenu.transform.parent, false);
            editUi.transform.localPosition = Vector3.zero;
            editUi.Initialize(gameSettingMenu, gameOptionsMenu, bubble);
        }, ref y);
    }

    private PassiveButton CreateRulesetButton(GameOptionsMenu gameOptionsMenu, Transform parentTransform, string name, Action callback, ref float y)
    {
        var gameSettingMenu = gameOptionsMenu.transform.parent.parent.GetComponent<GameSettingMenu>();
        var button = Object.Instantiate(gameSettingMenu.GamePresetsButton, parentTransform);
        Object.Destroy(button.buttonText.GetComponent<TextTranslatorTMP>());
        button.buttonText.text = name;
        button.OnClick = new Button.ButtonClickedEvent();
        foreach (var rend in button.GetComponentsInChildren<SpriteRenderer>(true))
        {
            rend.color = Color.Lerp(rend.color, Color.black, 0.3f);
        }

        button.OnClick.AddListener(callback);
        button.OnClick.AddListener(new Action(() => button.SelectButton(true)));
        buttons.Add(button);
        button.transform.localPosition = new Vector3(-0f, y, -20f);
        y -= 0.7f;
        return button;
    }
}