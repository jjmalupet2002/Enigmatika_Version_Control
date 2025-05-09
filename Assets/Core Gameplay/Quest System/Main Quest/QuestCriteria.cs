using UnityEngine;
using System;
using UnityEngine.Events;

[System.Serializable]
public class QuestCriteria
{
    public string criteriaName;
    public QuestEnums.QuestCriteriaType criteriaType;
    public int priority;  // Higher number = lower priority
    public QuestEnums.QuestCriteriaStatus CriteriaStatus;  // Use the new enum here

    // Add a reference to QuestObject
    public QuestObject associatedQuestObject;  // Reference to the QuestObject
    public string criteriaContext;

    public GameObject spawnZone;  // Example for Find/Explore criteria

    // Use UnityEvent for the completion event
    public UnityEvent OnCriteriaCompleted;

    public void CompleteCriteria()
    {
        // Invoke any UnityEvent actions assigned in the Inspector for this specific criteria
        if (OnCriteriaCompleted != null)
        {
            OnCriteriaCompleted.Invoke();
        }
    }
}

