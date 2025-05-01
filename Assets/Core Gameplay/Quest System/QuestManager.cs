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
    public CompassArrow compass; // Assign this in the Inspector

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
            quest.status = QuestEnums.QuestStatus.NotStarted;

            foreach (var criteria in quest.questCriteriaList)
            {
                criteria.CriteriaStatus = QuestEnums.QuestCriteriaStatus.NotStarted;

                if (criteria.associatedQuestObject != null)  // Add this check
                {
                    criteria.associatedQuestObject.gameObject.SetActive(false);
                }
                else
                {
                    UnityEngine.Debug.LogWarning($"Quest '{quest.questName}' criteria '{criteria.criteriaName}' has a null associatedQuestObject.");
                }
            }

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
        quest.questCriteriaList.Sort((a, b) => a.priority.CompareTo(b.priority));

        if (quest.questCriteriaList.Count > 0)
        {
            var highestPriorityCriteria = quest.questCriteriaList[0];

            if (highestPriorityCriteria.CriteriaStatus != QuestEnums.QuestCriteriaStatus.Completed)
            {
                highestPriorityCriteria.CriteriaStatus = QuestEnums.QuestCriteriaStatus.InProgress;

                if (highestPriorityCriteria.associatedQuestObject != null)
                {
                    highestPriorityCriteria.associatedQuestObject.gameObject.SetActive(true);

                    // Set compass target here
                    if (compass != null)
                    {
                        compass.SetTarget(highestPriorityCriteria.associatedQuestObject.transform);
                    }
                }
            }
        }
    }


    // Coroutine to disable the quest object after a delay
    private IEnumerator DisableAfterDelay(GameObject questObject, float delay)
    {
        yield return new WaitForSeconds(delay);
        questObject.SetActive(false);
    }

    public void SetNextActiveCriteria(MainQuest quest, int currentIndex)
    {
        // Disable the associated quest object of the completed criteria
        if (currentIndex >= 0 && currentIndex < quest.questCriteriaList.Count)
        {
            var completedCriteria = quest.questCriteriaList[currentIndex];
            if (completedCriteria.CriteriaStatus == QuestEnums.QuestCriteriaStatus.Completed)
            {
                if (completedCriteria.associatedQuestObject != null)
                {
                    float delay = 0f;

                    if (completedCriteria.criteriaType == QuestEnums.QuestCriteriaType.Talk)
                    {
                        delay = 30f; // 30 seconds for Talk criteria
                    }
                    else if (completedCriteria.criteriaType == QuestEnums.QuestCriteriaType.Explore ||
                             completedCriteria.criteriaType == QuestEnums.QuestCriteriaType.Escape)
                    {
                        delay = 3f; // 3 seconds for Explore and Escape criteria
                    }

                    if (delay > 0)
                    {
                        StartCoroutine(DisableAfterDelay(completedCriteria.associatedQuestObject.gameObject, delay));
                    }
                }

                // Activate the next criteria (if any)
                int nextIndex = currentIndex + 1;
                if (nextIndex < quest.questCriteriaList.Count)
                {
                    var nextCriteria = quest.questCriteriaList[nextIndex];

                    if (nextCriteria.associatedQuestObject != null)
                    {
                        nextCriteria.associatedQuestObject.gameObject.SetActive(true);

                        // Update compass target
                        if (compass != null)
                        {
                            compass.SetTarget(nextCriteria.associatedQuestObject.transform);
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

                        if (nextCriteria.associatedQuestObject != null) // Added null check for safety
                        {
                            nextCriteria.associatedQuestObject.gameObject.SetActive(true); // Enable the associated quest object
                        }

                        UnityEngine.Debug.Log($"Next Task: {nextCriteria.criteriaName}");

                        // Trigger the OnQuestAcceptedEvent here
                        OnNextCriteriaStarted(quest);
                        OnNextCriteriaStartedEvent?.Invoke(quest); // Notify listeners

                        return; // Exit once the next criteria is found and set to InProgress
                    }
                }

                // If no next criteria are found, log that the quest is completed
                UnityEngine.Debug.Log("No next criteria found, quest completed.");
            } // <-- This closing bracket was missing
        }
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
        QuestSaveObject.activeQuestNames.Value = new List<string>(activeQuests.Keys);

        List<QuestData> questDataList = new List<QuestData>();

        foreach (var questEntry in activeQuests)
        {
            string questName = questEntry.Key;
            MainQuest quest = questEntry.Value;

            QuestData questData = new QuestData
            {
                questName = questName,
                questStatus = quest.status,
                criteriaStatuses = new List<QuestCriteriaData>(),
                criteriaCompletionStatus = new List<bool>(),
                questObjectStates = new List<QuestObjectStateData>()
            };

            foreach (var criteria in quest.questCriteriaList)
            {
                questData.criteriaStatuses.Add(new QuestCriteriaData
                {
                    criteriaName = criteria.criteriaName,
                    criteriaStatus = criteria.CriteriaStatus
                });

                // Save associated quest object state (active/inactive)
                questData.questObjectStates.Add(new QuestObjectStateData
                {
                    criteriaName = criteria.criteriaName,
                    isActive = criteria.associatedQuestObject != null && criteria.associatedQuestObject.gameObject.activeSelf
                });

                // Save completion status
                questData.criteriaCompletionStatus.Add(criteria.CriteriaStatus == QuestEnums.QuestCriteriaStatus.Completed);
            }

            questDataList.Add(questData);
        }

        QuestSaveObject.questDataList.Value = questDataList;
    }

    private void LoadQuest()
    {
        List<string> savedQuestNames = QuestSaveObject.activeQuestNames.Value;
        List<QuestData> savedQuestDataList = QuestSaveObject.questDataList.Value;

        if (savedQuestNames == null || savedQuestNames.Count == 0) return;

        activeQuests.Clear();

        foreach (var questData in savedQuestDataList)
        {
            MainQuest quest = mainQuestObjects.Find(q => q.questName == questData.questName);
            if (quest != null)
            {
                // Instead of reinitializing, just add to active quests
                if (!activeQuests.ContainsKey(quest.questName))
                {
                    activeQuests.Add(quest.questName, quest);
                }

                // Restore quest status
                quest.status = questData.questStatus;

                bool foundActiveCriteria = false; // Track if an active criteria has been set

                for (int i = 0; i < questData.criteriaStatuses.Count; i++)
                {
                    var criteria = quest.questCriteriaList[i];
                    var savedCriteria = questData.criteriaStatuses[i];

                    // Restore criteria status
                    if (!foundActiveCriteria && savedCriteria.criteriaStatus == QuestEnums.QuestCriteriaStatus.InProgress)
                    {
                        // First found active criteria remains active
                        foundActiveCriteria = true;
                    }
                    else if (savedCriteria.criteriaStatus == QuestEnums.QuestCriteriaStatus.InProgress)
                    {
                        // Any additional "InProgress" criteria is reset to NotStarted
                        savedCriteria.criteriaStatus = QuestEnums.QuestCriteriaStatus.NotStarted;
                    }

                    criteria.CriteriaStatus = savedCriteria.criteriaStatus;

                    // Restore associated quest object states
                    if (criteria.associatedQuestObject != null)
                    {
                        criteria.associatedQuestObject.gameObject.SetActive(questData.questObjectStates[i].isActive);
                    }
                }

                // Ensure only one criteria is in progress
                if (quest.status == QuestEnums.QuestStatus.InProgress)
                {
                    LogActiveCriteria(quest);
                }

                // Update the UI for this quest after loading
                if (quest.status != QuestEnums.QuestStatus.Completed)
                {
                    questUIManager?.UpdateQuestUI(quest);
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


