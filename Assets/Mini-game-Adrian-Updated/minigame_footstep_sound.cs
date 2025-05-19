using UnityEngine;

public class minigame_footstep_sound : MonoBehaviour
{
    public AudioSource footstepAudioSource; // Reference to the AudioSource component
    public AudioClip footstepClip; // Footstep sound clip
    public float stepInterval = 0.5f; // Interval between footsteps

    private minigame_movement playerMovementScript; // Reference to the joystick movement script
    private float stepTimer; // Timer to track footstep intervals

    private void Start()
    {
        // Get the minigame_movement script attached to the player
        playerMovementScript = GetComponent<minigame_movement>();
        stepTimer = 0f;
    }

    private void Update()
    {
        // Check if the player is moving
        if (playerMovementScript.IsMoving())
        {
            // Handle footstep sounds
            stepTimer += Time.deltaTime;
            if (stepTimer >= stepInterval)
            {
                footstepAudioSource.PlayOneShot(footstepClip);
                stepTimer = 0f;
            }
        }
        else
        {
            // Reset the step timer when not moving
            stepTimer = 0f;
        }
    }
}
