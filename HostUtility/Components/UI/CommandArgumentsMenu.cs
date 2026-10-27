using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using HostUtility.Commands;
using HostUtility.Commands.Arguments;
using Il2CppInterop.Runtime.InteropTypes.Fields;
using TMPro;
using UnityEngine;
using UnityEngine.Serialization;
using UnityEngine.UI;
using Object = System.Object;

namespace HostUtility.Components.UI;

public class CommandArgumentsMenu(IntPtr ptr) : MonoBehaviour(ptr)
{
    public Il2CppReferenceField<Button> buttonPrefab;
    public Il2CppReferenceField<Sprite> playerCharacterSprite;
    public Il2CppReferenceField<Sprite> checkmarkSprite;
    public Il2CppReferenceField<Sprite> crossSprite;
    public Il2CppReferenceField<Transform> contentParent;
    public List<Button> Buttons = new();
    public Queue<CommandArgument> ArgumentsLeft = new();
    public CommandArgument CurrentArgument;
    private RectTransform _rectTransform;
    private Canvas _canvas;
    public void Begin(ChatCommand command)
    {
        gameObject.SetActive(true);
        var field = HudManager.Instance.Chat.freeChatField;
        field.submitButton.gameObject.SetActive(false);
        string text = "/cmd " + command.Command.ToLower();
        foreach (var arg in command.Arguments)
        {
            text += $" {arg.ArgumentName},";
        }

        text = text.Trim(",").ToString();
        field.textArea.SetText(text);
        ArgumentsLeft = new Queue<CommandArgument>(command.Arguments);
        field.textArea.enabled = false;
        GoToNextArgument();
    }

    private void GoToNextArgument()
    {
        if (ArgumentsLeft.Count == 0)
        {
            gameObject.SetActive(false);
            var field = HudManager.Instance.Chat.freeChatField;
            field.submitButton.gameObject.SetActive(true);
            field.textArea.enabled = true;
            return;
        }
        var arg = ArgumentsLeft.Dequeue();
        CurrentArgument = arg;
        foreach (var button in Buttons)
        {
            Destroy(button.gameObject);
        }
        Buttons.Clear();
        arg.CreateOptions(this);

        var sourceText = HudManager.Instance.TaskPanel.taskText;
        foreach (var tmp in gameObject.GetComponentsInChildren<TextMeshProUGUI>())
        {
            tmp.fontMaterial = new(sourceText.fontMaterial);
            tmp.font = sourceText.font;
            tmp.color = Color.white;
        }
    }

    public void CreateOption(string text, string value, ArgumentSpriteTypes spriteType = ArgumentSpriteTypes.None, bool applyPlayerShader = false, int colorId = -1)
    {
        var btn = Instantiate(buttonPrefab.Value, contentParent.Value);
        btn.transform.GetChild(0).GetComponent<TextMeshProUGUI>().text = text;
        var rend = btn.transform.GetChild(1).GetComponent<Image>();
        Sprite sprite;
        switch (spriteType)
        {
            case ArgumentSpriteTypes.Player:
                sprite = playerCharacterSprite.Value;
                break;
            case ArgumentSpriteTypes.Checkmark:
                sprite = checkmarkSprite.Value;
                break;
            case ArgumentSpriteTypes.Cross:
                sprite = crossSprite.Value;
                break;
            case ArgumentSpriteTypes.None:
            default:
                sprite = null!;
                break;
        }

        rend.sprite = sprite;
        if (applyPlayerShader)
        {
            rend.material = new Material(HatManager.Instance.PlayerMaterial);
            if (colorId != -1) PlayerMaterial.SetColors(colorId, rend.material);
        }
        btn.m_OnClick.AddListener(new Action(() =>
        {
            var field = HudManager.Instance.Chat.freeChatField;
            field.textArea.SetText(field.textArea.text.Replace(CurrentArgument.ArgumentName, value));
            GoToNextArgument();
        }));
        Buttons.Add(btn);
    }

    private void Start()
    {
        Instance = this;
        HudManager.Instance.Chat.freeChatField.OnChangedEvent += new Action(() =>
        {
            var chatText = HudManager.Instance.Chat.freeChatField.textArea.text;
            var cmd = ChatCommandsManager.Commands.Find(x => $"/cmd {x.Command.ToLower()} " == chatText);
            if (cmd != null) Instance.Begin(cmd);
        });
        _rectTransform = GetComponent<RectTransform>();
        _canvas = GetComponentInParent<Canvas>();
        gameObject.SetActive(false);
    }

    public static CommandArgumentsMenu Instance { get; set; }

    private void OnEnable()
    {
        if (!HudManager.InstanceExists)
        {
            return;
        }
        HudManager.Instance.StartCoroutine(Effects.Bloop(0, _rectTransform.transform, 0.7375f));
    }

    private void Update()
    {
        if (!HudManager.InstanceExists) return;

        var field = HudManager.Instance.Chat.freeChatField;
        var cam = HudManager.Instance.UICamera;
        var bounds = field.background.bounds;

        Vector2 min = cam.WorldToScreenPoint(bounds.min);
        Vector2 max = cam.WorldToScreenPoint(bounds.max);
        float fieldW = max.x - min.x;
        float fieldH = max.y - min.y;

        const float widthFraction  = 1f;
        const float heightFraction = 3f;
        const float gapFraction = 0.25f;

        float s = _canvas.scaleFactor;

        _rectTransform.sizeDelta = new Vector2(fieldW * widthFraction, fieldH * heightFraction) / s;
        _rectTransform.pivot = Vector2.zero;
        _rectTransform.position = new Vector3(
            min.x + fieldW * (1f - widthFraction) * 0.5f,
            max.y + fieldH * gapFraction,
            0f);
    }

    public enum ArgumentSpriteTypes
    {
        Player,
        Checkmark,
        Cross,
        None
    }
}
