using UnityEngine;

[System.Serializable]
public class QuestCriteria
{
    public string criteriaName;
    public QuestEnums.QuestCriteriaType criteriaType;
    public QuestEnums.QuestStatus status;
    public int priority;  // Higher number = lower priority

    // Add a reference to QuestObject
    public QuestObject associatedQuestObject;  // Reference to the QuestObject

    // Add any criteria-specific data, such as spawn zones, NPC references, etc.
    public GameObject spawnZone;  // Example for Find/Explore criteria

    public string conversationName;  // The name of the conversation
    public NPCInteractable npcConversationStarter;  // Reference to the NPCConversationStarter component
}
