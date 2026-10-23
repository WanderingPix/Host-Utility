using System;
using System.Collections.Generic;
using System.Linq;
using Il2CppInterop.Runtime.InteropTypes.Fields;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace HostUtility.Components.UI;

public class CommandSuggestionMenu(IntPtr ptr) : MonoBehaviour(ptr)
{
    public Il2CppReferenceField<Button> SuggestionPrefab;
    public List<Button> Buttons = new();
    private RectTransform _rectTransform;
    private Canvas _canvas;

    public void Refresh()
    {
        foreach (var button in Buttons)
        {
            Destroy(button.gameObject);
        }

        Buttons = new();

        string text = HudManager.Instance.Chat.freeChatField.Text;
        if (text.IsNullOrWhiteSpace() || !text.StartsWith("/cmd"))
        {
            gameObject.SetActive(false);
            return;
        }

        string searchTerm = text.Replace("/cmd ", "").ToLower();
        foreach (var cmd in ChatCommandsManager.Commands.Where(x => x.Command.ToLower().Contains(searchTerm)).Take(6))
        {
            if (cmd.HostOnly && !AmongUsClient.Instance.AmHost) continue;
            var btn = Instantiate(SuggestionPrefab.Value, transform);
            Buttons.Add(btn);
            btn.transform.GetChild(0).GetComponent<TextMeshProUGUI>().text = cmd.Command;
            btn.transform.GetChild(1).GetComponent<TextMeshProUGUI>().text = cmd.Description;
            btn.onClick.AddListener(new Action(() =>
            {
                var targetTextBox = HudManager.Instance.Chat.freeChatField.textArea;
                targetTextBox.SetText("/cmd " + cmd.Command);
                CommandArgumentsMenu.Instance.Begin(cmd);
                gameObject.SetActive(false);
            }));
        }
        if (Buttons.Count == 0) gameObject.SetActive(false);
        else gameObject.SetActive(true);
    }

    private void Start()
    {
        _rectTransform = GetComponent<RectTransform>();
        _canvas = GetComponentInParent<Canvas>();
        HudManager.Instance.Chat.freeChatField.OnChangedEvent += new Action(Refresh);
        var sourceText = HudManager.Instance.TaskPanel.taskText;
        foreach (var tmp in SuggestionPrefab.Value.gameObject.GetComponentsInChildren<TextMeshProUGUI>())
        {
            tmp.fontMaterial = new(sourceText.fontMaterial);
            tmp.font = sourceText.font;
        }
        gameObject.SetActive(false);
    }

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
        const float heightFraction = 0.75f;
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