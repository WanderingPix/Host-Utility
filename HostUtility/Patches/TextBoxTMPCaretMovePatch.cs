using HarmonyLib;
using UnityEngine;

namespace HostUtility.Patches;

[HarmonyPatch(typeof(TextBoxTMP), nameof(TextBoxTMP.MoveCaret))]
static class TextBoxTMPMoveCaretPatch
{
    static void Postfix(TextBoxTMP __instance)
    {
        bool up = Input.GetKeyDown(KeyCode.UpArrow);
        bool down = Input.GetKeyDown(KeyCode.DownArrow);
        if (!up && !down) return;

        var tb = __instance;
        if (string.IsNullOrEmpty(tb.text)) return;

        tb.outputText.ForceMeshUpdate();
        var info = tb.outputText.textInfo;
        int count = Mathf.Min(tb.text.Length, info.characterCount);
        if (count == 0) return;

        int pos = Mathf.Clamp(tb.caretPos, 0, count);
        var cur = info.characterInfo[Mathf.Min(pos, count - 1)];

        float x = pos < count ? cur.origin : cur.xAdvance;

        int targetLine = cur.lineNumber + (down ? 1 : -1);
        if (targetLine < 0 || targetLine >= info.lineCount) return;

        var line = info.lineInfo[targetLine];
        int first = line.firstCharacterIndex;
        int last = Mathf.Min(line.lastCharacterIndex, count - 1);

        int best = first;
        float bestDist = float.MaxValue;
        for (int i = first; i <= last; i++)
        {
            float d = Mathf.Abs(info.characterInfo[i].origin - x);
            if (d < bestDist) { bestDist = d; best = i; }
        }

        if (targetLine == info.lineCount - 1 &&
            Mathf.Abs(info.characterInfo[count - 1].xAdvance - x) < bestDist)
            best = count;

        tb.caretPos = best;
        tb.pipeBlinkTimer = 0f;

        if (tb.Pipe)
            tb.Pipe.transform.localPosition = (Vector3)tb.outputText.CursorPos(tb.caretPos, tb.caretYOffset);
    }
}