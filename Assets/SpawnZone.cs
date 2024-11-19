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

        if (criteria.status != QuestEnums.QuestStatus.InProgress)
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

    private void HandleFindCriteria(QuestCriteria criteria)
    {
        if (criteria.status == QuestEnums.QuestStatus.InProgress)
        {
            foreach (QuestObject item in findItemList)
            {
                if (item.isFoundByPlayer)
                {
                    criteria.status = QuestEnums.QuestStatus.Completed;
                    UnityEngine.Debug.Log("Find criteria met, marking as complete: " + criteria.criteriaName);
                    NotifyCriteriaComplete(criteria);
                }
            }
        }
    }

    private void HandleExploreCriteria(QuestCriteria criteria)
    {
        if (criteria.status == QuestEnums.QuestStatus.InProgress)
        {
            foreach (var questObject in findItemList)
            {
                if (questObject.isExplorationCompleted)
                {
                    criteria.status = QuestEnums.QuestStatus.Completed;
                    UnityEngine.Debug.Log("Explore criteria met, marking as complete: " + criteria.criteriaName);
                    NotifyCriteriaComplete(criteria);
                }
            }
        }
    }

    private void HandleEscapeCriteria(QuestCriteria criteria)
    {
        if (criteria.status == QuestEnums.QuestStatus.InProgress && escapeExitCollider.bounds.Contains(GameObject.FindGameObjectWithTag("Player").transform.position))
        {
            criteria.status = QuestEnums.QuestStatus.Completed;
            UnityEngine.Debug.Log("Escape criteria met, marking as complete: " + criteria.criteriaName);
            NotifyCriteriaComplete(criteria);
        }
    }


    private void HandleTalkCriteria(QuestCriteria criteria)
    {
        if (criteria.status == QuestEnums.QuestStatus.InProgress)
        {
            // Ensure that we check the individual completion condition for this criteria
            var questObject = criteria.associatedQuestObject;

            if (questObject.isTalkCompleted)  // Replace with specific condition for talk completion
            {
                criteria.status = QuestEnums.QuestStatus.Completed;
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
        // Log before updating the criteria status
        UnityEngine.Debug.Log("Completing criteria: " + criteria.criteriaName);

        // Mark the criteria as completed
        criteria.status = QuestEnums.QuestStatus.Completed;
        UnityEngine.Debug.Log("Criteria completed: " + criteria.criteriaName);

        // Now check if the quest is fully completed
        questManager.CheckQuestCompletion(criteria.associatedQuestObject.associatedQuest);

        // Handle the next criteria
        foreach (var nextCriteria in criteria.associatedQuestObject.associatedQuest.questCriteriaList)
        {
            if (nextCriteria.status == QuestEnums.QuestStatus.NotStarted)
            {
                nextCriteria.status = QuestEnums.QuestStatus.InProgress;
                UnityEngine.Debug.Log($"Next active criteria: {nextCriteria.criteriaName} for quest: {criteria.associatedQuestObject.associatedQuest.questName}");
                break;  // Only set the first available NotStarted criteria
            }
        }


        // Handle enabling/disabling MeshRenderer and Colliders based on criteria priority
        HandleQuestObjectVisibility(criteria);
    }


    private void HandleQuestObjectVisibility(QuestCriteria criteria)
    {
        // Sort the criteria by priority (ascending order)
        List<QuestCriteria> sortedCriteria = new List<QuestCriteria>(criteria.associatedQuestObject.associatedQuest.questCriteriaList);
        sortedCriteria.Sort((c1, c2) => c1.priority.CompareTo(c2.priority));

        bool foundInProgress = false;

        // Enable MeshRenderer and Collider for the highest-priority, InProgress criteria
        foreach (var questCriteria in sortedCriteria)
        {
            if (questCriteria.status == QuestEnums.QuestStatus.InProgress)
            {
                var associatedQuestObject = questCriteria.associatedQuestObject;

                // Enable components only for the highest-priority in-progress criteria
                if (!foundInProgress)
                {
                    if (associatedQuestObject.meshRenderer != null) associatedQuestObject.meshRenderer.enabled = true;
                    if (associatedQuestObject.colliderComponent != null) associatedQuestObject.colliderComponent.enabled = true;

                    foundInProgress = true; // Mark that we have enabled components for one object
                }
                else
                {
                    // Disable components for any lower-priority in-progress objects
                    if (associatedQuestObject.meshRenderer != null) associatedQuestObject.meshRenderer.enabled = false;
                    if (associatedQuestObject.colliderComponent != null) associatedQuestObject.colliderComponent.enabled = false;
                }
            }
            else
            {
                // If the criteria is completed, ensure components are disabled
                var associatedQuestObject = questCriteria.associatedQuestObject;
                if (associatedQuestObject.meshRenderer != null) associatedQuestObject.meshRenderer.enabled = false;
                if (associatedQuestObject.colliderComponent != null) associatedQuestObject.colliderComponent.enabled = false;
            }
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
                    criteria.status = QuestEnums.QuestStatus.Completed;
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
                    criteria.status = QuestEnums.QuestStatus.Completed;
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
                    criteria.status = QuestEnums.QuestStatus.Completed;
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
                    criteria.status = QuestEnums.QuestStatus.Completed;
                    questManager.CheckQuestCompletion(quest);

                }
            }
        }
    }
}