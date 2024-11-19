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

    // Added talkId to connect criteria with a unique dialogue ID
    public int talkId;  // Unique identifier for conversation-based criteria

    // A flag for Talk criteria to ensure only one is completed at a time
    public bool isTalkCompleted;  // Used to track Talk criteria completion
}
