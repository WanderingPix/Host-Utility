using System;
using System.Collections.Generic;
using AmongUs.GameOptions;
using TMPro;
using UnityEngine;

namespace HostUtility.Components;

public class NumberOptionDivider(IntPtr ptr) : MonoBehaviour(ptr)
{
    private NumberOption _option;
    private float _originalIncrement;
    private FloatRange _originalRange;
    private GameOptionButton _button;
    private TextMeshPro _text;
    private int _state = 1;
    public void Init(NumberOption option)
    {
        _originalRange = option.ValidRange;
        _option = option;
        _originalIncrement = option.Increment;
        _button = GetComponent<GameOptionButton>();
        _button.SetInteractable(true);
        _text = _button.transform.GetChild(0).GetComponent<TextMeshPro>();

        if (!OptionStates.TryGetValue(option.floatOptionName, out int lastState))
        {
            lastState = 1;
            OptionStates.Add(option.floatOptionName, lastState);
        }

        _state = lastState;
        _button.OnClick.AddListener(new Action(OnClick));
        _text.text = $"1/{_state}";
        _text.fontSize *= 0.7f;
        _text.transform.localPosition = new Vector3(0, 0.05f, -2);
    }

    public static Dictionary<FloatOptionNames, int> OptionStates = new Dictionary<FloatOptionNames, int>();

    public void OnClick()
    {
        _state *= 2;
        if (_state > 4)
        {
            _state = 1;
        }

        _text.text = $"1/{_state}";
        _option.Increment = _originalIncrement / _state;
    }
}