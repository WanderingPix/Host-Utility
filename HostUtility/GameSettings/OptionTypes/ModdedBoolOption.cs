using System;
using BepInEx.Configuration;
using UnityEngine;
using Object = System.Object;

namespace HostUtility.GameSettings.OptionTypes;

public class ModdedBoolOption : ModdedOption<bool>
{
    public ModdedBoolOption(string name, bool defaultValue)
    {
        Name = name;
        DefaultValue = defaultValue;
    }

    public override GameObject Create(GameOptionsMenu __instance, Transform parentTransform, ref float y)
    {
        y -= 0.45f;
        var optionBehaviour1 = UnityEngine.Object.Instantiate<ToggleOption>(__instance.checkboxOrigin, Vector3.zero, Quaternion.identity, __instance.settingsContainer);
        optionBehaviour1.transform.localPosition = new Vector3(1.2f, y, -2f);
        optionBehaviour1.SetClickMask(__instance.ButtonClickMask);
        foreach (var rend in optionBehaviour1.GetComponentsInChildren<SpriteRenderer>(true))
        {
            rend.maskInteraction = SpriteMaskInteraction.VisibleInsideMask;
        }

        optionBehaviour1.CheckMark.color = Color.white;
    
        optionBehaviour1.StartCoroutine(Effects.ActionAfterDelay(0.05f, new System.Action(() => optionBehaviour1.TitleText.text = Name)));
        optionBehaviour1.CheckMark.enabled = ConfigEntry.Value;
        optionBehaviour1.buttons[0].OnClick = new();
        optionBehaviour1.buttons[0].OnClick.AddListener(new System.Action(() =>
        {
            
            ConfigEntry.Value = !ConfigEntry.Value;
            optionBehaviour1.CheckMark.enabled = ConfigEntry.Value;
        }));
        __instance.Children.Add(optionBehaviour1);
        
        return optionBehaviour1.gameObject;
    }
}