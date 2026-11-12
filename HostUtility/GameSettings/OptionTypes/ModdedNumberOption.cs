using System;
using AmongUs.GameOptions;
using BepInEx.Configuration;
using UnityEngine;
using Object = UnityEngine.Object;

namespace HostUtility.GameSettings.OptionTypes;

public class ModdedNumberOption : ModdedOption<int>
{
    public ModdedNumberOption(string name, int defaultValue, int min, int max, int increment, string suffix = "")
    {
        Name = name;
        DefaultValue = defaultValue;
        MinimumValue = min;
        MaximumValue = max;
        Suffix = suffix;
        Increment = increment;
    }

    public int MinimumValue { get; set; }
    public int Increment { get; set; }

    public string Suffix { get; set; }

    public int MaximumValue { get; set; }

    public override GameObject Create(GameOptionsMenu __instance, Transform parentTransform, ref float y)
    {
        y -= 0.45f;
        var optionBehaviour1 = UnityEngine.Object.Instantiate(__instance.numberOptionOrigin, Vector3.zero, Quaternion.identity, __instance.settingsContainer);
        optionBehaviour1.transform.localPosition = new Vector3(1f, y, -2f);
        Object.Destroy(optionBehaviour1.TitleText.GetComponent<TextTranslatorTMP>());
        optionBehaviour1.SetClickMask(__instance.ButtonClickMask);
        foreach (var rend in optionBehaviour1.GetComponentsInChildren<SpriteRenderer>(true))
        {
            rend.maskInteraction = SpriteMaskInteraction.VisibleInsideMask;
        }

        optionBehaviour1.TitleText.text = Name;
        optionBehaviour1.ValidRange = new FloatRange(MinimumValue, MaximumValue);
        optionBehaviour1.Value = Value;
        optionBehaviour1.ValueText.text = Value + Suffix;
        optionBehaviour1.Increment = Increment;
        optionBehaviour1.intOptionName = Int32OptionNames.Invalid;
        optionBehaviour1.floatOptionName = FloatOptionNames.Invalid;
        
        optionBehaviour1.StartCoroutine(Effects.ActionAfterDelay(0.05f, new System.Action(() =>
        {
            optionBehaviour1.MinusBtn.SetInteractable(true);
            optionBehaviour1.PlusBtn.SetInteractable(true);
            optionBehaviour1.TitleText.text = Name;
        })));
        optionBehaviour1.buttons[0].OnClick = new();
        optionBehaviour1.buttons[0].OnClick.AddListener(new System.Action(() =>
        {
            optionBehaviour1.Value -= optionBehaviour1.Increment;
            optionBehaviour1.Value = Mathf.Clamp(optionBehaviour1.Value, optionBehaviour1.ValidRange.min, optionBehaviour1.ValidRange.max);
            optionBehaviour1.ValueText.text = optionBehaviour1.Value + Suffix;
            ConfigEntry.Value = (int)optionBehaviour1.Value;
            optionBehaviour1.PlusBtn.SetInteractable(true);
            optionBehaviour1.MinusBtn.SetInteractable(true);
        }));
        optionBehaviour1.buttons[1].OnClick = new();
        optionBehaviour1.buttons[1].OnClick.AddListener(new System.Action(() =>
        {
            optionBehaviour1.Value += optionBehaviour1.Increment;
            optionBehaviour1.Value = Mathf.Clamp(optionBehaviour1.Value, optionBehaviour1.ValidRange.min, optionBehaviour1.ValidRange.max);
            optionBehaviour1.ValueText.text = optionBehaviour1.Value + Suffix;
            ConfigEntry.Value = (int)optionBehaviour1.Value;
        }));
        __instance.Children.Add(optionBehaviour1);
        optionBehaviour1.enabled = false;
        
        return optionBehaviour1.gameObject;
    }
}