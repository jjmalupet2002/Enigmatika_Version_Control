using System.Collections.Generic;
using UnityEngine;

public class MainQuest : MonoBehaviour
{
    public string questName;
    public string questDescription;
    public GameObject questReward;
    public GameObject questGiver;
    public QuestEnums.QuestStatus status;  // Main quest status (QuestStatus)
    public List<QuestCriteria> questCriteriaList = new List<QuestCriteria>();

    // Modified AddCriteria to accept talkId as a parameter
    public void AddCriteria(string name, QuestEnums.QuestCriteriaType type, GameObject spawnZone, int priority)
    {
        QuestCriteria newCriteria = new QuestCriteria
        {
            criteriaName = name,
            criteriaType = type,
            spawnZone = spawnZone,
            priority = priority,
            CriteriaStatus = QuestEnums.QuestCriteriaStatus.NotStarted,  // Criteria status
        };

        questCriteriaList.Add(newCriteria);
    }

    public void LogActiveCriteria()
    {
        bool activeCriteriaFound = false;

        UnityEngine.Debug.Log("Remaining tasks for: " + questName);
        foreach (var criteria in questCriteriaList)
        {
            if (criteria.CriteriaStatus != QuestEnums.QuestCriteriaStatus.Completed)
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
}