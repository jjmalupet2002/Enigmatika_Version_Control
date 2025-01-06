using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using DialogueEditor; // Or the correct namespace where NPCConversation is located
using System.Linq; // Add this at the top

public class SpawnZone : MonoBehaviour
{
    public QuestManager questManager;
    public List<QuestObject> findItemList;
    public Collider exploreAreaCollider;
    public Collider escapeExitCollider;
    public GameObject unlockableObject;
    public GameObject unlockableObjectPrefab;
    public UnityEvent onUnlockEvent;
    public NPCConversation conversation;

    void Start()
    {
        foreach (var questObject in findItemList)
        {
            foreach (var criteria in questObject.associatedQuest.questCriteriaList)
            {
                HandleCriteria(criteria);
            }
        }
    }

    private void HandleCriteria(QuestCriteria criteria)
    {
        // Adjusted condition to check for new status logic
        if (!IsCriteriaInProgress(criteria))
        {
            return; // Skip if not InProgress
        }

        switch (criteria.criteriaType)
        {
            case QuestEnums.QuestCriteriaType.Find:
                HandleFindCriteria(criteria);
                break;
            case QuestEnums.QuestCriteriaType.Explore:
                HandleExploreCriteria(criteria);
                break;
            case QuestEnums.QuestCriteriaType.Escape:
                HandleEscapeCriteria(criteria);
                break;
            case QuestEnums.QuestCriteriaType.UnlockSolve:
                HandleUnlockSolveCriteria(criteria);
                break;
            case QuestEnums.QuestCriteriaType.Talk:
                HandleTalkCriteria(criteria);
                break;
            default:
                UnityEngine.Debug.LogWarning("Unknown criteria type: " + criteria.criteriaType);
                break;
        }
    }

    private bool IsCriteriaInProgress(QuestCriteria criteria)
    {
        // Assuming new logic might be to use another method or flag to determine progress.
        return criteria.CriteriaStatus == QuestEnums.QuestCriteriaStatus.InProgress;
    }

    private void HandleFindCriteria(QuestCriteria criteria)
    {
        if (IsCriteriaInProgress(criteria))
        {
            foreach (QuestObject item in findItemList)
            {
                if (item.isFoundByPlayer)
                {
                    UnityEngine.Debug.Log("Find criteria met, marking as complete: " + criteria.criteriaName);
                    SetNextActiveCriteria(criteria);
                }
            }
        }
    }

    private void HandleExploreCriteria(QuestCriteria criteria)
    {
        if (IsCriteriaInProgress(criteria))
        {
            foreach (var questObject in findItemList)
            {
                if (questObject.isExplorationCompleted)
                {
                    UnityEngine.Debug.Log("Explore criteria met, marking as complete: " + criteria.criteriaName);
                    SetNextActiveCriteria(criteria);
                }
            }
        }
    }

    private void HandleEscapeCriteria(QuestCriteria criteria)
    {
        if (IsCriteriaInProgress(criteria))
        {
            UnityEngine.Debug.Log("Escape criteria met, marking as complete: " + criteria.criteriaName);
            SetNextActiveCriteria(criteria);
        }
    }

    private void HandleTalkCriteria(QuestCriteria criteria)
    {
        if (IsCriteriaInProgress(criteria))
        {
            // Ensure that we check the individual completion condition for this criteria
            var questObject = criteria.associatedQuestObject;

            if (questObject.isTalkCompleted)  // Replace with specific condition for talk completion
            {
                UnityEngine.Debug.Log("Talk criteria met, marking as complete: " + criteria.criteriaName);
                SetNextActiveCriteria(criteria);
            }
        }
    }

    private void HandleUnlockSolveCriteria(QuestCriteria criteria)
    {
        // Implement unlock criteria logic
    }

    // Method to mark criteria as complete and move to the next active criteria
    public void SetNextActiveCriteria(QuestCriteria criteria)
    {
        UnityEngine.Debug.Log("Completing criteria: " + criteria.criteriaName);
        criteria.CriteriaStatus = QuestEnums.QuestCriteriaStatus.Completed;

        // Find the index of the criteria in the quest's criteria list
        var quest = criteria.associatedQuestObject.associatedQuest;
        int currentIndex = quest.questCriteriaList.IndexOf(criteria);

        if (currentIndex >= 0)
        {
            // Call SetNextActiveCriteria to move to the next available criteria in the quest
            questManager.SetNextActiveCriteria(quest, currentIndex);
        }
        else
        {
            UnityEngine.Debug.LogWarning("Criteria not found in the quest's criteria list.");
        }

        // Check if the quest is now complete
        if (quest.questCriteriaList.All(c => c.CriteriaStatus == QuestEnums.QuestCriteriaStatus.Completed))
        {
            questManager.CompleteQuest(quest);  // Ensure quest is completed
        }
    }

    public void NotifyQuestObjectFound(QuestObject questObject)
    {
        foreach (var entry in questManager.GetActiveQuests())
        {
            MainQuest quest = entry.Value;
            foreach (var criteria in quest.questCriteriaList)
            {
                if (criteria.criteriaType == QuestEnums.QuestCriteriaType.Find && criteria.associatedQuestObject == questObject)
                {
                    criteria.CriteriaStatus = QuestEnums.QuestCriteriaStatus.Completed;
                    SetNextActiveCriteria(criteria); // Replacing CheckQuestCompletion
                }
            }
        }
    }

    public void NotifyExploreCriteriaComplete(QuestObject questObject)
    {
        foreach (var entry in questManager.GetActiveQuests())
        {
            MainQuest quest = entry.Value;
            foreach (var criteria in quest.questCriteriaList)
            {
                if (criteria.criteriaType == QuestEnums.QuestCriteriaType.Explore && criteria.associatedQuestObject == questObject)
                {
                    criteria.CriteriaStatus = QuestEnums.QuestCriteriaStatus.Completed;
                    SetNextActiveCriteria(criteria); // Replacing CheckQuestCompletion
                    return;  // Exit the loop after finding the matching criteria
                }
            }
        }
    }

    // Existing method for handling talk criteria
    public void NotifyTalkCriteriaComplete(QuestObject questObject)
    {
        foreach (var entry in questManager.GetActiveQuests())
        {
            MainQuest quest = entry.Value;
            foreach (var criteria in quest.questCriteriaList)
            {
                if (criteria.criteriaType == QuestEnums.QuestCriteriaType.Talk && criteria.associatedQuestObject == questObject)
                {
                    criteria.CriteriaStatus = QuestEnums.QuestCriteriaStatus.Completed;
                    SetNextActiveCriteria(criteria); // Replacing CheckQuestCompletion
                    return;  // Exit the loop after finding the matching criteria
                }
            }
        }
    }

    public void NotifyUnlockCriteriaComplete(QuestObject questObject)
    {
        foreach (var entry in questManager.GetActiveQuests())
        {
            MainQuest quest = entry.Value;
            foreach (var criteria in quest.questCriteriaList)
            {
                if (criteria.criteriaType == QuestEnums.QuestCriteriaType.UnlockSolve && criteria.associatedQuestObject == questObject)
                {
                    criteria.CriteriaStatus = QuestEnums.QuestCriteriaStatus.Completed;
                    SetNextActiveCriteria(criteria); // Replacing CheckQuestCompletion
                    return;  // Exit the loop after finding the matching criteria
                }
            }
        }
    }

    public void NotifyDeliverCriteriaComplete(QuestObject questObject)
    {
        foreach (var entry in questManager.GetActiveQuests())
        {
            MainQuest quest = entry.Value;
            foreach (var criteria in quest.questCriteriaList)
            {
                if (criteria.criteriaType == QuestEnums.QuestCriteriaType.Deliver && criteria.associatedQuestObject == questObject)
                {
                    criteria.CriteriaStatus = QuestEnums.QuestCriteriaStatus.Completed;
                    SetNextActiveCriteria(criteria); // Replacing CheckQuestCompletion
                    return;  // Exit the loop after finding the matching criteria
                }
            }
        }
    }

    public void NotifyEscapeCriteriaComplete(QuestObject questObject)
    {
        foreach (var entry in questManager.GetActiveQuests())
        {
            MainQuest quest = entry.Value;
            foreach (var criteria in quest.questCriteriaList)
            {
                if (criteria.criteriaType == QuestEnums.QuestCriteriaType.Escape && criteria.associatedQuestObject == questObject)
                {
                    criteria.CriteriaStatus = QuestEnums.QuestCriteriaStatus.Completed;
                    SetNextActiveCriteria(criteria); // Replacing CheckQuestCompletion
                }
            }
        }
    }
}