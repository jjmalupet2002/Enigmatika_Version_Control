using System.Collections.Generic;
using System.Diagnostics;
using UnityEngine;
using UnityEngine.Events;
using DialogueEditor; // Or the correct namespace where NPCConversation is located

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
                    criteria.CriteriaStatus = QuestEnums.QuestCriteriaStatus.Completed;
                    UnityEngine.Debug.Log("Find criteria met, marking as complete: " + criteria.criteriaName);
                    NotifyCriteriaComplete(criteria);
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
                    criteria.CriteriaStatus = QuestEnums.QuestCriteriaStatus.Completed;
                    UnityEngine.Debug.Log("Explore criteria met, marking as complete: " + criteria.criteriaName);
                    NotifyCriteriaComplete(criteria);
                }
            }
        }
    }

    private void HandleEscapeCriteria(QuestCriteria criteria)
    {
        if (IsCriteriaInProgress(criteria) && escapeExitCollider.bounds.Contains(GameObject.FindGameObjectWithTag("Player").transform.position))
        {
            criteria.CriteriaStatus = QuestEnums.QuestCriteriaStatus.Completed;
            UnityEngine.Debug.Log("Escape criteria met, marking as complete: " + criteria.criteriaName);
            NotifyCriteriaComplete(criteria);
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
                criteria.CriteriaStatus = QuestEnums.QuestCriteriaStatus.Completed;
                UnityEngine.Debug.Log("Talk criteria met, marking as complete: " + criteria.criteriaName);
                NotifyCriteriaComplete(criteria);
            }
        }
    }

    private void HandleUnlockSolveCriteria(QuestCriteria criteria)
    {
        // Implement unlock criteria logic
    }

    public void NotifyCriteriaComplete(QuestCriteria criteria)
    {
        UnityEngine.Debug.Log("Completing criteria: " + criteria.criteriaName);
        criteria.CriteriaStatus = QuestEnums.QuestCriteriaStatus.Completed;

        // After completing a criteria, set the next highest priority criteria to InProgress
        questManager.SetHighestPriorityCriteriaInProgress(criteria.associatedQuestObject.associatedQuest);
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
                    questManager.CheckQuestCompletion(quest);
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
                    questManager.CheckQuestCompletion(quest);
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
                    questManager.CheckQuestCompletion(quest);
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
                    questManager.CheckQuestCompletion(quest);
                }
            }
        }
    }
}
