using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using UnityEngine;
using UnityEngine.UI;

public class MapController : MonoBehaviour
{
    [System.Serializable]
    public class MapCheckpoint
    {
        public GameObject checkpointObject;
        public Image playerImage;
        public Image questImage;
        public float interactRange = 5f;
        public bool isPlayerNearby = false;
        [HideInInspector] public QuestCriteria associatedQuestCriteria;
    }

    [Header("References")]
    public QuestManager questManager;
    public GameObject player;
    public List<MapCheckpoint> mapCheckpoints = new List<MapCheckpoint>();
    public List<MainQuest> mainQuestList;

    [Header("Detection")]
    public float defaultInteractRange = 5f;
    public LayerMask checkpointLayerMask;

    [Header("Icons")]
    public Sprite playerIcon;
    public Sprite findQuestIcon;
    public Sprite talkQuestIcon;
    public Sprite exploreQuestIcon;
    public Sprite escapeQuestIcon;

    private void Start()
    {
        // Initialize the quest list reference from the QuestManager if not assigned
        if (questManager != null && mainQuestList == null || mainQuestList.Count == 0)
        {
            mainQuestList = questManager.mainQuestObjects;
        }

        // Make sure player reference exists
        if (player == null)
        {
            player = GameObject.FindGameObjectWithTag("Player");
            if (player == null)
            {
                UnityEngine.Debug.LogError("Player reference not found. Please assign it in the inspector or tag your player GameObject with 'Player'.");
            }
        }

        // Initialize all map checkpoint images (hide them at start)
        foreach (var checkpoint in mapCheckpoints)
        {
            if (checkpoint.playerImage != null)
            {
                checkpoint.playerImage.enabled = false;
            }

            if (checkpoint.questImage != null)
            {
                checkpoint.questImage.enabled = false;
            }
        }
    }

    private void Update()
    {
        CheckPlayerProximityToCheckpoints();
        UpdateQuestIcons();
    }

    [Header("Debug")]
    public bool showDebugLogs = false;

    private void CheckPlayerProximityToCheckpoints()
    {
        if (player == null) return;

        Vector3 playerPosition = player.transform.position;

        // Check each checkpoint
        foreach (var checkpoint in mapCheckpoints)
        {
            if (checkpoint.checkpointObject == null) continue;

            Vector3 checkpointPosition = checkpoint.checkpointObject.transform.position;
            bool wasNearby = checkpoint.isPlayerNearby;

            // Using PhysicsOverlapSphere for more accurate detection including colliders
            Collider[] hitColliders = Physics.OverlapSphere(playerPosition, checkpoint.interactRange, checkpointLayerMask);
            checkpoint.isPlayerNearby = System.Array.Exists(hitColliders,
                collider => collider.gameObject == checkpoint.checkpointObject);

            // Alternative simple distance check
            // checkpoint.isPlayerNearby = Vector3.Distance(playerPosition, checkpointPosition) <= checkpoint.interactRange;

            // Only update UI if state changed (optimization)
            if (wasNearby != checkpoint.isPlayerNearby)
            {
                UpdatePlayerIcon(checkpoint);

                if (showDebugLogs)
                {
                    if (checkpoint.isPlayerNearby)
                        return;
                    else
                        return;
                }
            }
        }
    }

    private void UpdatePlayerIcon(MapCheckpoint checkpoint)
    {
        if (checkpoint.playerImage != null)
        {
            checkpoint.playerImage.enabled = checkpoint.isPlayerNearby;
            checkpoint.playerImage.sprite = playerIcon;
        }
    }

    private void UpdateQuestIcons()
    {
        // First, hide all quest icons
        foreach (var checkpoint in mapCheckpoints)
        {
            if (checkpoint.questImage != null)
            {
                checkpoint.questImage.enabled = false;
            }
        }

        // Then, process in-progress quest criteria
        if (questManager != null)
        {
            Dictionary<string, MainQuest> activeQuests = questManager.GetActiveQuests();

            foreach (var questEntry in activeQuests)
            {
                MainQuest quest = questEntry.Value;

                // Skip completed quests
                if (quest.status != QuestEnums.QuestStatus.InProgress)
                    continue;

                // Find the in-progress criteria
                QuestCriteria inProgressCriteria = null;
                foreach (var criteria in quest.questCriteriaList)
                {
                    if (criteria.CriteriaStatus == QuestEnums.QuestCriteriaStatus.InProgress &&
                        criteria.criteriaType != QuestEnums.QuestCriteriaType.Deliver)
                    {
                        inProgressCriteria = criteria;
                        break;
                    }
                }

                // Skip if no in-progress criteria found or associated object is null
                if (inProgressCriteria == null || inProgressCriteria.associatedQuestObject == null)
                    continue;

                // Get the position of the associated quest object
                Vector3 questObjectPosition = inProgressCriteria.associatedQuestObject.transform.position;

                // Find the closest checkpoint to this quest object
                foreach (var checkpoint in mapCheckpoints)
                {
                    if (checkpoint.checkpointObject == null) continue;

                    float distance = Vector3.Distance(questObjectPosition, checkpoint.checkpointObject.transform.position);

                    // If quest object is near this checkpoint, display the appropriate icon
                    if (distance <= checkpoint.interactRange)
                    {
                        checkpoint.associatedQuestCriteria = inProgressCriteria;
                        SetQuestIconByCriteriaType(checkpoint, inProgressCriteria.criteriaType);

                        if (showDebugLogs)
                        {
                            return;
                            //UnityEngine.Debug.Log($"Quest object '{inProgressCriteria.associatedQuestObject.name}' is near checkpoint '{checkpoint.checkpointObject.name}'");
                        }
                    }
                }
            }
        }
    }

    private void SetQuestIconByCriteriaType(MapCheckpoint checkpoint, QuestEnums.QuestCriteriaType criteriaType)
    {
        if (checkpoint.questImage == null) return;

        // Enable the image
        checkpoint.questImage.enabled = true;

        // Set appropriate icon based on criteria type
        switch (criteriaType)
        {
            case QuestEnums.QuestCriteriaType.Find:
                checkpoint.questImage.sprite = findQuestIcon;
                break;
            case QuestEnums.QuestCriteriaType.Talk:
                checkpoint.questImage.sprite = talkQuestIcon;
                break;
            case QuestEnums.QuestCriteriaType.Explore:
                checkpoint.questImage.sprite = exploreQuestIcon;
                break;
            case QuestEnums.QuestCriteriaType.Escape:
                checkpoint.questImage.sprite = escapeQuestIcon;
                break;
            default:
                checkpoint.questImage.sprite = findQuestIcon; // Default icon
                break;
        }
    }

    // Optional: Helper method to find a checkpoint by object reference
    public MapCheckpoint FindCheckpointByObject(GameObject targetObject)
    {
        return mapCheckpoints.Find(checkpoint => checkpoint.checkpointObject == targetObject);
    }

    // Optional: Draw gizmos for visualizing interaction range in the editor
    private void OnDrawGizmosSelected()
    {
        // Visualize player interaction range
        if (player != null)
        {
            Gizmos.color = Color.yellow;
            Gizmos.DrawWireSphere(player.transform.position, defaultInteractRange);
        }

        // Visualize checkpoint interaction ranges
        foreach (var checkpoint in mapCheckpoints)
        {
            if (checkpoint.checkpointObject != null)
            {
                Gizmos.color = Color.cyan;
                Gizmos.DrawWireSphere(checkpoint.checkpointObject.transform.position, checkpoint.interactRange);
            }
        }

        // Show lines between quest objects and checkpoints for in-progress quests
        if (Application.isPlaying && questManager != null)
        {
            Dictionary<string, MainQuest> activeQuests = questManager.GetActiveQuests();
            foreach (var questEntry in activeQuests)
            {
                MainQuest quest = questEntry.Value;
                if (quest.status == QuestEnums.QuestStatus.InProgress)
                {
                    foreach (var criteria in quest.questCriteriaList)
                    {
                        if (criteria.CriteriaStatus == QuestEnums.QuestCriteriaStatus.InProgress &&
                            criteria.associatedQuestObject != null)
                        {
                            Gizmos.color = Color.green;
                            Vector3 questObjectPos = criteria.associatedQuestObject.transform.position;

                            // Find closest checkpoint
                            foreach (var checkpoint in mapCheckpoints)
                            {
                                if (checkpoint.checkpointObject != null)
                                {
                                    Vector3 checkpointPos = checkpoint.checkpointObject.transform.position;
                                    if (Vector3.Distance(questObjectPos, checkpointPos) <= checkpoint.interactRange)
                                    {
                                        Gizmos.DrawLine(questObjectPos, checkpointPos);
                                    }
                                }
                            }
                        }
                    }
                }
            }
        }
    }
}