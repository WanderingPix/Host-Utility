using System;
using BepInEx.Configuration;
using UnityEngine;
using Object = System.Object;

namespace HostUtility.GameSettings.OptionTypes;

public abstract class ModdedOption
{
    public string Name;
    public abstract Type OptionType { get; }
    public abstract GameObject Create(GameOptionsMenu menu, Transform parentTransform, ref float y);
    public abstract void Bind(string groupName, ConfigFile config);
}

public abstract class ModdedOption<T> : ModdedOption
{
    public override Type OptionType => typeof(T);
    public ConfigEntry<T> ConfigEntry;
    public T Value => ConfigEntry.Value;
    public T DefaultValue { get; set; }

    public override void Bind(string groupName, ConfigFile config)
    {
        config.Bind(new ConfigDefinition(groupName, Name), DefaultValue);
    }
}