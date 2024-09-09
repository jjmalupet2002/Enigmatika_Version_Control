using DialogueEditor;
using UnityEngine;

public class NPCInteractable : MonoBehaviour
{
    [SerializeField] private NPCConversation conversation;
    [SerializeField] private string npcName;

    public void Interact()
    {
        // Start the conversation using the Dialogue Editor's ConversationManager
        if (conversation != null)
        {
            ConversationManager.Instance.StartConversation(conversation);
        }
        else
        {
            Debug.LogError($"{npcName} conversation is not assigned.");
        }
    }
}
