using System.Collections.Generic;
using UnityEngine;

public class MainQuest : MonoBehaviour
{
    public string questName;
    public string questDescription;
    public List<QuestCriteria> questCriteriaList = new List<QuestCriteria>();
    public GameObject questReward;
    public GameObject questGiver;
    public QuestEnums.QuestStatus status;

    // Modified AddCriteria to accept talkId as a parameter
    public void AddCriteria(string name, QuestEnums.QuestCriteriaType type, GameObject spawnZone, int priority)
    {
        QuestCriteria newCriteria = new QuestCriteria
        {
            criteriaName = name,
            criteriaType = type,
            spawnZone = spawnZone,
            priority = priority,
            status = QuestEnums.QuestStatus.NotStarted,
            
        };

        questCriteriaList.Add(newCriteria);
        UnityEngine.Debug.Log("Added criteria: " + name + " to quest: " + questName);
        SortCriteriaByPriority();  // Sort criteria by priority after adding
    }


    public void SortCriteriaByPriority()
    {
        questCriteriaList.Sort((criteria1, criteria2) => criteria1.priority.CompareTo(criteria2.priority));
    }

    public void LogActiveCriteria()
    {
        bool activeCriteriaFound = false;

        UnityEngine.Debug.Log("Remaining tasks for: " + questName);
        foreach (var criteria in questCriteriaList)
        {
            if (criteria.status != QuestEnums.QuestStatus.Completed)
            {
                activeCriteriaFound = true;
                UnityEngine.Debug.Log("Task: " + criteria.criteriaName + " (Priority: " + criteria.priority + ")");
            }
        }

        if (!activeCriteriaFound)
        {
            UnityEngine.Debug.Log("No tasks left, this main quest is completed!");
        }
    }

    // Ensures that only the highest priority is InProgress
    public void SetHighestPriorityCriteriaInProgress()
    {
        SortCriteriaByPriority();
        foreach (var criteria in questCriteriaList)
        {
            if (criteria.status == QuestEnums.QuestStatus.NotStarted)
            {
                criteria.status = QuestEnums.QuestStatus.InProgress;
                break;  // Only activate the first NotStarted criteria (highest priority)
            }
        }
    }
}
