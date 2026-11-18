using System;
using UnityEngine;

namespace HostUtility.Components;

public class NumberOptionUnlocker(IntPtr ptr) : MonoBehaviour(ptr)
{
    private NumberOption _option;
    private FloatRange _range;
    private FloatRange _originalRange;
    private GameOptionButton _button;
    private bool _state;
    public void Init(NumberOption option, FloatRange extendedRange)
    {
        _originalRange = option.ValidRange;
        _option = option;
        _range = extendedRange;
        _button = GetComponent<GameOptionButton>();
        _button.SetInteractable(true);
        _button.transform.GetChild(0).gameObject.SetActive(false);
        _state = _option.Value > _range.min && _option.Value < _range.max;
        _button.buttonSprite.sprite = _state ? HostUtilityAssets.UnlockOptionSprite : HostUtilityAssets.LockOptionSprite;

        _button.OnClick.AddListener(new Action(Toggle));
    }

    public void Toggle()
    {
        _state = !_state;
        _button.buttonSprite.sprite = _state ? HostUtilityAssets.UnlockOptionSprite : HostUtilityAssets.LockOptionSprite;
        if (_state)
        {
            _option.ValidRange = _range;
        }
        else
        {
            _option.ValidRange = _originalRange;
        }
        _option.Value = Math.Clamp(_option.Value, _option.ValidRange.min, _option.ValidRange.max);
        _option.UpdateValue();
        _option.AdjustButtonsActiveState();
    }
}