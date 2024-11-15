using System.Collections;
using UnityEngine;

public class TrapGate : MonoBehaviour
{
    [Header("TrapGate Settings")]
    public Collider triggerCollider;
    public GameObject gateObject;
    public float gateStartingY;
    public float gateTargetY;
    public float gateMovementDelay = 1f;
    public AudioSource trapGateSound;

    private Vector3 initialPosition;
    private Vector3 targetPosition;

    void Start()
    {
        // Set the initial and target positions for the gate
        initialPosition = new Vector3(gateObject.transform.position.x, gateStartingY, gateObject.transform.position.z);
        targetPosition = new Vector3(gateObject.transform.position.x, gateTargetY, gateObject.transform.position.z);

        // Set the gate to the starting position
        gateObject.transform.position = initialPosition;
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            // Start the coroutine to move the gate
            StartCoroutine(MoveGate());
        }
    }

    private IEnumerator MoveGate()
    {
        // Wait for the specified delay
        yield return new WaitForSeconds(gateMovementDelay);

        // Play the sound if available
        if (trapGateSound != null)
        {
            trapGateSound.Play();
        }

        // Move the gate to the target position
        float elapsedTime = 0;
        float duration = 1f; // Duration of the gate movement

        while (elapsedTime < duration)
        {
            gateObject.transform.position = Vector3.Lerp(initialPosition, targetPosition, elapsedTime / duration);
            elapsedTime += Time.deltaTime;
            yield return null;
        }

        gateObject.transform.position = targetPosition;

        // Delete the trigger collider object
        Destroy(triggerCollider.gameObject);
    }
}
