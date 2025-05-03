using UnityEngine;
using System.Collections;

public class TutorialHandMovement : MonoBehaviour
{
    public GameObject handImage; // Drag your hand UI object here
    public GameObject bookUI;    // Drag your Book UI Panel here

    public Vector3 firstOffset = new Vector3(-50f, 50f, 0f); // First move (NW diagonal)
    public Vector3 secondOffset = new Vector3(-200f, 200f, 0f); // Second move (further)
    public float forwardSpeed = 2f;
    public float returnSpeed = 0.5f;
    public float pauseDuration = 0.5f; // Time to pause after each movement phase
    public float handDisplayDuration = 5f; // Time before disabling hand image

    private Vector3 startPos;
    private Vector3 targetPos;
    private bool isBookOpened = false;
    private bool isPausing = false;
    private bool isMoving = false;
    private float t = 0f;
    private float pauseTimer = 0f;
    private int movementPhase = 1; // 1 for the first move, 2 for the second, 3 for final reset
    private float handTimer = 0f; // Timer to track hand display time

    void Start()
    {
        if (handImage != null)
            handImage.SetActive(false); // Hand starts hidden

        if (handImage != null)
            startPos = handImage.GetComponent<RectTransform>().anchoredPosition;

        targetPos = startPos + firstOffset; // Start with the first offset
    }

    void Update()
    {
        if (bookUI != null && bookUI.activeInHierarchy && !isBookOpened)
        {
            isBookOpened = true;
            if (handImage != null)
            {
                handImage.SetActive(true);
                t = 0f;
                movementPhase = 1; // Start at phase 1
                isMoving = true;  // Start the movement
            }
        }

        if (isBookOpened && handImage != null && isMoving)
        {
            AnimateHand();

            // Track the time the hand has been active
            if (handTimer < handDisplayDuration)
            {
                handTimer += Time.deltaTime; // Increment timer
            }
            else
            {
                handImage.SetActive(false); // Disable hand image after 5 seconds
                isMoving = false; // Stop movement after 5 seconds
            }
        }
    }

    void AnimateHand()
    {
        RectTransform handRect = handImage.GetComponent<RectTransform>();

        if (movementPhase == 1)
        {
            // Move to the first target position (e.g., -50f, 50f)
            t += Time.deltaTime * forwardSpeed;
            handRect.anchoredPosition = Vector3.Lerp(startPos, targetPos, t);

            if (t >= 1f)
            {
                t = 0f;
                movementPhase = 0; // Pause after reaching the first target
                pauseTimer = 0f; // Reset pause timer
                isPausing = true; // Begin pause phase
            }
        }
        else if (movementPhase == 0)
        {
            // Pause logic at -50f, 50f
            pauseTimer += Time.deltaTime;
            if (pauseTimer >= pauseDuration)
            {
                // After pause, move to the second target position (e.g., -200f, 200f)
                movementPhase = 2;
                targetPos = startPos + firstOffset + secondOffset; // Apply both offsets
                t = 0f; // Reset lerp t for the new movement
            }
        }
        else if (movementPhase == 2)
        {
            // Move to the second target position (e.g., -200f, 200f)
            t += Time.deltaTime * forwardSpeed;
            handRect.anchoredPosition = Vector3.Lerp(startPos + firstOffset, targetPos, t);

            if (t >= 1f)
            {
                t = 0f;
                movementPhase = 3; // Begin reset phase
                pauseTimer = 0f; // Reset the pause timer
                isPausing = true; // Pause at the end position
            }
        }
        else if (movementPhase == 3)
        {
            // Pause logic at -200f, 200f
            pauseTimer += Time.deltaTime;
            if (pauseTimer >= pauseDuration)
            {
                // After pause, reset back to the original position
                movementPhase = 1; // Return to phase 1
                targetPos = startPos + firstOffset; // Set back to the first offset position
                handRect.anchoredPosition = startPos; // Reset position immediately
                isMoving = false; // Stop moving
                handImage.SetActive(false); // Hide the hand
                isBookOpened = false; // Reset the book UI check
            }
        }
    }
}
