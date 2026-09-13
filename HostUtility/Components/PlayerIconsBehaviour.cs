using System;
using System.Collections.Generic;
using System.Linq;
using Reactor.Utilities;
using Reactor.Utilities.Attributes;
using Reactor.Utilities.Extensions;
using TMPro;
using UnityEngine;
using Object = UnityEngine.Object;

namespace HostUtility.Components;

/// <summary>
/// Utility component for handling player icons in a centered layout, similar to <see cref="GridArrange"/>.
/// </summary>
/// <param name="iPtr">The <see cref="IntPtr"/> for the component.</param>
[RegisterInIl2Cpp]
public class PlayerIconsBehaviour(IntPtr iPtr) : MonoBehaviour(iPtr)
{
    /// <summary>
    /// Gets or sets the cell size, which is used for spacing.
    /// </summary>
    public Vector2 CellSize = new Vector2(0.3f, 0.3f);

    /// <summary>
    /// Gets or sets the maximum amount of columns.
    /// </summary>
    public int MaxColumns = 6;

    private List<Transform> cells;
    private List<Transform> currentChildren = new List<Transform>();

    private void Start()
    {
        AllIcons.Add(this);
        cells = new List<Transform>();
        GetChildsActive();
        CheckCurrentChildren();
    }

    private void OnDestroy()
    {
        AllIcons.Remove(this);
    }

    public static List<PlayerIconsBehaviour> AllIcons = new();

    public GameObject LevelIcon;
    public GameObject IdIcon;
    public GameObject HostIcon;
    public GameObject PlatformIcon;
    public void Initialize(PlayerControl owner)
    {
        LevelIcon = Object.Instantiate(HudManager.Instance.MeetingPrefab.PlayerButtonPrefab.LevelNumberText.transform.parent, transform).gameObject;
        LevelIcon.transform.GetChild(1).GetComponent<TextMeshPro>().text = owner.Data.PlayerLevel.ToString();
        IdIcon = Object.Instantiate(HudManager.Instance.MeetingPrefab.PlayerButtonPrefab.LevelNumberText.transform.parent, transform).gameObject;
        IdIcon.transform.GetChild(1).GetComponent<TextMeshPro>().text = owner.Data.PlayerId.ToString();
        var headerText = IdIcon.transform.GetChild(0);
        headerText.GetComponent<TextTranslatorTMP>().Destroy();
        headerText.GetComponent<TextMeshPro>().text = "ID";
        IdIcon.GetComponent<SpriteRenderer>().color = Color.magenta;
        HostIcon = Object.Instantiate(HudManager.Instance.MeetingPrefab.PlayerButtonPrefab.LevelNumberText.transform.parent, transform).gameObject;
        var text = HostIcon.transform.GetChild(1);
        text.GetComponent<TextMeshPro>().text = "★";
        text.transform.localPosition = Vector3.zero;
        var headerText2 = HostIcon.transform.GetChild(0);
        headerText2.GetComponent<TextTranslatorTMP>().Destroy();
        headerText2.GetComponent<TextMeshPro>().text = "";
        HostIcon.GetComponent<SpriteRenderer>().color = new Color(0.5f, 0, 1, 1);
        PlatformIcon = Object.Instantiate(HudManager.Instance.MeetingPrefab.PlayerButtonPrefab.LevelNumberText.transform.parent, transform).gameObject;
        PlatformIcon.transform.GetChild(0).gameObject.SetActive(false);
        PlatformIcon.transform.GetChild(1).gameObject.SetActive(false);
        PlatformIcon.GetComponent<SpriteRenderer>().color = new Color(0.5f, 0.5f, 1, 1);
        var identifier = Object.Instantiate(FriendsListManager.Instance.UiPrefab.LobbyPlayerBar.PlatformIdentifier, PlatformIcon.transform);
        identifier.transform.localPosition = new  Vector3(0.45f, 0, -1);
        identifier.SetInfo(AmongUsClient.Instance.GetClientFromCharacter(owner));
        identifier.iconRenderer.color = Color.white;
        owner.StartCoroutine(Effects.ActionAfterDelay(0.05f, new System.Action(() =>
            identifier.gameObject.transform.localScale = new Vector3(1.5f, 1.5f, 1))));
        
        var shader = Shader.Find("Sprites/Default");
        foreach (var rend in identifier.GetComponentsInChildren<SpriteRenderer>())
        {
            rend.material = new Material(shader);
        }
        identifier.transform.localScale = HostIcon.transform.localScale;

        foreach (var go in GetComponentsInChildren<Transform>())
        {
            go.gameObject.layer = 0;
        }
        
        IdIcon.SetActive(PluginSingleton<HostUtilityPlugin>.Instance.ShowPlayerIDs.Value);
        PlatformIcon.SetActive(PluginSingleton<HostUtilityPlugin>.Instance.ShowPlayerPlatforms.Value); 
        HostIcon.SetActive(AmongUsClient.Instance.GetHost().Character == owner);
    }

    private void FixedUpdate() => CheckCurrentChildren();

    private void CheckCurrentChildren()
    {
        GetChildsActive();
        if (cells.SequenceEqual(currentChildren))
            return;
        cells.Clear();
        foreach (Transform currentChild in currentChildren)
            cells.Add(currentChild);
        ArrangeChilds();
    }

    private void GetChildsActive()
    {
        currentChildren.Clear();
        foreach (var obj in transform)
        {
            var child = obj.TryCast<Transform>();
            if (child == null) continue;
            if (child.gameObject.activeSelf)
                currentChildren.Add(child);
        }
    }

    private void ArrangeChilds()
    {
        if (cells.Count == 0)
            return;

        int totalRows = Mathf.CeilToInt((float)cells.Count / MaxColumns);
        float totalHeight = (totalRows - 1) * CellSize.y;
        float startY = transform.position.y + totalHeight * 0.5f;

        for (int index = 0; index < cells.Count; ++index)
        {
            int row = index / MaxColumns;
            int rowStartIndex = row * MaxColumns;
            int itemsInRow = Mathf.Min(MaxColumns, cells.Count - rowStartIndex);
            int col = index - rowStartIndex;

            float rowWidth = (itemsInRow - 1) * CellSize.x;
            float startX = transform.position.x - rowWidth * 0.5f;

            float x = startX + col * CellSize.x;
            float y = startY - row * CellSize.y;

            Transform cell = cells[index];
            cell.position = new Vector3(x, y, cell.position.z);
        }
    }
}