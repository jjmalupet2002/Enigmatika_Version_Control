using System.Collections.Generic;
using UnityEngine;

public class MainQuest : MonoBehaviour
{
    // Quest Details
    public string questName;
    public string questDescription;
    public GameObject questReward;
    public GameObject questGiver;
    public QuestEnums.QuestStatus status;
    public List<QuestCriteria> questCriteriaList = new List<QuestCriteria>();

    // Add Criteria to the quest
    public void AddCriteria(string name, QuestEnums.QuestCriteriaType type, GameObject spawnZone, int priority)
    {
        QuestCriteria newCriteria = new QuestCriteria
        {
            criteriaName = name,
            criteriaType = type,
            spawnZone = spawnZone,
            priority = priority,
            CriteriaStatus = QuestEnums.QuestCriteriaStatus.NotStarted,
        };

        questCriteriaList.Add(newCriteria);
    }

    // Update the criteria status
    public void UpdateCriteriaStatus(string criteriaName, QuestEnums.QuestCriteriaStatus status)
    {
        var criteria = questCriteriaList.Find(c => c.criteriaName == criteriaName);
        if (criteria != null)
        {
            criteria.CriteriaStatus = status;
        }
    }

    // Set the overall quest status
    public void SetQuestStatus(QuestEnums.QuestStatus newStatus)
    {
        status = newStatus;
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
