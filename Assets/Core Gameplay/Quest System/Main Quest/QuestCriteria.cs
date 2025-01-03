using UnityEngine;

[System.Serializable]
public class QuestCriteria
{
    public string criteriaName;
    public QuestEnums.QuestCriteriaType criteriaType;
    public int priority;  // Higher number = lower priority
    public QuestEnums.QuestCriteriaStatus CriteriaStatus;  // Use the new enum here

    // Add a reference to QuestObject
    public QuestObject associatedQuestObject;  // Reference to the QuestObject

    // Add any criteria-specific data, such as spawn zones, NPC references, etc.
    public GameObject spawnZone;  // Example for Find/Explore criteria

   
}
