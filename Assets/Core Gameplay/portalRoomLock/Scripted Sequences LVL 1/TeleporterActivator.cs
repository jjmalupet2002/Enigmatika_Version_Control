using System.Diagnostics;
using UnityEngine;
using UnityEngine.UI;

public class TeleporterActivator : MonoBehaviour
{
    public Text questCriteria; // Reference to the quest criteria text object
    public BoxCollider LibraryTeleporter; // Reference to the Library teleporter collider

    void Start()
    {
        // Disable the teleporter at the start
        if (LibraryTeleporter != null)
        {
            LibraryTeleporter.enabled = false;
        }
    }

    void Update()
    {
        // Check if the quest criteria text is active
        if (questCriteria != null && questCriteria.gameObject.activeSelf)
        {
            EnableTeleporter();
        }
    }

    void EnableTeleporter()
    {
        if (LibraryTeleporter != null && !LibraryTeleporter.enabled)
        {
            LibraryTeleporter.enabled = true;
            UnityEngine.Debug.Log("Library Teleporter Enabled");
        }
    }
}
