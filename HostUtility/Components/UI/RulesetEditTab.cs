using System;
using System.Collections.Generic;
using System.IO;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using Object = UnityEngine.Object;

namespace HostUtility.Components.UI;

public class RulesetEditTab(IntPtr ptr) : MonoBehaviour(ptr)
{
    public GameSettingMenu gameSettingsMenu;
    public GameOptionsMenu gameOptionsMenu;
    public ChatBubble bubble;
    public TextBoxTMP bubbleTextBox;
    public PassiveButton bubbleButton;
    public FreeChatInputField saveField;

    public void Initialize(GameSettingMenu menu, GameOptionsMenu menu2, ChatBubble myBubble)
    {
        gameSettingsMenu = menu;
        gameOptionsMenu = menu2;
        bubble = Instantiate(myBubble, myBubble.transform.parent, true);
        bubbleTextBox = bubble.TextArea.gameObject.AddComponent<TextBoxTMP>();
        bubbleTextBox.outputText = bubble.TextArea;
        bubbleTextBox.OnChange = new Button.ButtonClickedEvent();
        bubbleTextBox.OnEnter = new Button.ButtonClickedEvent();
        bubbleTextBox.OnFocusLost  = new Button.ButtonClickedEvent();
        bubbleTextBox.allowAllCharacters = true;
        bubbleTextBox.characterLimit = 250;
        bubble.NameText.m_enableWordWrapping = true;
        bubbleTextBox.Pipe = Instantiate(HudManager.Instance.Chat.freeChatField.textArea.Pipe, bubbleTextBox.transform);
        bubbleTextBox.pipeBlinkTimer = 0.7f;
        bubbleTextBox.caretYOffset = 0;
        bubble.TextArea.text = bubbleTextBox.text = string.Empty;
        bubbleTextBox.keyboard = new TouchScreenKeyboard(string.Empty, TouchScreenKeyboardType.Social, true, true, false, false, string.Empty, 100);
        bubbleButton = bubble.Background.gameObject.AddComponent<PassiveButton>();
        var col = bubble.Background.gameObject.AddComponent<BoxCollider2D>();
        col.isTrigger = true;
        col.size = bubble.Background.size;
        bubbleButton.Colliders = new([col]);
        bubbleButton.OnClick = new Button.ButtonClickedEvent();
        bubbleButton.OnMouseOut = new Button.ButtonClickedEvent();
        bubbleButton.OnMouseOver = new Button.ButtonClickedEvent();
        bubbleButton.OnClick = new Button.ButtonClickedEvent();
        bubbleButton.OnClick.AddListener(new Action(bubbleTextBox.GiveFocus));
        saveField = Instantiate(HudManager.Instance.Chat.freeChatField, transform);
        saveField.submitButton.button.OnClick = new();
        saveField.submitButton.button.OnClick.AddListener(new Action(Save));
        saveField.textArea.SetText(string.Empty);
        saveField.submitButton.button.buttonText.GetComponent<TextTranslatorTMP>().enabled = false; 
        saveField.submitButton.button.buttonText.text = "Save";
        saveField.submitButton.button.buttonText.transform.localPosition = new Vector3(-0.125f, 0, -10);
        saveField.submitButton.transform.localPosition = new Vector3(1.75f, 0, -1f);
        menu.StartCoroutine(Effects.ActionAfterDelay(0.01f, new Action(() => 
            saveField.submitButton.transform.localPosition = new Vector3(1.75f, 0, -1f))));
        saveField.submitButton.transform.localScale *= 1.2f;
        foreach (var icon in saveField.submitButton.iconSprites)
        {
            icon.enabled = false;
        }
        saveField.transform.localPosition = new Vector3(-0.6f, -4.1f, -50);
        saveField.transform.localScale *= 0.8f;
        saveField.background.size =
            saveField.fieldButton.Colliders[0].TryCast<BoxCollider2D>()!.size = new Vector2(3, 0.6f);
        saveField.charCountText.enabled = false;
        saveField.OnChangedEvent += new Action(() =>
        {
            saveField.transform.localPosition = new Vector3(-0.6f, -4.1f, -50);
        });
        BeginEditing();
    }

    public void Save()
    {
        File.WriteAllText(Path.Combine(HostUtilityDirectories.RulesetsPath, saveField.Text), bubbleTextBox.text);
        foreach (var g in _toReactivate)
        {
            g.SetActive(true);
        }
        Destroy(gameObject);
    }
    
    public void BeginEditing()
    {
        var parentTransform = gameOptionsMenu.settingsContainer;
        gameObject.transform.SetParent(gameOptionsMenu.transform, false);
        parentTransform.transform.parent.gameObject.SetActive(false);
        gameObject.transform.localPosition = Vector3.zero;
        bubble.transform.SetParent(gameObject.transform, true);
        var leftPanel = gameSettingsMenu.transform.Find("LeftPanel").gameObject;
        leftPanel.gameObject.SetActive(false);
        var originalLabel = gameSettingsMenu.transform.FindChild("GameSettingsLabel");
        var label = Instantiate(originalLabel, gameObject.transform, true).GetComponent<TextMeshPro>();
        originalLabel.gameObject.SetActive(false);
        Destroy(label.GetComponent<TextTranslatorTMP>());
        label.text = "Ruleset Editor";
        var tint = gameSettingsMenu.transform.FindChild("PanelSprite/LeftSideTint").gameObject;
        tint.SetActive(false);
        var tip = gameSettingsMenu.MenuDescriptionText.transform.parent.gameObject;
        tip.gameObject.SetActive(false);
        bubble.maskLayer = 0;
        bubble.SetMaskLayer();
        var formatControls = CreateFormatControls();
        formatControls.transform.SetParent(gameObject.transform, false);
        formatControls.transform.localPosition = new Vector3(-0.95f, -1.5f, -1);
        var sizeControls = CreateSizeControls();
        sizeControls.transform.SetParent(gameObject.transform, false);
        sizeControls.transform.localPosition = new Vector3(-0.95f, -2f, -1);
        var colorControls = CreateColorControls(null);
        colorControls.transform.SetParent(gameObject.transform, false);
        colorControls.transform.localPosition = new Vector3(-1.175f, -2.5f, -1);

        _toReactivate = [parentTransform.transform.parent.gameObject, leftPanel, originalLabel.gameObject, tint, tip ];
    }

    private List<GameObject> _toReactivate;
    
    //Size controls
    public List<PassiveButton> sizeButtons = [];
    public GameObject CreateSizeControls()
    {
        var container = new GameObject("SizeControls");
        var grid = container.AddComponent<GridArrange>();
        grid.Alignment = GridArrange.StartAlign.Right;
        grid.MaxColumns = 6;
        grid.CellSize = new Vector2(0.9f, 0.355f);
        int[] sizes = [75, 100, 125, 150];
        sizeButtons = [];
        foreach (var size in sizes)
        {
            var btn = CreateSizeButton(size + "%", () =>
            {
                foreach (var btn in sizeButtons)
                {
                    btn.SelectButton(false);
                }
                bubbleTextBox.SetText(bubbleTextBox.text + $"<size={size}%>");
                bubbleTextBox.GiveFocus();
            });
            btn.transform.SetParent(container.transform);
            sizeButtons.Add(btn);
        }
        return container;
    }
    
    private PassiveButton CreateSizeButton(string buttonName, Action callback)
    {
        var gameSettingMenu = gameOptionsMenu.transform.parent.parent.GetComponent<GameSettingMenu>();
        var button = Instantiate(gameSettingMenu.GamePresetsButton);
        Destroy(button.buttonText.GetComponent<TextTranslatorTMP>());
        button.buttonText.text = buttonName;
        button.buttonText.transform.localPosition = new Vector3(0f, 0.025f, -10);
        button.buttonText.alignment = TextAlignmentOptions.Center;
        button.OnClick = new Button.ButtonClickedEvent();
        foreach (var rend in button.GetComponentsInChildren<SpriteRenderer>(true))
        {
            rend.color = Color.Lerp(rend.color, Color.black, 0.7f);
        }

        button.OnClick.AddListener(callback);
        button.OnClick.AddListener(new Action(() => button.SelectButton(true)));
        button.GetComponent<BoxCollider2D>().size = 
            button.activeSprites.GetComponent<SpriteRenderer>().size = 
                button.selectedSprites.GetComponent<SpriteRenderer>().size =
                    button.inactiveSprites.GetComponent<SpriteRenderer>().size = new Vector2(1, 0.5f);
        button.buttonText.m_maxFontSize = 1;
        return button;
    }
    
    //Color controls management
    public List<PassiveButton> colorButtons = [];
    public GameObject CreateColorControls(GameOptionsMenu menu)
    {
        var container = new GameObject("ColorControls");
        var grid = container.AddComponent<GridArrange>();
        grid.Alignment = GridArrange.StartAlign.Right;
        grid.MaxColumns = 8;
        grid.CellSize = new Vector2(0.4f, -0.4f);
        colorButtons = [];
        foreach (var col in Colors)
        {
            var btn = CreateColorButton(col);
            btn.transform.SetParent(container.transform);
            colorButtons.Add(btn);
        }
        colorButtons[0].SelectButton(true);
        return container;
    }
    
    public static readonly Color32[] Colors =
    [
        // Light (pastel)
        new Color32(255, 255, 255, 255),
        new Color32(255, 166, 166, 255), // Light Red
        new Color32(166, 255, 166, 255), // Light Green
        new Color32(166, 166, 255, 255), // Light Blue
        new Color32(166, 255, 255, 255), // Light Cyan
        new Color32(255, 224, 166, 255), // Light Orange
        new Color32(255, 203, 229, 255), // Light Pink
        new Color32(211, 166, 211, 255), // Light Purple

        // Normal
        new Color32(127, 127, 127, 255),
        new Color32(255, 0, 0, 255),     // Red
        new Color32(0, 255, 0, 255),     // Green
        new Color32(0, 0, 255, 255),     // Blue
        new Color32(0, 255, 255, 255),   // Cyan
        new Color32(255, 165, 0, 255),   // Orange
        new Color32(255, 105, 180, 255), // Pink
        new Color32(128, 0, 128, 255),   // Purple

        // Dark
        new Color32(0, 0, 0, 255),
        new Color32(128, 0, 0, 255),     // Dark Red
        new Color32(0, 128, 0, 255),     // Dark Green
        new Color32(0, 0, 128, 255),     // Dark Blue
        new Color32(0, 128, 128, 255),   // Dark Cyan
        new Color32(128, 82, 0, 255),    // Dark Orange
        new Color32(128, 53, 90, 255),   // Dark Pink
        new Color32(64, 0, 64, 255)      // Dark Purple
    ];
    private PassiveButton CreateColorButton(Color32 color)
    {
        var gameSettingMenu = gameOptionsMenu.transform.parent.parent.GetComponent<GameSettingMenu>();
        var button = Instantiate(gameSettingMenu.GamePresetsButton);
        Destroy(button.buttonText.GetComponent<TextTranslatorTMP>());
        button.buttonText.text = "●";
        button.buttonText.transform.localPosition = new Vector3(0f, 0.05f, -10);
        button.buttonText.m_maxFontSize = 3.5f;
        button.activeTextColor = button.inactiveTextColor = button.selectedTextColor = button.buttonText.color = color;
        button.buttonText.transform.localPosition = new Vector3(0f, 0.025f, -10);
        button.buttonText.alignment = TextAlignmentOptions.Center;
        button.OnClick = new Button.ButtonClickedEvent();
        foreach (var rend in button.GetComponentsInChildren<SpriteRenderer>(true))
        {
            rend.color = Color.Lerp(color, Color.black, 0.3f);
        }

        button.OnClick.AddListener(new Action(() =>
        {
            foreach (var btn in colorButtons)
            {
                btn.SelectButton(false);
            }
            button.SelectButton(!button.selected);
            bubbleTextBox.SetText(bubbleTextBox.text + $"<color=#{ColorUtility.ToHtmlStringRGBA(color)}>");
            bubbleTextBox.GiveFocus();
        }));
        button.GetComponent<BoxCollider2D>().size = 
            button.activeSprites.GetComponent<SpriteRenderer>().size = 
                button.selectedSprites.GetComponent<SpriteRenderer>().size =
                    button.inactiveSprites.GetComponent<SpriteRenderer>().size = new Vector2(0.5f, 0.5f);
        button.buttonText.m_maxFontSize = 1;
        return button;
    }
    //Formatting controls
    
    public GameObject CreateFormatControls()
    {
        var container = new GameObject("FormatControls");
        var grid = container.AddComponent<GridArrange>();
        grid.Alignment = GridArrange.StartAlign.Right;
        grid.MaxColumns = 8;
        grid.CellSize = new Vector2(0.9f, 0.355f);
        colorButtons = [];

        List<PassiveButton> buttons = [
            CreateFormatButton("Ab", "<b>", "</b>"),
            CreateFormatButton("Ab", "<i>", "</i>"),
            CreateFormatButton("Ab", "<s>", "</s>"),
            CreateFormatButton("Ab", "<u>", "</u>"),
        ];
        
        foreach (var btn in buttons) btn.transform.SetParent(container.transform);
        
        return container;
    }
    
    private PassiveButton CreateFormatButton(string buttonName, string openingTag, string closingTag)
    {
        var gameSettingMenu = gameOptionsMenu.transform.parent.parent.GetComponent<GameSettingMenu>();
        var button = Instantiate(gameSettingMenu.GamePresetsButton);
        Destroy(button.buttonText.GetComponent<TextTranslatorTMP>());
        button.buttonText.text = openingTag + buttonName + closingTag;
        button.buttonText.transform.localPosition = new Vector3(0f, 0.025f, -10);
        button.buttonText.alignment = TextAlignmentOptions.Center;
        button.OnClick = new Button.ButtonClickedEvent();
        foreach (var rend in button.GetComponentsInChildren<SpriteRenderer>(true))
        {
            rend.color = Color.Lerp(rend.color, Color.black, 0.7f);
        }

        button.OnClick.AddListener(new Action(() =>
        {
            foreach (var btn in colorButtons)
            {
                btn.SelectButton(false);
            }
            button.SelectButton(!button.selected);
            bubbleTextBox.SetText(bubbleTextBox.text + (button.selected ? openingTag : closingTag));
            bubbleTextBox.GiveFocus();
        }));
        button.GetComponent<BoxCollider2D>().size = 
            button.activeSprites.GetComponent<SpriteRenderer>().size = 
                button.selectedSprites.GetComponent<SpriteRenderer>().size =
                    button.inactiveSprites.GetComponent<SpriteRenderer>().size = new Vector2(1, 0.5f);
        button.buttonText.m_minFontSize = 0.5f;
        button.buttonText.m_maxFontSize = 2f;
        return button;
    }
}