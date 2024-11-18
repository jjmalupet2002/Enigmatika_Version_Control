using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class SpawnZone : MonoBehaviour
{
    public QuestManager questManager;

    public List<QuestObject> findItemList;
    public Collider exploreAreaCollider;
    public Collider escapeExitCollider;
    public GameObject unlockableObject;
    public GameObject unlockableObjectPrefab;
    public UnityEvent onUnlockEvent;

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

    private void HandleUnlockSolveCriteria(QuestCriteria criteria)
    {
        // Implement unlock criteria logic
    }

    public void NotifyCriteriaComplete(QuestCriteria criteria)
    {
        criteria.status = QuestEnums.QuestStatus.Completed;
        questManager.CheckQuestCompletion(criteria.associatedQuestObject.associatedQuest);
        UnityEngine.Debug.Log("Criteria completed: " + criteria.criteriaName);

        // Automatically update next criteria
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

        // Enable MeshRenderer and Collider for the first QuestObject that is InProgress
        foreach (var questObject in sortedCriteria)
        {
            if (questObject.status != QuestEnums.QuestStatus.Completed)
            {
                var associatedQuestObject = questObject.associatedQuestObject;

                // Enable the components for the first in-progress object
                if (!foundInProgress)
                {
                    if (associatedQuestObject.meshRenderer != null) associatedQuestObject.meshRenderer.enabled = true;
                    if (associatedQuestObject.colliderComponent != null) associatedQuestObject.colliderComponent.enabled = true;

                    foundInProgress = true; // Mark that we have enabled components for one object
                }
                else
                {
                    // Disable the components for any lower-priority in-progress objects
                    if (associatedQuestObject.meshRenderer != null) associatedQuestObject.meshRenderer.enabled = false;
                    if (associatedQuestObject.colliderComponent != null) associatedQuestObject.colliderComponent.enabled = false;
                }
            }
            else
            {
                // If the criteria is completed, ensure the components are disabled
                var associatedQuestObject = questObject.associatedQuestObject;
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
