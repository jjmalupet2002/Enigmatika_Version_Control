using System.Diagnostics;
using UnityEngine;

public class DoorManagerScript : MonoBehaviour
{
    public static DoorManagerScript Instance { get; private set; }

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject); // Ensure only one instance
        }
        else
        {
            Instance = this;
            DontDestroyOnLoad(gameObject); // Persist across scenes
        }
    }

    public void InteractWithNearestDoor()
    {
        DoorObjectHandler nearestDoor = FindNearestDoor();
        if (nearestDoor != null)
        {
            nearestDoor.Interact();
        }
        // Removed the debug log for no nearby door
    }


    private DoorObjectHandler FindNearestDoor()
    {
        DoorObjectHandler nearestDoor = null;
        float closestDistance = float.MaxValue;

        foreach (DoorObjectHandler door in FindObjectsOfType<DoorObjectHandler>())
        {
            if (door.IsPlayerNearby())
            {
                float distance = Vector3.Distance(transform.position, door.transform.position);
                if (distance < closestDistance)
                {
                    closestDistance = distance;
                    nearestDoor = door;
                }
            }
        }

        return nearestDoor;
    }
}