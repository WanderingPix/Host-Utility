using System;
using BepInEx.Configuration;
using Il2CppInterop.Runtime.InteropTypes.Arrays;
using UnityEngine;
using UnityEngine.Events;
using Object = UnityEngine.Object;

namespace HostUtility.GameSettings.OptionTypes;

public class ModdedFloatRangeOption : ModdedOption
{
    public override Type OptionType => typeof(FloatRange);
    public FloatRange Range;
    public float MinDistance = 0f;   // minimum required (max - min)
    public bool RoundToInt = false;
    public ConfigEntry<float> MinEntry;
    public ConfigEntry<float> MaxEntry;
    public FloatRange StoredValue;
    public FloatRange DefaultValue;
    public FloatRange Value => GetValue();
    private const float Epsilon = 0.0001f;
    private bool IntMode => RoundToInt && Mathf.Ceil(Range.min - Epsilon) <= Range.max + Epsilon;
    private float GridMin => IntMode ? Mathf.Ceil(Range.min - Epsilon) : Range.min;
    private float GridMax => IntMode ? Mathf.Floor(Range.max + Epsilon) : Range.max;

    private float EffectiveMinDistance
    {
        get
        {
            var dist = Mathf.Max(MinDistance, 0f);
            if (IntMode) dist = Mathf.Ceil(dist - Epsilon);
            return Mathf.Min(dist, GridMax - GridMin);
        }
    }

    private float Snap(float v)
    {
        v = Mathf.Clamp(v, GridMin, GridMax);
        if (IntMode) v = Mathf.Round(v);
        return Mathf.Clamp(v, GridMin, GridMax);
    }

    public FloatRange GetValue()
    {
        if (AmongUsClient.Instance.AmHost)
        {
            if (!Init) Bind(Name);
            return Constrain(MinEntry.Value, MaxEntry.Value);
        }
        return StoredValue;
    }

    private FloatRange Constrain(float min, float max)
    {
        var dist = EffectiveMinDistance;
        min = Snap(min);
        max = Snap(max);
        if (min > max) min = max;
        if (max - min < dist - Epsilon)
        {
            max = min + dist;
            if (max > GridMax + Epsilon)
            {
                max = GridMax;
                min = max - dist;
            }
        }
        return new FloatRange(min, max);
    }

    public void SetValue(FloatRange newValue)
    {
        var constrained = Constrain(newValue.min, newValue.max);

        if (AmongUsClient.Instance.AmHost)
        {
            if (!Init) Bind(Name);
            MinEntry.Value = constrained.min;
            MaxEntry.Value = constrained.max;
        }
        StoredValue = constrained;
    }

    private static string Format(FloatRange r) => $"{r.min:0.##} - {r.max:0.##}";

    public override GameObject Create(GameOptionsMenu __instance, Transform parentTransform, ref float y)
    {
        y -= 0.45f;
        var optionBehaviour1 = Object.Instantiate(__instance.numberOptionOrigin, Vector3.zero, Quaternion.identity, __instance.settingsContainer);
        optionBehaviour1.transform.localPosition = new Vector3(1f, y, -2f);
        Object.Destroy(optionBehaviour1.TitleText.GetComponent<TextTranslatorTMP>());
        optionBehaviour1.TitleText.text = Name;
        optionBehaviour1.ValueText.gameObject.SetActive(false);
        optionBehaviour1.transform.FindChild("ValueBox").gameObject.SetActive(false);
        optionBehaviour1.SetClickMask(__instance.ButtonClickMask);
        foreach (var btn in optionBehaviour1.buttons)
        {
            btn.gameObject.SetActive(false);
        }
        optionBehaviour1.enabled = false;

        var scroller1 = Object.Instantiate(HudManager.Instance.GameMenu.SoundSlider, optionBehaviour1.transform); // Min
        scroller1.transform.localPosition = new Vector3(0.15f, -0.1f, -2f);
        scroller1.transform.GetChild(1).gameObject.SetActive(false);
        var buttonCollider = scroller1.Dot.gameObject.AddComponent<BoxCollider2D>();
        buttonCollider.isTrigger = true;
        buttonCollider.size = new Vector2(3f, 3f);
        scroller1.Colliders = new Il2CppReferenceArray<Collider2D>([
            buttonCollider
        ]);
        scroller1.Dot.transform.localScale = new Vector3(0.1f, .75f, 1);

        var scroller2 = Object.Instantiate(scroller1, optionBehaviour1.transform); // Max
        scroller2.transform.localPosition = new Vector3(0.15f, -0.1f, -2f);

        var valueText = Object.Instantiate(optionBehaviour1.TitleText, optionBehaviour1.transform);
        Object.Destroy(valueText.GetComponent<TextTranslatorTMP>());
        valueText.transform.localPosition = new Vector3(1.8f, -0.5f, -10);
        float ToReal(float t) => Range.min + t * Range.Width;
        float ToNormalized(float v) => Range.Width <= 0f ? 0f : (v - Range.min) / Range.Width;
        float SnapNorm(float t) => ToNormalized(Snap(ToReal(t)));
        var minDistNorm = Range.Width <= 0f ? 0f : EffectiveMinDistance / Range.Width;
        var initializing = true;

        void UpdateVal()
        {
            SetValue(new FloatRange(ToReal(scroller1.Value), ToReal(scroller2.Value)));
            valueText.text = Format(Value);
        }

        scroller1.OnValueChange = new UnityEvent();
        scroller1.OnValueChange.AddListener(new Action(() =>
        {
            if (initializing) return;
            var snapped = SnapNorm(scroller1.Value);
            if (Mathf.Abs(snapped - scroller1.Value) > Epsilon) scroller1.SetValue(snapped);
            var limit = SnapNorm(scroller2.Value - minDistNorm);
            if (scroller1.Value > limit + Epsilon) scroller1.SetValue(limit);
            UpdateVal();
        }));
        scroller2.OnValueChange = new UnityEvent();
        scroller2.OnValueChange.AddListener(new Action(() =>
        {
            if (initializing) return;
            var snapped = SnapNorm(scroller2.Value);
            if (Mathf.Abs(snapped - scroller2.Value) > Epsilon) scroller2.SetValue(snapped);
            var limit = SnapNorm(scroller1.Value + minDistNorm);
            if (scroller2.Value < limit - Epsilon) scroller2.SetValue(limit);
            UpdateVal();
        }));

        foreach (var rend in optionBehaviour1.GetComponentsInChildren<SpriteRenderer>(true))
        {
            rend.maskInteraction = SpriteMaskInteraction.VisibleInsideMask;
        }

        var current = Value;
        scroller1.SetValue(ToNormalized(current.min));
        scroller2.SetValue(ToNormalized(current.max));
        valueText.text = Format(current);
        initializing = false;
        return optionBehaviour1.gameObject;
    }

    public override void Bind(string groupName)
    {
        HostUtilityPlugin.Instance.Log.LogInfo($"Binding {OptionType.ToString()} option {Name}...");
        MinEntry = HostUtilityPlugin.Instance.Config.Bind(OptionType.Name + " Options", Name + " - Minimum", DefaultValue.min);
        MaxEntry = HostUtilityPlugin.Instance.Config.Bind(OptionType.Name + " Options", Name + " - Maximum", DefaultValue.max);
        HostUtilityPlugin.Instance.Log.LogInfo($"Bound {OptionType.ToString()} option {Name}...");
        Init = true;
    }
}