using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using HostUtility.Commands;
using Il2CppInterop.Runtime.InteropTypes.Fields;
using TMPro;
using UnityEngine;
using UnityEngine.Serialization;
using UnityEngine.UI;

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
            text += $" {arg.Name},";
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
        foreach (var button in Buttons)
        {
            Destroy(button.gameObject);
        }
        Buttons.Clear();
        switch (arg.Type)
        {
            case CommandArgument.ArgumentType.Bool:
                var btnOne = Instantiate(buttonPrefab.Value, contentParent.Value);
                btnOne.transform.GetChild(0).GetComponent<TextMeshProUGUI>().text = "True";
                btnOne.transform.GetChild(1).GetComponent<Image>().sprite = checkmarkSprite.Value;
                btnOne.m_OnClick.AddListener(new Action(() =>
                {
                    var field = HudManager.Instance.Chat.freeChatField;
                    field.textArea.SetText(field.textArea.text.Replace(arg.Name, "true"));
                    GoToNextArgument();
                }));
                var btnTwo = Instantiate(buttonPrefab.Value, contentParent.Value);
                btnTwo.transform.GetChild(0).GetComponent<TextMeshProUGUI>().text = "False";
                btnTwo.transform.GetChild(1).GetComponent<Image>().sprite = crossSprite.Value;
                btnTwo.m_OnClick.AddListener(new Action(() =>
                {
                    var field = HudManager.Instance.Chat.freeChatField;
                    field.textArea.SetText(field.textArea.text.Replace(arg.Name, "false"));
                    GoToNextArgument();
                }));
                Buttons.Add(btnOne);
                Buttons.Add(btnTwo);
                break;
            case CommandArgument.ArgumentType.Player:
                foreach (var p in PlayerControl.AllPlayerControls)
                {
                    var btn = Instantiate(buttonPrefab.Value, contentParent.Value);
                    btn.transform.GetChild(0).GetComponent<TextMeshProUGUI>().text = p.Data.PlayerName;
                    var rend = btn.transform.GetChild(1).GetComponent<Image>();
                    rend.sprite = playerCharacterSprite.Value;
                    rend.material = new Material(HatManager.Instance.PlayerMaterial);
                    PlayerMaterial.SetColors(p.cosmetics.ColorId, rend.material);
                    btn.m_OnClick.AddListener(new Action(() =>
                    {
                        var field = HudManager.Instance.Chat.freeChatField;
                        field.textArea.SetText(field.textArea.text.Replace(arg.Name, p.Data.PlayerName));
                        GoToNextArgument();
                    }));
                    Buttons.Add(btn);
                }
                break;
            case CommandArgument.ArgumentType.Color:
                int colorIndex = 0;
                foreach (var color in Palette.ColorNames)
                {
                    var btn = Instantiate(buttonPrefab.Value, contentParent.Value);
                    btn.transform.GetChild(0).GetComponent<TextMeshProUGUI>().text = TranslationController.Instance.GetString(color);
                    var rend = btn.transform.GetChild(1).GetComponent<Image>();
                    rend.sprite = playerCharacterSprite.Value;
                    rend.material = new Material(HatManager.Instance.PlayerMaterial);
                    PlayerMaterial.SetColors(colorIndex, rend.material);
                    var index = colorIndex;
                    btn.m_OnClick.AddListener(new Action(() =>
                    {
                        var field = HudManager.Instance.Chat.freeChatField;
                        field.textArea.SetText(field.textArea.text.Replace(arg.Name, index.ToString()));
                        GoToNextArgument();
                    }));
                    colorIndex++;
                    Buttons.Add(btn);
                }
                break;
            default:
                var btnErr = Instantiate(buttonPrefab.Value, contentParent.Value);
                btnErr.transform.GetChild(0).GetComponent<TextMeshProUGUI>().text = "If you're reading this, Pix messed up.\nReport it to him in the RFS Discord.";
                Buttons.Add(btnErr);
                break;
        }
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
}