using System;
using BepInEx.Configuration;
using UnityEngine;
using Object = System.Object;

namespace HostUtility.GameSettings.OptionTypes;

public abstract class ModdedOption
{
    public string Name;
    public bool Init = false;
    public abstract Type OptionType { get; }
    public abstract GameObject Create(GameOptionsMenu menu, Transform parentTransform, ref float y);
    public abstract void Bind(string groupName);
}

public abstract class ModdedOption<T> : ModdedOption
{
    public override Type OptionType => typeof(T);
    public ConfigEntry<T> ConfigEntry;
    public T Value => GetValue();
    public T StoredValue { get; set; }
    public T DefaultValue { get; set; }

    private T GetValue()
    {
        if (AmongUsClient.Instance.AmHost)
        {
            if (!Init) Bind(Name);
            return ConfigEntry.Value;
        }

        return StoredValue;
    }
    public override void Bind(string groupName)
    {
        HostUtilityPlugin.Instance.Log.LogInfo($"Binding {OptionType.ToString()} option {Name}...");
        ConfigEntry = HostUtilityPlugin.Instance.Config.Bind(OptionType.Name + " Options", Name, DefaultValue);
        HostUtilityPlugin.Instance.Log.LogInfo($"Bound {OptionType.ToString()} option {Name}...");
    }
}