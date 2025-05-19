using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class TextStyler : MonoBehaviour
{
    [Tooltip("Legacy UI Text components to apply borders to.")]
    public List<Text> texts;

    // Border color and thickness can be configured here
    public Color borderColor = Color.black;
    public float borderThickness = 1f;

    // Call this to apply borders to all texts in the list
    public void ApplyBorders()
    {
        foreach (Text text in texts)
        {
            if (text == null) continue;

            // Remove existing Shadow components added by this script before adding new ones
            RemoveOldShadows(text);

            // Add shadows in 4 directions for a border effect
            AddShadow(text, new Vector2(-borderThickness, borderThickness));  // top-left
            AddShadow(text, new Vector2(borderThickness, borderThickness));   // top-right
            AddShadow(text, new Vector2(-borderThickness, -borderThickness)); // bottom-left
            AddShadow(text, new Vector2(borderThickness, -borderThickness));  // bottom-right
        }
    }

    // Helper: Remove shadows previously added by this script
    private void RemoveOldShadows(Text text)
    {
        Shadow[] shadows = text.GetComponents<Shadow>();
        foreach (Shadow shadow in shadows)
        {
            // To keep the original shadow if it exists, only remove shadows with color == borderColor
            // Or just remove all shadows except one? Here, removing all shadows to avoid duplicates
            DestroyImmediate(shadow);
        }
    }

    // Helper: Add a Shadow component with specific effect distance and color
    private void AddShadow(Text text, Vector2 effectDistance)
    {
        Shadow shadow = text.gameObject.AddComponent<Shadow>();
        shadow.effectColor = borderColor;
        shadow.effectDistance = effectDistance;
        shadow.useGraphicAlpha = true;
    }
}
