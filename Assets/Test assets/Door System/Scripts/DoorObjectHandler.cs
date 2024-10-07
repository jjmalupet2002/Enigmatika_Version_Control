using System.Diagnostics;
using UnityEngine;

public class DoorObjectHandler : MonoBehaviour
{
    [Tooltip("Check to lock a door")]
    public bool Locked = false;

    [Tooltip("Check this if the door can open.")]
    public bool CanOpen = true;

    [Tooltip("Check this if the door can close.")]
    public bool CanClose = true;

    [Tooltip("When true, the door hinge is free to move, and the player will be able to move through.")]
    public bool IsOpened = false;

    [Tooltip("The interaction radius for the door.")]
    public float interactRange = 3f;

    private HingeJoint hinge;
    private Rigidbody rbDoor;
    private bool isPlayerNearby = false;
    private JointLimits hingeLimits;
    private float currentLimit;
    public float OpenSpeed = 3f;

    void Start()
    {
        hinge = GetComponent<HingeJoint>();
        rbDoor = GetComponent<Rigidbody>();

        if (hinge == null)
        {
            UnityEngine.Debug.LogError("No HingeJoint component found on the door.");
        }
    }

    void Update()
    {
        CheckPlayerProximity();
    }

    private void CheckPlayerProximity()
    {
        Collider[] colliders = Physics.OverlapSphere(transform.position, interactRange);
        foreach (var collider in colliders)
        {
            if (collider.CompareTag("Player"))
            {
                isPlayerNearby = true;
                return;
            }
        }
        isPlayerNearby = false;
    }

    public void Interact()
    {
        if (Locked)
        {
            UnityEngine.Debug.Log("Door is locked.");
            return; // Early exit if the door is locked
        }

        if (IsOpened)
        {
            CloseDoor();
        }
        else
        {
            OpenDoor();
        }
    }

    public void OpenDoor()
    {
        if (!Locked && CanOpen && !IsOpened)
        {
            IsOpened = true;
            rbDoor.AddRelativeTorque(new Vector3(0, 0, 20f)); // Apply torque to open the door
            UnityEngine.Debug.Log("Door is now open.");
        }
    }

    public void CloseDoor()
    {
        if (!Locked && CanClose && IsOpened)
        {
            IsOpened = false;
            UnityEngine.Debug.Log("Door is now closed.");
        }
    }

    private void FixedUpdate()
    {
        if (IsOpened)
        {
            currentLimit = 85f;
        }
        else
        {
            if (currentLimit > 1f)
                currentLimit -= .5f * OpenSpeed;
        }

        hingeLimits.max = currentLimit;
        hingeLimits.min = -currentLimit;
        hinge.limits = hingeLimits;
    }

    public bool IsPlayerNearby()
    {
        return isPlayerNearby;
    }
}
