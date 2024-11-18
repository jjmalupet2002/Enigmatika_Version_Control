using System.Collections.Generic;
using UnityEngine;

public class QuestManager : MonoBehaviour
{
    public List<MainQuest> mainQuestObjects;
    public Dictionary<string, MainQuest> activeQuests = new Dictionary<string, MainQuest>();


    public void AcceptQuest(MainQuest quest)
    {

        // Only initialize if the quest has not been initialized already
        if (quest.status == QuestEnums.QuestStatus.NotStarted)
        {
            InitializeQuest(quest);

            quest.status = QuestEnums.QuestStatus.InProgress;
            OnQuestAccepted(quest);
            LogActiveCriteria(quest);  // Log the active criteria after quest acceptance
        }
        else
        {
            UnityEngine.Debug.Log($"Quest {quest.questName} is already in progress or completed.");
        }
    }



    // Initialize the quest and its criteria
    void InitializeQuest(MainQuest quest)
    {
        if (!activeQuests.ContainsKey(quest.questName))
        {
            activeQuests.Add(quest.questName, quest);
            UnityEngine.Debug.Log($"Main Quest Started: {quest.questName}");

            // Ensure status is set to NotStarted
            quest.status = QuestEnums.QuestStatus.NotStarted;

            // Set all criteria to NotStarted
            foreach (var criteria in quest.questCriteriaList)
            {
                criteria.status = QuestEnums.QuestStatus.NotStarted;
            }

           
        }
        else
        {
            UnityEngine.Debug.Log($"Quest {quest.questName} is already initialized in activeQuests.");
        }
    }


    // Complete a quest and log the next active criteria
    public void CompleteQuest(MainQuest quest)
    {
        if (quest.status == QuestEnums.QuestStatus.InProgress)
        {
            bool questCompleted = true;

            // Iterate through all criteria to check and update their status
            foreach (var criteria in quest.questCriteriaList)
            {
                if (criteria.status == QuestEnums.QuestStatus.NotStarted)
                {
                    criteria.status = QuestEnums.QuestStatus.InProgress;
                    questCompleted = false;
                    break;  // Break only if we found an unstarted criteria
                }
                else if (criteria.status == QuestEnums.QuestStatus.InProgress)
                {
                    UnityEngine.Debug.Log($"Criteria {criteria.criteriaName} is already in progress.");
                }
                else if (criteria.status == QuestEnums.QuestStatus.Completed)
                {
                    // Log that the criteria has been completed
                    UnityEngine.Debug.Log($"Criteria {criteria.criteriaName} has been completed for quest: {quest.questName}");
                }
            }

            // If all criteria are completed, mark the quest as completed
            if (questCompleted)
            {
                quest.status = QuestEnums.QuestStatus.Completed;
                UnityEngine.Debug.Log($"Main Quest Completed: {quest.questName}");
                OnQuestCompleted(quest);
            }

            // Log the next active criteria if quest is not completed
            LogActiveCriteria(quest);
        }
    }



    // Log the current active criteria
    public void LogActiveCriteria(MainQuest quest)
    {
        if (quest.questCriteriaList.Count > 0)
        {
            // Sort criteria by priority (ascending)
            quest.SortCriteriaByPriority();

            // Log the first (highest priority) active criteria
            foreach (var criteria in quest.questCriteriaList)
            {
                if (criteria.status == QuestEnums.QuestStatus.NotStarted)
                {
                    UnityEngine.Debug.Log($"Active criteria: {criteria.criteriaName} (Priority: {criteria.priority}) for quest: {quest.questName}");
                    break;  // Only log the highest priority (first in sorted list)
                }
            }
        }
    }

    public Dictionary<string, MainQuest> GetActiveQuests()
    {
        return activeQuests;
    }

    // Check quest completion
    public void CheckQuestCompletion(MainQuest quest)
    {
        bool allCompleted = true;
        foreach (var criteria in quest.questCriteriaList)
        {
            if (criteria.status != QuestEnums.QuestStatus.Completed)
            {
                allCompleted = false;
                break;
            }
        }

        if (allCompleted)
        {
            quest.status = QuestEnums.QuestStatus.Completed;
            UnityEngine.Debug.Log("Main Quest Completed: " + quest.questName);
        }
    }

    // Optional event to handle quest acceptance
    void OnQuestAccepted(MainQuest quest)
    {
        // Implement logic for accepting quests here, such as showing UI or activating NPC dialogue
    }

    // Optional event to handle quest completion
    void OnQuestCompleted(MainQuest quest)
    {
        // Implement logic for completing quests here, such as giving rewards
    }
}