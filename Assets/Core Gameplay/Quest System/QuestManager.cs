using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using UnityEngine;
using Save;
using CarterGames.Assets.SaveManager;

public class QuestManager : MonoBehaviour
{
    // Declare events for quest acceptance and completion
    public delegate void QuestEventHandler(MainQuest quest);
    public event QuestEventHandler OnQuestAcceptedEvent;
    public event QuestEventHandler OnNextCriteriaStartedEvent;
    public event QuestEventHandler OnQuestCompletedEvent;

    public List<MainQuest> mainQuestObjects;
    public Dictionary<string, MainQuest> activeQuests = new Dictionary<string, MainQuest>();
    public QuestSavingSaveObject QuestSaveObject;
    public QuestUIManager questUIManager;


    private void OnEnable()
    {
        SaveEvents.OnSaveGame += SaveQuest;
        SaveEvents.OnLoadGame += LoadQuest;
    }

    private void OnDisable()
    {
        SaveEvents.OnSaveGame -= SaveQuest;
        SaveEvents.OnLoadGame -= LoadQuest;
    }

    public void AcceptQuest(MainQuest quest)
    {
        // Only initialize if the quest has not been initialized already
        if (quest.status == QuestEnums.QuestStatus.NotStarted)
        {
            InitializeQuest(quest);

            quest.status = QuestEnums.QuestStatus.InProgress;
            // Trigger the OnQuestAcceptedEvent here
            OnQuestAccepted(quest);
            OnQuestAcceptedEvent?.Invoke(quest); // Notify listeners
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
                criteria.associatedQuestObject.gameObject.SetActive(false); // Disable the associated quest object initially
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
            var highestPriorityCriteria = quest.questCriteriaList[0];
            highestPriorityCriteria.CriteriaStatus = QuestEnums.QuestCriteriaStatus.InProgress;

            // Enable the associated quest object
            if (highestPriorityCriteria.associatedQuestObject != null)
            {
                highestPriorityCriteria.associatedQuestObject.gameObject.SetActive(true);
            }
        }
    }


    // Coroutine to disable the quest object after a delay
    private IEnumerator DisableAfterDelay(GameObject questObject, float delay)
    {
        yield return new WaitForSeconds(delay);
        questObject.SetActive(false);
    }

    // Set the next active criteria
    public void SetNextActiveCriteria(MainQuest quest, int currentIndex)
    {
        // Disable the associated quest object of the completed criteria
        if (currentIndex >= 0 && currentIndex < quest.questCriteriaList.Count)
        {
            var completedCriteria = quest.questCriteriaList[currentIndex];
            if (completedCriteria.CriteriaStatus == QuestEnums.QuestCriteriaStatus.Completed)
            {
                // Only disable the associated quest object for the "Talk" quest criteria type
                if (completedCriteria.associatedQuestObject != null &&
                    completedCriteria.criteriaType == QuestEnums.QuestCriteriaType.Talk)
                {
                    // Assuming QuestObject has a gameObject property, pass it to the coroutine
                    StartCoroutine(DisableAfterDelay(completedCriteria.associatedQuestObject.gameObject, 30f));
                }
            }
        }

        // Look for the next criteria that is NotStarted
        for (int i = currentIndex + 1; i < quest.questCriteriaList.Count; i++)
        {
            var nextCriteria = quest.questCriteriaList[i];

            if (nextCriteria.CriteriaStatus == QuestEnums.QuestCriteriaStatus.NotStarted)
            {
                nextCriteria.CriteriaStatus = QuestEnums.QuestCriteriaStatus.InProgress;
                nextCriteria.associatedQuestObject.gameObject.SetActive(true); // Enable the associated quest object
                UnityEngine.Debug.Log($"Next Task: {nextCriteria.criteriaName}");
                // Trigger the OnQuestAcceptedEvent here
                OnNextCriteriaStarted(quest);
                OnNextCriteriaStartedEvent?.Invoke(quest); // Notify listeners
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

            // Loop through the quest criteria
            for (int i = 0; i < quest.questCriteriaList.Count; i++)
            {
                var criteria = quest.questCriteriaList[i];

                // Check if any criteria are still in progress
                if (criteria.CriteriaStatus == QuestEnums.QuestCriteriaStatus.InProgress)
                {
                    // If found, mark as completed and continue to next criteria
                    criteria.CriteriaStatus = QuestEnums.QuestCriteriaStatus.Completed;
                                        

                    // Set the next available criteria to InProgress
                    SetNextActiveCriteria(quest, i);

                    // If any criteria are not completed, set the flag to false and break
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

            // Check if all criteria are completed
            if (allCriteriaCompleted)
            {
                quest.status = QuestEnums.QuestStatus.Completed;
                UnityEngine.Debug.Log($"Main Quest {quest.questName} Completed!");
                // Trigger the OnQuestCompletedEvent here
                OnQuestCompleted(quest);
                OnQuestCompletedEvent?.Invoke(quest); // Notify listeners
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

    public Dictionary<string, MainQuest> GetActiveQuests()
    {
        return activeQuests;
    }

    private void SaveQuest()
    {
        // Save active quest names
        QuestSaveObject.activeQuestNames.Value = new List<string>(activeQuests.Keys);

        // Save quest statuses, criteria statuses, and criteria completion statuses
        List<QuestStatusData> questStatusList = new List<QuestStatusData>();
        List<QuestCriteriaData> criteriaStatusList = new List<QuestCriteriaData>();
        List<CriteriaCompletionData> criteriaCompletionList = new List<CriteriaCompletionData>();

        foreach (var questEntry in activeQuests)
        {
            string questName = questEntry.Key;
            MainQuest quest = questEntry.Value;

            // Save quest status
            questStatusList.Add(new QuestStatusData
            {
                questName = questName,
                questStatus = quest.status
            });

            // Save each criteria status
            foreach (var criteria in quest.questCriteriaList)
            {
                criteriaStatusList.Add(new QuestCriteriaData
                {
                    questName = questName,
                    criteriaName = criteria.criteriaName,
                    criteriaStatus = criteria.CriteriaStatus
                });
            }

            // Save criteria completion status
            List<bool> completionStatusList = new List<bool>();
            foreach (var criteria in quest.questCriteriaList)
            {
                completionStatusList.Add(criteria.CriteriaStatus == QuestEnums.QuestCriteriaStatus.Completed);
            }
            criteriaCompletionList.Add(new CriteriaCompletionData
            {
                questName = questName,
                completionStatus = completionStatusList
            });
        }

        // Assign to save object (saving directly without GetValue/SetValue)
        QuestSaveObject.questStatuses.Value = questStatusList;
        QuestSaveObject.criteriaStatuses.Value = criteriaStatusList;
        QuestSaveObject.criteriaCompletionStatus.Value = criteriaCompletionList;
    }
    private void LoadQuest()
    {
        // Load data from the save object (directly referencing Value)
        List<string> savedQuestNames = QuestSaveObject.activeQuestNames.Value;
        List<QuestStatusData> questStatuses = QuestSaveObject.questStatuses.Value;
        List<QuestCriteriaData> criteriaStatuses = QuestSaveObject.criteriaStatuses.Value;
        List<CriteriaCompletionData> criteriaCompletionStatus = QuestSaveObject.criteriaCompletionStatus.Value;

        if (savedQuestNames == null || savedQuestNames.Count == 0) return;

        activeQuests.Clear();

        foreach (var questStatusData in questStatuses)
        {
            // Find the corresponding quest
            MainQuest quest = mainQuestObjects.Find(q => q.questName == questStatusData.questName);
            if (quest != null)
            {
                // Restore the quest status
                quest.status = questStatusData.questStatus;
                activeQuests.Add(quest.questName, quest);

                // Restore criteria statuses
                foreach (var criteriaData in criteriaStatuses)
                {
                    if (criteriaData.questName == quest.questName)
                    {
                        quest.UpdateCriteriaStatus(criteriaData.criteriaName, criteriaData.criteriaStatus);
                    }
                }

                // Restore criteria completion statuses
                foreach (var completionData in criteriaCompletionStatus)
                {
                    if (completionData.questName == quest.questName)
                    {
                        for (int i = 0; i < completionData.completionStatus.Count && i < quest.questCriteriaList.Count; i++)
                        {
                            if (completionData.completionStatus[i])
                            {
                                quest.questCriteriaList[i].CriteriaStatus = QuestEnums.QuestCriteriaStatus.Completed;
                            }
                        }
                    }
                }

                // Update the UI for this quest after loading
                QuestUIManager questUIManager = FindObjectOfType<QuestUIManager>();
                if (questUIManager != null)
                {
                    questUIManager.UpdateQuestUI(quest);  // Call the UI update method
                }
            }
        }
    }


    // Optional event to handle quest acceptance
    void OnQuestAccepted(MainQuest quest)
    {
        // Implement logic for accepting quests here, such as showing UI or activating NPC dialogue
    }

    // Optional event to handle quest acceptance
    void OnNextCriteriaStarted(MainQuest quest)
    {
        // Implement logic for accepting quests here, such as showing UI or activating NPC dialogue
    }

    // Optional event to handle quest completion
    void OnQuestCompleted(MainQuest quest)
    {
        // Implement logic for completing quests here, such as giving rewards
    }
}


