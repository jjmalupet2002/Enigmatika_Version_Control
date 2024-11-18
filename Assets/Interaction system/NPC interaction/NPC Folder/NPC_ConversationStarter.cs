using System.Collections.Generic;
using DialogueEditor;
using UnityEngine;

public class NPCInteractable : MonoBehaviour
{
    [SerializeField] private NPCConversation Activeconversation;
    [SerializeField] private string npcName;

    // List to store multiple conversations in the Inspector
    [SerializeField] private List<ConversationEntry> conversationEntries = new List<ConversationEntry>();

    // Dictionary to store multiple conversations (not serialized)
    private Dictionary<string, NPCConversation> conversations = new Dictionary<string, NPCConversation>();

    private void Awake()
    {
        // Populate the dictionary from the list for easy access by conversation name
        foreach (var entry in conversationEntries)
        {
            if (!conversations.ContainsKey(entry.conversationName))
            {
                conversations.Add(entry.conversationName, entry.conversation);
            }
        }
    }

    // Existing method to start the default conversation
    public void Interact()
    {
        if (Activeconversation != null)
        {
            ConversationManager.Instance.StartConversation(Activeconversation);
        }
        else
        {
            UnityEngine.Debug.LogError($"{npcName} conversation is not assigned.");
        }
    }

    // Method to add conversations to the NPC
    public void AddConversation(string conversationName, NPCConversation newConversation)
    {
        if (!conversations.ContainsKey(conversationName))
        {
            conversations.Add(conversationName, newConversation);
            conversationEntries.Add(new ConversationEntry { conversationName = conversationName, conversation = newConversation });
        }
        else
        {
            UnityEngine.Debug.LogWarning($"{npcName} already has a conversation named {conversationName}");
        }
    }

    // Method to start a specific conversation by name
    public void StartConversation(string conversationName)
    {
        if (conversations.TryGetValue(conversationName, out NPCConversation conversationToStart))
        {
            ConversationManager.Instance.StartConversation(conversationToStart);
        }
        else
        {
            UnityEngine.Debug.LogError($"{npcName} does not have a conversation named {conversationName}");
        }
    }

    // Method to set the active conversation dynamically
    public void SetActiveConversation(string conversationName)
    {
        if (conversations.TryGetValue(conversationName, out NPCConversation selectedConversation))
        {
            Activeconversation = selectedConversation;
            UnityEngine.Debug.Log($"Active conversation set to {conversationName} for {npcName}");
        }
        else
        {
            UnityEngine.Debug.LogError($"{npcName} does not have a conversation named {conversationName}");
        }
    }
}
