using System.Collections;
using System.Diagnostics;
using UnityEngine;

public class TestNpcRotation : MonoBehaviour
{
    public GameObject npcGameObject; // The NPC GameObject to detect
    private Quaternion initialNpcRotation; // The initial rotation of the NPC
    private bool rotationDetected = false; // To track if rotation exceeds threshold

    // Event to notify when the NPC rotation exceeds the threshold, passing QuestObject reference
    public delegate void NpcRotationExceededThreshold(QuestObject questObject);
    public static event NpcRotationExceededThreshold OnNpcRotationExceeded;

    // Start is called before the first frame update
    void Start()
    {
        if (npcGameObject != null)
        {
            // Capture the initial rotation of the NPC at the start
            initialNpcRotation = npcGameObject.transform.rotation;
        }
        else
        {
            UnityEngine.Debug.LogWarning("NPC GameObject is not assigned!");
        }
    }

    // Update is called once per frame
    void Update()
    {
        if (npcGameObject != null)
        {
            // Check the angle difference between current rotation and the initial rotation
            float rotationDifference = Quaternion.Angle(npcGameObject.transform.rotation, initialNpcRotation);

            // Ensure we only process once the rotation exceeds the threshold
            if (!rotationDetected && rotationDifference > 5f) // Threshold of 5 degrees
            {
                rotationDetected = true; // Set to true to prevent further checks

                // Log once when rotation exceeds the threshold

                // Trigger event to notify other components (e.g., QuestObject)
                if (OnNpcRotationExceeded != null)
                {
                    // Find the QuestObject associated with this NPC (assuming NPC has a QuestObject component attached)
                    QuestObject questObject = npcGameObject.GetComponent<QuestObject>();
                    if (questObject != null)
                    {
                        OnNpcRotationExceeded.Invoke(questObject); // Pass the specific QuestObject to the handler
                    }
                }
            }
        }
    }
}
