using UnityEngine;
using System.Collections;
using System.Diagnostics;
using UnityEngine.UI;

public class QuestObject : MonoBehaviour
{
    [Header("Main Quest Reference")]
    public MainQuest associatedQuest;

    [Header("Find Quest settings")]
    public MeshRenderer meshRenderer;
    public Collider colliderComponent;
    public bool isInteractable;
    public bool isCompleted;
    public bool isFoundByPlayer;
    public bool isNote;
    public bool is3DObject;
    public GameObject noteUI;
    public GameObject referenced3DObject;
    private Vector3 initialPosition;

    [Header("Explore Quest settings")]
    public Collider exploreAreaCollider;
    public bool isExplorationCompleted = false;

    [Header("Escape Quest settings")]
    public Collider escapeCollider;
    public bool isEscapeCompleted = false;

    [Header("Talk Quest settings")]
    private Quaternion initialNpcRotation;
    public bool isTalkCompleted = false;

    [Header("Solve/Unlock settings")]
    public GameObject unlockObject;
    public bool isUnlockCompleted = false;

    [Header("Deliver settings")]
    public bool isDeliverCompleted = false;

    [Header("Spawn Zone reference:")]
    public SpawnZone spawnZone;

    void Start()
    {
        if (is3DObject && referenced3DObject != null)
        {
            initialPosition = referenced3DObject.transform.position;
        }

        TestNpcRotation.OnNpcRotationExceeded += HandleNpcRotationExceeded; 
    }

    void OnDestroy()
    {
        TestNpcRotation.OnNpcRotationExceeded -= HandleNpcRotationExceeded;
    }

    void Update()
    {
        if (associatedQuest == null || associatedQuest.status != QuestEnums.QuestStatus.InProgress)
        {
            return;
        }

        if (is3DObject && referenced3DObject != null)
        {
            if (Vector3.Distance(referenced3DObject.transform.position, initialPosition) > 0.1f)
            {
                Interact();
            }
        }

        if (isNote && noteUI != null && noteUI.activeSelf)
        {
            Interact();
        }

        if (!isExplorationCompleted && exploreAreaCollider != null)
        {
            Collider[] colliders = Physics.OverlapSphere(exploreAreaCollider.bounds.center, exploreAreaCollider.bounds.extents.magnitude);

            foreach (Collider col in colliders)
            {
                if (col.CompareTag("Player"))
                {
                    isExplorationCompleted = true;
                    StartCoroutine(NotifySpawnZoneExploreComplete());
                    break;
                }
            }
        }

        if (!isEscapeCompleted && escapeCollider != null)
        {
            Collider[] escapeColliders = Physics.OverlapSphere(escapeCollider.bounds.center, 2.0f);

            foreach (Collider col in escapeColliders)
            {
                if (col.CompareTag("Player"))
                {
                    isEscapeCompleted = true;
                    StartCoroutine(NotifySpawnZoneEscapeComplete());
                    break;
                }
            }
        }

        if (!isUnlockCompleted && unlockObject != null)
        {
            Vector3 rotation = unlockObject.transform.eulerAngles;
            if (rotation.x != 0 || rotation.y != 0 || rotation.z != 0)
            {
                isUnlockCompleted = true;
                StartCoroutine(NotifySpawnZoneUnlockComplete());
            }
        }

        // Check for Deliver criteria completion
        if (!isDeliverCompleted)
        {
            GameObject wrongItemDeliverUI = GameObject.FindWithTag("WrongItemDeliverUI");
            GameObject checkMarkQuest = GameObject.FindWithTag("CheckMarkQuest");

            // If either condition is met, mark Deliver as complete and notify
            if ((wrongItemDeliverUI != null && wrongItemDeliverUI.activeSelf) ||
                (checkMarkQuest != null && checkMarkQuest.activeSelf))
            {
                isDeliverCompleted = true; // Mark deliver as completed
                NotifySpawnZoneDeliverComplete(); // Notify spawn zone
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
        if (isInteractable && !isFoundByPlayer)
        {
            isFoundByPlayer = true;
            StartCoroutine(NotifySpawnZone());
        }
    }

    private IEnumerator NotifySpawnZone()
    {
        yield return new WaitForSeconds(5);
        if (associatedQuest != null && isFoundByPlayer && spawnZone != null)
        {
            spawnZone.NotifyQuestObjectFound(this);
        }
    }

    private IEnumerator NotifySpawnZoneExploreComplete()
    {
        yield return new WaitForSeconds(2);
        if (associatedQuest != null && isExplorationCompleted && spawnZone != null)
        {
            spawnZone.NotifyExploreCriteriaComplete(this);
        }
    }

    private IEnumerator NotifySpawnZoneEscapeComplete()
    {
        yield return new WaitForSeconds(2);
        if (associatedQuest != null && isEscapeCompleted && spawnZone != null)
        {
            spawnZone.NotifyEscapeCriteriaComplete(this);
        }
    }

    private IEnumerator NotifySpawnZoneTalkComplete()
    {
        yield return new WaitForSeconds(15);
        if (associatedQuest != null && spawnZone != null)
        {
            spawnZone.NotifyTalkCriteriaComplete(this);
        }
    }

    private IEnumerator NotifySpawnZoneUnlockComplete()
    {
        yield return new WaitForSeconds(2);
        if (associatedQuest != null && isUnlockCompleted && spawnZone != null)
        {
            spawnZone.NotifyUnlockCriteriaComplete(this);
        }
    }

    private void NotifySpawnZoneDeliverComplete()
    {
        if (associatedQuest != null && spawnZone != null)
        {
            UnityEngine.Debug.Log("Deliver criteria completed.");
            isDeliverCompleted = true;

            // Notify the spawn zone that Deliver is complete
            spawnZone.NotifyDeliverCriteriaComplete(this);
        }
    }
}
