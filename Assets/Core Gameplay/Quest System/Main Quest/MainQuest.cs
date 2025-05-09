using System.Collections.Generic;
using UnityEngine;

public class MainQuest : MonoBehaviour
{
    // Quest Details
    public string questName;
    public string questDescription;
    public string readingQuestion;
    public string readingSkill;
    public QuestEnums.QuestStatus status;
    public List<QuestCriteria> questCriteriaList = new List<QuestCriteria>();

    // Add Criteria to the quest
    public void AddCriteria(string name, QuestEnums.QuestCriteriaType type, GameObject spawnZone, int priority, string criteriaContext)
    {
        QuestCriteria newCriteria = new QuestCriteria
        {
            criteriaName = name,
            criteriaType = type,
            spawnZone = spawnZone,
            priority = priority,
            CriteriaStatus = QuestEnums.QuestCriteriaStatus.NotStarted,
            criteriaContext = criteriaContext
        };

        // Add the new criteria to the list
        questCriteriaList.Add(newCriteria);
    }

    // Update the criteria status
    public void UpdateCriteriaStatus(string criteriaName, QuestEnums.QuestCriteriaStatus status)
    {
        var criteria = questCriteriaList.Find(c => c.criteriaName == criteriaName);
        if (criteria != null)
        {
            criteria.CriteriaStatus = status;
            if (status == QuestEnums.QuestCriteriaStatus.Completed)
            {
                criteria.CompleteCriteria();  // This will trigger the UnityEvent
            }
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
                UnityEngine.Debug.Log("Context: " + criteria.criteriaContext);  // Log the criteria context as well
            }
        }

        if (!activeCriteriaFound)
        {
            UnityEngine.Debug.Log("No tasks left, this main quest is completed!");
        }
    }
}
