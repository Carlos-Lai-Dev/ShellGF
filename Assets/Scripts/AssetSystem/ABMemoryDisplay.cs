using System.Linq;
using UnityEngine;

public class ABMemoryDisplay : MonoBehaviour
{
    public bool showOnScreen = true;
    public Rect displayRect = new Rect(10, 10, 300, 200);
    public Color textColor = Color.white;
    public int fontSize = 14;

    void OnGUI()
    {
        if (!showOnScreen) return;

        GUIStyle style = new GUIStyle();
        style.normal.textColor = textColor;
        style.fontSize = fontSize;

        GUI.Label(displayRect, GetMemoryInfo(), style);
    }

    private string GetMemoryInfo()
    {
        System.Text.StringBuilder sb = new System.Text.StringBuilder();
        sb.AppendLine("== AssetBundle Memory ==");

        long total = 0;
        foreach (var bundle in ABMemoryTracker._loadedBundles.Values
            .OrderByDescending(b => b.memorySize))
        {
            sb.AppendLine($"{bundle.bundleName}: {ExtensionMethods.FormatBytes(bundle.memorySize)} (Ref: {bundle.referenceCount})");
            total += bundle.memorySize;
        }

        sb.AppendLine($"Total: {ExtensionMethods.FormatBytes(total)}");
        sb.AppendLine($"Loaded Bundles: {ABMemoryTracker._loadedBundles.Count}");

        return sb.ToString();
    }


}
