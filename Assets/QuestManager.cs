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
                criteria.CriteriaStatus = QuestEnums.QuestCriteriaStatus.NotStarted;
            }

            // Ensure highest priority is set to InProgress (moved to QuestManager)
            SetHighestPriorityCriteriaInProgress(quest);
        }
        else
        {
            UnityEngine.Debug.Log($"Quest {quest.questName} is already initialized in activeQuests.");
        }
    }

    // Set the highest priority criteria to InProgress
    public void SetHighestPriorityCriteriaInProgress(MainQuest quest)
    {
        // Sort the criteria list by priority
        quest.questCriteriaList.Sort((a, b) => a.priority.CompareTo(b.priority)); // Sort by priority ascending

        // Set the first (highest priority) criteria to InProgress
        if (quest.questCriteriaList.Count > 0)
        {
            quest.questCriteriaList[0].CriteriaStatus = QuestEnums.QuestCriteriaStatus.InProgress;
        }
    }

    // Set the next active criteria
    public void SetNextActiveCriteria(MainQuest quest, int currentIndex)
    {
        // Look for the next criteria that is NotStarted
        for (int i = currentIndex + 1; i < quest.questCriteriaList.Count; i++)
        {
            var nextCriteria = quest.questCriteriaList[i];
            UnityEngine.Debug.Log($"Checking next criteria: {nextCriteria.criteriaName}, Status: {nextCriteria.CriteriaStatus}, Priority: {nextCriteria.priority}");

            if (nextCriteria.CriteriaStatus == QuestEnums.QuestCriteriaStatus.NotStarted)
            {
                nextCriteria.CriteriaStatus = QuestEnums.QuestCriteriaStatus.InProgress;
                UnityEngine.Debug.Log($"Next criteria {nextCriteria.criteriaName} is now in progress.");
                return; // Exit once the next criteria is found and set to InProgress
            }
        }

        // If no next criteria are found, log that the quest is completed
        UnityEngine.Debug.Log("No next criteria found, quest completed.");
    }

    // Complete a quest and log the next active criteria
    public void CompleteQuest(MainQuest quest)
    {
        if (quest.status == QuestEnums.QuestStatus.InProgress)
        {
            // Ensure the criteria are sorted by priority before processing
            quest.questCriteriaList.Sort((a, b) => a.priority.CompareTo(b.priority));

            bool allCriteriaCompleted = true;

            for (int i = 0; i < quest.questCriteriaList.Count; i++)
            {
                var criteria = quest.questCriteriaList[i];

                if (criteria.CriteriaStatus == QuestEnums.QuestCriteriaStatus.InProgress)
                {
                    criteria.CriteriaStatus = QuestEnums.QuestCriteriaStatus.Completed;
                    UnityEngine.Debug.Log($"Criteria {criteria.criteriaName} has been completed.");

                    // Set the next available criteria to InProgress
                    SetNextActiveCriteria(quest, i);

                    // Check if there are any remaining criteria that aren't completed
                    for (int j = i + 1; j < quest.questCriteriaList.Count; j++)
                    {
                        if (quest.questCriteriaList[j].CriteriaStatus != QuestEnums.QuestCriteriaStatus.Completed)
                        {
                            allCriteriaCompleted = false;
                            break;
                        }
                    }

                    break; // Exit the loop once we've processed the current InProgress criteria
                }
            }

            if (allCriteriaCompleted)
            {
                quest.status = QuestEnums.QuestStatus.Completed;
                UnityEngine.Debug.Log($"Main Quest {quest.questName} Completed!");
                OnQuestCompleted(quest);
            }

            LogActiveCriteria(quest);
        }
    }


    // Log the current active criteria
    public void LogActiveCriteria(MainQuest quest)
    {
        if (quest.questCriteriaList.Count > 0)
        {
            foreach (var criteria in quest.questCriteriaList)
            {
                if (criteria.CriteriaStatus == QuestEnums.QuestCriteriaStatus.InProgress)
                {
                    UnityEngine.Debug.Log($"Active Criteria - Name: {criteria.criteriaName}, Status: {criteria.CriteriaStatus}, Priority: {criteria.priority}");
                    break;
                }
            }
        }
    }

    // Check quest completion
    public void CheckQuestCompletion(MainQuest mainQuest)
    {
        // Check each criteria for completion
        foreach (var criteria in mainQuest.questCriteriaList)
        {
            if (criteria.CriteriaStatus != QuestEnums.QuestCriteriaStatus.Completed)
            {
                UnityEngine.Debug.Log($"Task completed!");
                UnityEngine.Debug.Log("Next task: " + criteria.criteriaName);
                return;  // Return early if any criteria are not complete
            }


        }

        // All criteria completed
        UnityEngine.Debug.Log("Main quest completed: " + mainQuest.questName);
        mainQuest.status = QuestEnums.QuestStatus.Completed;
    }


    public Dictionary<string, MainQuest> GetActiveQuests()
    {
        return activeQuests;
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



  