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

    // Add a criteria with priority handling
    public void AddCriteria(string name, QuestEnums.QuestCriteriaType type, GameObject spawnZone, int priority)
    {
        QuestCriteria newCriteria = new QuestCriteria
        {
            criteriaName = name,
            criteriaType = type,
            spawnZone = spawnZone,
            priority = priority,
            status = QuestEnums.QuestStatus.NotStarted
        };

        questCriteriaList.Add(newCriteria);
        UnityEngine.Debug.Log("Added criteria: " + name + " to quest: " + questName);
        SortCriteriaByPriority();  // Sort criteria by priority after adding
    }

    // Sort the criteria list by priority (ascending order)
    public void SortCriteriaByPriority()
    {
        questCriteriaList.Sort((criteria1, criteria2) => criteria1.priority.CompareTo(criteria2.priority));
    }

    // Log active criteria
    public void LogActiveCriteria()
    {
        bool activeCriteriaFound = false;

        UnityEngine.Debug.Log("Active criteria for quest: " + questName);
        foreach (var criteria in questCriteriaList)
        {
            if (criteria.status != QuestEnums.QuestStatus.Completed)
            {
                activeCriteriaFound = true;
                UnityEngine.Debug.Log("Active criteria: " + criteria.criteriaName + " (Priority: " + criteria.priority + ")");
            }
        }

        if (!activeCriteriaFound)
        {
            UnityEngine.Debug.Log("No active criteria left, quest is completed!");
        }
    }
}
