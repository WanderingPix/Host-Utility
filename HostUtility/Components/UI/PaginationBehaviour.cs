using System;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

namespace HostUtility.Components.UI;

public class PaginationBehaviour(IntPtr ptr) : MonoBehaviour(ptr)
{
    public List<GameObject> pages = new();
    public TextMeshPro text;
    public int currentPage;
    public void NextPage()
    {
        currentPage++;
        HideAll();
        if (currentPage > pages.Count)
        {
            currentPage = 0;
        }
        pages[currentPage].SetActive(true);
        UpdateText();
    }

    public void PreviousPage()
    {
        currentPage--;
        HideAll();
        if (currentPage < 0)
        {
            currentPage = pages.Count - 1;
        }
        pages[currentPage].SetActive(true);
        UpdateText();
    }

    private void HideAll()
    {
        foreach (var page in pages)
        {
            page.SetActive(false);
        }
    }

    private void UpdateText()
    {
        text.text = $"{currentPage + 1} / {pages.Count}";
    }
}