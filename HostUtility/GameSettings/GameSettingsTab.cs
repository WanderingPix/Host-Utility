using System.Collections.Generic;
using UnityEngine;

namespace HostUtility.GameSettings;

public abstract class GameSettingsTab
{
    public abstract string Name { get; }
    public virtual Color DisplayColor { get; } = new(0.2f, 0, 0.6f);
    public abstract List<OptionGroup> OptionGroups { get; }
    public virtual void CreateSettings(GameOptionsMenu gameOptionsMenu, Transform parentTransform)
    {
        float y = 0;
        foreach (var optionGroup in OptionGroups)
        {
            optionGroup.Create(gameOptionsMenu, parentTransform, ref y);
        }
    }
    public virtual void OnCreate(GameOptionsMenu menu, Transform parentTransform, ref float y)
    {
    }
}