using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TeleporterManager : MonoBehaviour
{
    [System.Serializable]
    public class TeleporterPair
    {
        public Transform teleporterA;
        public Transform teleporterB;
    }

    public List<TeleporterPair> teleporterPairs;
    public Transform playerTransform;        // Player's Transform reference
    public Rigidbody playerRigidbody;        // Player's Rigidbody reference (to modify interpolation)
    public Transform mainCamera;
    public float teleportDelay = 0.5f;
    public CanvasGroup fadeCanvasGroup;

    private Dictionary<Transform, Transform> teleporterDict;
    private bool isTeleporting = false;

    private void Start()
    {
        teleporterDict = new Dictionary<Transform, Transform>();

        foreach (var pair in teleporterPairs)
        {
            teleporterDict[pair.teleporterA] = pair.teleporterB;
            teleporterDict[pair.teleporterB] = pair.teleporterA;
        }

        // Check if Rigidbody is assigned
        if (playerRigidbody == null)
        {
            UnityEngine.Debug.LogError("No Rigidbody found on the player object. Please assign it.");
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            Teleport(other.transform);
        }
    }

    public void Teleport(Transform teleporter)
    {
        if (!isTeleporting && teleporterDict.ContainsKey(teleporter))
        {
            StartCoroutine(TeleportPlayer(teleporterDict[teleporter]));
        }
    }

    private IEnumerator TeleportPlayer(Transform destination)
    {
        isTeleporting = true;

        // Disable interpolation before teleport to prevent physics-based jumps or movements
        playerRigidbody.interpolation = RigidbodyInterpolation.None;

        // Fade out
        yield return StartCoroutine(FadeOut());

        // Store the current interpolation setting to restore later
        RigidbodyInterpolation originalInterpolation = playerRigidbody.interpolation;

        // Store the player's position before teleporting for comparison
        Vector3 initialPosition = playerTransform.position;

        // Teleport the player now that interpolation is disabled
        Vector3 destinationPosition = destination.position;

        // If the destination has a collider, make sure the player lands on top of it
        Collider destinationCollider = destination.GetComponent<Collider>();
        if (destinationCollider != null)
        {
            destinationPosition.y += destinationCollider.bounds.extents.y;  // Ensure the player is on top
        }

        // Set the player's position to the destination
        playerTransform.position = destinationPosition;

        // Adjust the camera position relative to the player, keeping y-axis constant
        Vector3 cameraOffset = mainCamera.position - playerTransform.position;
        mainCamera.position = new Vector3(playerTransform.position.x + cameraOffset.x, mainCamera.position.y, playerTransform.position.z + cameraOffset.z);

        // Store the player's position after teleporting to compare later
        Vector3 postTeleportPosition = playerTransform.position;

        // Fade in
        yield return StartCoroutine(FadeIn());

        // Check if the player has moved (i.e., if the Rigidbody's position has changed)
        if (postTeleportPosition != initialPosition)
        {
            // If the position has changed, set interpolation to Extrapolate for smoother movement
            playerRigidbody.interpolation = RigidbodyInterpolation.Extrapolate;
        }
        else
        {
            // If no movement occurred, restore the original interpolation setting
            playerRigidbody.interpolation = originalInterpolation;
        }

        isTeleporting = false;
    }



    private IEnumerator FadeOut()
    {
        float elapsedTime = 0f;
        while (fadeCanvasGroup.alpha < 1)
        {
            elapsedTime += Time.deltaTime;
            fadeCanvasGroup.alpha = Mathf.Clamp01(elapsedTime / teleportDelay);
            yield return null;
        }
    }

    private IEnumerator FadeIn()
    {
        float elapsedTime = 0f;
        while (fadeCanvasGroup.alpha > 0)
        {
            elapsedTime += Time.deltaTime;
            fadeCanvasGroup.alpha = Mathf.Clamp01(1 - (elapsedTime / teleportDelay));
            yield return null;
        }
    }
}
