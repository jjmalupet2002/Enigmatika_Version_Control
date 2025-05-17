using System.Collections.Generic;
using UnityEngine;

public class InstructionEnabler : MonoBehaviour
{
    [System.Serializable]
    public class InstructionEntry
    {
        public GameObject triggerObject;
        public GameObject instructionUI;
        public float interactRange = 2f;

        [HideInInspector] public bool hasTriggered = false;
    }

    public GameObject player;
    public List<InstructionEntry> instructionEntries = new List<InstructionEntry>();
    public LayerMask triggerLayer; // Optional: Assign this if you want filtering

    void Update()
    {
        foreach (var entry in instructionEntries)
        {
            if (entry.hasTriggered || entry.triggerObject == null || entry.instructionUI == null || player == null)
                continue;

            float range = entry.interactRange;
            Vector3 center = entry.triggerObject.transform.position;

            Collider[] hits = Physics.OverlapSphere(center, range);
            foreach (Collider hit in hits)
            {
                if (hit.gameObject == player)
                {
                    entry.instructionUI.SetActive(true);
                    entry.hasTriggered = true; // Mark as triggered
                    break;
                }
            }
        }
    }

    void OnDrawGizmosSelected()
    {
        if (instructionEntries == null) return;

        Gizmos.color = Color.green;
        foreach (var entry in instructionEntries)
        {
            if (entry != null && entry.triggerObject != null)
            {
                Gizmos.DrawWireSphere(entry.triggerObject.transform.position, entry.interactRange);
            }
        }
    }
}
