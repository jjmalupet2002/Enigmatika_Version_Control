using UnityEngine;
using UnityEngine.UI;
using CarterGames.Assets.SaveManager; // Include SaveManager namespace
using Save;

public class TeleporterActivator : MonoBehaviour
{
    public Text questCriteria; // Reference to the quest criteria text object
    public BoxCollider LibraryTeleporter; // Reference to the Library teleporter collider
    public PortalRoomTrapLockSaveObject saveObject;

    private void OnEnable()
    {
        SaveEvents.OnSaveGame += SaveTeleporterState;
        SaveEvents.OnLoadGame += LoadTeleporterState;
    }

    private void OnDisable()
    {
        SaveEvents.OnSaveGame -= SaveTeleporterState;
        SaveEvents.OnLoadGame -= LoadTeleporterState;
    }

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
            saveObject.isTeleporterActive.Value = true; // Save teleporter state
        }
    }

    private void SaveTeleporterState()
    {
        if (saveObject != null)
        {
            saveObject.isTeleporterActive.Value = LibraryTeleporter.enabled;
        }
    }

    private void LoadTeleporterState()
    {
        if (saveObject != null)
        {
            LibraryTeleporter.enabled = saveObject.isTeleporterActive.Value;
        }
    }
}
