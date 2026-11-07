using System.Collections.Generic;
using HostUtility.Commands;
using HostUtility.GameSettings.OptionTypes;
using UnityEngine;

namespace HostUtility.GameSettings;

public class OptionGroup
{
    public string Name;
    public List<ModdedOption> Options;
    public void Create(GameOptionsMenu gameOptionsMenu, Transform parentTransform, ref float y)
    {
        foreach (var option in Options)
        {
            y -= 0.63f;
            CategoryHeaderMasked categoryHeaderMasked = Object.Instantiate(gameOptionsMenu.categoryHeaderOrigin, Vector3.zero, Quaternion.identity, parentTransform);
            categoryHeaderMasked.Title.text = Name;
            categoryHeaderMasked.transform.localScale = Vector3.one * 0.63f;
            categoryHeaderMasked.transform.localPosition = new Vector3(-0.6f, y, -2f);
            int maskLayer = 20;
            categoryHeaderMasked.Background.color = Color.Lerp(categoryHeaderMasked.Background.color, new Color(0.1f, 0.1f, 0.5f), 0.4f);
            categoryHeaderMasked.Background.material.SetInt(PlayerMaterial.MaskLayer, maskLayer);
            if (categoryHeaderMasked.Divider != null)
                categoryHeaderMasked.Divider.material.SetInt(PlayerMaterial.MaskLayer, maskLayer);
            categoryHeaderMasked.Title.fontMaterial.SetFloat("_StencilComp", 3f);
            categoryHeaderMasked.Title.fontMaterial.SetFloat("_Stencil", maskLayer);
            y -= 0.15f;
            option.Create(gameOptionsMenu, parentTransform, ref y);
        }
    }
}