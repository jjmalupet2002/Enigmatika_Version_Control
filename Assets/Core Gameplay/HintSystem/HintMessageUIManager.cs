using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;
using System.Diagnostics;

public class HintMessageUIManager : MonoBehaviour
{
    public Button hintButton; // Reference to the hint button
    public Button exitButton; // Reference to the exit button
    public GameObject hintMessageUI; // Reference to the hint message UI game object
    public Text hintMessageText; // Text component to display the hint message

    private Dictionary<string, string> hintMessages = new Dictionary<string, string>(); // Private dictionary for context-specific hints
    private string currentContext = ""; // Tracks the current gameplay context

    private void OnEnable()
    {
        hintButton.onClick.AddListener(DisplayHintMessage);
        exitButton.onClick.AddListener(HideHintMessage);
    }

    private void OnDisable()
    {
        hintButton.onClick.RemoveListener(DisplayHintMessage);
        exitButton.onClick.RemoveListener(HideHintMessage);
    }

    /// <summary>
    /// Registers or updates a hint message for a specific context.
    /// </summary>
    public void RegisterHintMessage(string context, string hintMessage)
    {
        if (string.IsNullOrWhiteSpace(context) || string.IsNullOrWhiteSpace(hintMessage))
        {
            UnityEngine.Debug.LogError("Context or hint message cannot be null or empty.");
            return;
        }

        if (!hintMessages.ContainsKey(context))
        {
            hintMessages.Add(context, hintMessage);
            UnityEngine.Debug.Log($"Hint message for '{context}' added.");
        }
        else
        {
            hintMessages[context] = hintMessage; // Update existing hint
            UnityEngine.Debug.Log($"Hint message for '{context}' updated.");
        }
    }

    /// <summary>
    /// Sets the current context for hint display.
    /// </summary>
    public void SetCurrentContext(string context)
    {
        if (hintMessages.ContainsKey(context))
        {
            currentContext = context;
            UnityEngine.Debug.Log($"Current context set to '{context}'.");
        }
        else
        {
            UnityEngine.Debug.LogWarning($"No hint message registered for context '{context}'.");
            currentContext = "";
        }
    }

    /// <summary>
    /// Displays the hint message for the current context.
    /// </summary>
    public void DisplayHintMessage()
    {
        if (hintMessages.TryGetValue(currentContext, out string hintMessage))
        {
            hintMessageText.text = hintMessage; // Update UI text with the appropriate hint
            hintMessageUI.SetActive(true); // Show the hint message UI
        }
        else
        {
            hintMessageText.text = "No hints available for this context."; // Fallback message
            hintMessageUI.SetActive(true);
        }
    }

    /// <summary>
    /// Hides the hint message UI.
    /// </summary>
    private void HideHintMessage()
    {
        hintMessageUI.SetActive(false); // Deactivate the hint message UI
    }
}
