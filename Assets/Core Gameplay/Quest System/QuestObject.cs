using UnityEngine;
using System.Collections;

public class QuestObject : MonoBehaviour
{
    [Header("Main Quest Reference")]
    public MainQuest associatedQuest;  // The quest this object is associated with (quest details)

    [Header("Find Quest settings")]
    public MeshRenderer meshRenderer;  // Reference to the object's MeshRenderer (for visibility control)
    public Collider colliderComponent; // Reference to the object's Collider (for interaction)
    public bool isInteractable;        // Whether the object can be interacted with
    public bool isCompleted;           // Whether this part of the quest has been completed
    public bool isFoundByPlayer;       // Flag to check if the item has been found by the player
    public bool isNote;                // Is this a note?
    public bool is3DObject;            // Is this a 3D object?
    public GameObject noteUI;          // Reference to the note UI (for detecting when it is shown)
    public GameObject referenced3DObject; // Reference to the 3D object to track movement
    private Vector3 initialPosition;   // Store the initial position of the 3D object for movement detection

    [Header("Explore Quest settings")]
    public Collider exploreAreaCollider; // Collider for the explore area
    public bool isExplorationCompleted = false; // Flag to track if exploration is completed

    [Header("Escape Quest settings")]
    public Collider escapeCollider;    // Collider for the escape area
    public bool isEscapeCompleted = false; // Flag to track if escape is completed

    [Header("Talk Quest settings")]
    private Quaternion initialNpcRotation;  // Store the initial rotation of the NPC
    public bool isTalkCompleted = false;

    [Header("Solve/Unlock settings")]
    public GameObject unlockObject;    // Reference to the unlock object
    public bool isUnlockCompleted = false; // Flag to track if unlock is completed

    [Header("Spawn Zone reference:")]
    public SpawnZone spawnZone;        // Reference to the associated SpawnZone

    void Start()
    {
        // Initialize the 3D object's initial position
        if (is3DObject && referenced3DObject != null)
        {
            initialPosition = referenced3DObject.transform.position;
        }

        // Subscribe to the rotation event from TestNpcRotation
        TestNpcRotation.OnNpcRotationExceeded += HandleNpcRotationExceeded;
    }

    void OnDestroy()
    {
        // Unsubscribe from the event when the object is destroyed to avoid memory leaks
        TestNpcRotation.OnNpcRotationExceeded -= HandleNpcRotationExceeded;
    }

    void Update()
    {
        if (associatedQuest == null || associatedQuest.status != QuestEnums.QuestStatus.InProgress)
        {
            return; // Exit if the associated quest is not in progress
        }

        // Handle 3D Object inspection (if moved from the initial position)
        if (is3DObject && referenced3DObject != null)
        {
            if (Vector3.Distance(referenced3DObject.transform.position, initialPosition) > 0.1f)
            {
                Interact(); // Call Interact if the 3D object has moved
            }
        }

        // Handle Note UI inspection
        if (isNote && noteUI != null && noteUI.activeSelf)
        {
            Interact(); // Call Interact if the note UI is active
        }

        // Check for exploration criteria completion using Physics.OverlapSphere
        if (!isExplorationCompleted && exploreAreaCollider != null)
        {
            // Perform an overlap check using the center of the explore area and its radius
            Collider[] colliders = Physics.OverlapSphere(exploreAreaCollider.bounds.center, exploreAreaCollider.bounds.extents.magnitude);

            // Check if the player is within the area
            foreach (Collider col in colliders)
            {
                if (col.CompareTag("Player"))
                {
                    UnityEngine.Debug.Log("Player has explored a new area");

                    isExplorationCompleted = true; // Mark exploration as completed
                    StartCoroutine(NotifySpawnZoneExploreComplete()); // Notify spawn zone that exploration is complete
                    break; // Exit the loop as the player has been found
                }
            }
        }

        // Check if the player is within the escape area using Physics.OverlapSphere
        if (!isEscapeCompleted && escapeCollider != null)
        {
            Collider[] escapeColliders = Physics.OverlapSphere(escapeCollider.bounds.center, 2.0f); // Use a smaller radius for escape interaction

            foreach (Collider col in escapeColliders)
            {
                if (col.CompareTag("Player"))
                {
                    isEscapeCompleted = true;
                    StartCoroutine(NotifySpawnZoneEscapeComplete());
                    break; // Exit the loop as the player has escaped
                }
            }
        }

        // Check if the unlock object's rotation has changed
        if (!isUnlockCompleted && unlockObject != null)
        {
            Vector3 rotation = unlockObject.transform.eulerAngles;
            if (rotation.x != 0 || rotation.y != 0 || rotation.z != 0)
            {
                isUnlockCompleted = true;
                StartCoroutine(NotifySpawnZoneUnlockComplete());
            }
        }
    }

    // This method will be called when NPC rotation exceeds the threshold
    private void HandleNpcRotationExceeded(QuestObject questObject)
    {
        // Ensure that the completion is specific to the QuestObject passed
        if (questObject == this)
        {
            // Here, you can update the state of the QuestObject
            isTalkCompleted = true;
            StartCoroutine(NotifySpawnZoneTalkComplete());
        }
    }

    public void Interact()
    {
        // If the object is interactable, set it as found by player
        if (isInteractable && !isFoundByPlayer)
        {
            isFoundByPlayer = true; // Mark the object as found
            StartCoroutine(NotifySpawnZone()); // Notify the spawn zone that the object was found
        }
    }

    private IEnumerator NotifySpawnZone()
    {
        yield return new WaitForSeconds(2); // Wait for 2 seconds
        if (associatedQuest != null && isFoundByPlayer && spawnZone != null)
        {
            // Debug log for when an object is found
            spawnZone.NotifyQuestObjectFound(this);
        }
    }

    private IEnumerator NotifySpawnZoneExploreComplete()
    {
        yield return new WaitForSeconds(2); // Wait for 2 seconds
        if (associatedQuest != null && isExplorationCompleted && spawnZone != null)
        {
            // Notify the spawn zone that exploration is complete
            spawnZone.NotifyExploreCriteriaComplete(this);
        }
    }

    private IEnumerator NotifySpawnZoneEscapeComplete()
    {
        yield return new WaitForSeconds(2); // Wait for 2 seconds
        if (associatedQuest != null && isEscapeCompleted && spawnZone != null)
        {
            // Notify the spawn zone that escape is complete
            spawnZone.NotifyEscapeCriteriaComplete(this);
        }
    }

    private IEnumerator NotifySpawnZoneTalkComplete()
    {
        yield return new WaitForSeconds(2); // Wait for 2 seconds
        if (associatedQuest != null && spawnZone != null)
        {
            spawnZone.NotifyTalkCriteriaComplete(this);
        }
    }

    private IEnumerator NotifySpawnZoneUnlockComplete()
    {
        yield return new WaitForSeconds(2); // Wait for 2 seconds
        if (associatedQuest != null && isUnlockCompleted && spawnZone != null)
        {
            // Notify the spawn zone that unlock is complete
            spawnZone.NotifyUnlockCriteriaComplete(this);
        }
    }
}
