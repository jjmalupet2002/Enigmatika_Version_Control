using UnityEngine;
using System.Collections;

public class CameraManager : MonoBehaviour
{
    [Header("Cameras")]
    [SerializeField] private Camera closeUpCamera; // Assign your close-up camera
    [SerializeField] private Camera topDownCamera; // Assign your top-down camera

    [Header("Question Display")]
    [SerializeField] private GameObject questionDisplay; // Assign the QuestionDisplay GameObject

    [Header("Camera Switch Settings")]
    [SerializeField] private float topDownDuration = 5f; // Duration in seconds for the top-down view

    [Header("Player Controls")]
    [SerializeField] private GameObject playerJoystick; // Assign the player's Joystick GameObject
    [SerializeField] private GameObject playerAttackButton; // Assign the player's Attack Button GameObject

    [Header("Player Model")]
    [SerializeField] private Transform playerModel; // Assign the player's model Transform
    [SerializeField] private Vector3 playerResetPosition = Vector3.zero; // Default reset position for the player
    [SerializeField] private Quaternion playerResetRotation = Quaternion.identity; // Default reset rotation for the player

    [Header("Billboard Scripts")]
    [SerializeField] private MonoBehaviour closeUpBillboard; // Assign the billboard script for close-up view
    [SerializeField] private MonoBehaviour topDownBillboard; // Assign the billboard script for top-down view

    [Header("Projectile Shooters")]
    [SerializeField] private ProjectileShooter[] projectileShooters; // Array of ProjectileShooter scripts

    public void SwitchToCloseUpCamera()
    {
        if (closeUpCamera != null && topDownCamera != null)
        {
            closeUpCamera.enabled = true;
            topDownCamera.enabled = false;

            // Show the question display when in close-up camera
            if (questionDisplay != null)
            {
                questionDisplay.SetActive(true);
            }

            // Disable the player's Joystick during close-up camera
            if (playerJoystick != null)
            {
                playerJoystick.SetActive(false);
            }
            else
            {
                Debug.LogWarning("Player Joystick is not assigned!");
            }

            // Enable the attack button during close-up camera
            if (playerAttackButton != null)
            {
                playerAttackButton.SetActive(true);
            }
            else
            {
                Debug.LogWarning("Player Attack Button is not assigned!");
            }

            // Switch billboard scripts
            SwitchBillboard(closeUpBillboard, topDownBillboard);

            // Disable all projectile shooters during close-up camera
            DisableAllShooters();
        }
        else
        {
            Debug.LogError("Cameras are not assigned!");
        }
    }

    public void SwitchToTopDownCamera()
    {
        if (closeUpCamera != null && topDownCamera != null)
        {
            closeUpCamera.enabled = false;
            topDownCamera.enabled = true;

            // Hide the question display when in top-down camera
            if (questionDisplay != null)
            {
                questionDisplay.SetActive(false);
            }

            // Enable the player's Joystick during top-down camera
            if (playerJoystick != null)
            {
                playerJoystick.SetActive(true);
            }
            else
            {
                Debug.LogWarning("Player Joystick is not assigned!");
            }

            // Disable the attack button during top-down camera
            if (playerAttackButton != null)
            {
                playerAttackButton.SetActive(false);
            }
            else
            {
                Debug.LogWarning("Player Attack Button is not assigned!");
            }

            // Switch billboard scripts
            SwitchBillboard(topDownBillboard, closeUpBillboard);

            // Enable all projectile shooters during top-down camera
            EnableAllShooters();

            // Start the coroutine to switch back to close-up after the specified duration
            StartCoroutine(SwitchBackToCloseUpAfterDelay());
        }
        else
        {
            Debug.LogError("Cameras are not assigned!");
        }
    }
    
    private IEnumerator SwitchBackToCloseUpAfterDelay()
    {
        // Wait for the specified duration
        yield return new WaitForSeconds(topDownDuration);

        // Switch back to the close-up camera
        SwitchToCloseUpCamera();

        // Reset the player's model position and rotation
        ResetPlayerModel();
    }

    private void SwitchBillboard(MonoBehaviour enableBillboard, MonoBehaviour disableBillboard)
    {
        // Disable the specified billboard script
        if (disableBillboard != null)
        {
            disableBillboard.enabled = false;
        }
        else
        {
            Debug.LogWarning("Disable Billboard script is not assigned!");
        }

        // Enable the specified billboard script
        if (enableBillboard != null)
        {
            enableBillboard.enabled = true;
        }
        else
        {
            Debug.LogWarning("Enable Billboard script is not assigned!");
        }
    }

    private void ResetPlayerModel()
    {
        if (playerModel != null)
        {
            // Get the Rigidbody and Collider components
            Rigidbody playerRigidbody = playerModel.GetComponent<Rigidbody>();
            Collider playerCollider = playerModel.GetComponent<Collider>();

            // If the player has a Collider, disable it during the reset
            if (playerCollider != null)
            {
                playerCollider.enabled = false;
            }

            // If the player has a Rigidbody, make it kinematic during the reset
            if (playerRigidbody != null)
            {
                playerRigidbody.isKinematic = true; // Disable physics during reset
            }

            // Reset the player's position and rotation
            playerModel.position = playerResetPosition;
            playerModel.rotation = playerResetRotation;

            // Force Unity to synchronize the physics engine with the transform changes
            Physics.SyncTransforms();

            // If the player has a Rigidbody, reset its velocity and angular velocity
            if (playerRigidbody != null)
            {
                playerRigidbody.velocity = Vector3.zero;
                playerRigidbody.angularVelocity = Vector3.zero;
                playerRigidbody.isKinematic = false; // Re-enable physics after reset
            }

            // If the player has a Collider, re-enable it after the reset
            if (playerCollider != null)
            {
                playerCollider.enabled = true;
            }

            Debug.Log("Player model reset to position: " + playerResetPosition + " and rotation: " + playerResetRotation);
        }
        else
        {
            Debug.LogWarning("Player model is not assigned!");
        }
    }

    // Enable all projectile shooters
    private void EnableAllShooters()
    {
        if (projectileShooters != null && projectileShooters.Length > 0)
        {
            foreach (var shooter in projectileShooters)
            {
                if (shooter != null)
                {
                    shooter.EnableShooting();
                }
            }
        }
        else
        {
            Debug.LogWarning("No projectile shooters assigned!");
        }
    }

    // Disable all projectile shooters
    private void DisableAllShooters()
    {
        if (projectileShooters != null && projectileShooters.Length > 0)
        {
            foreach (var shooter in projectileShooters)
            {
                if (shooter != null)
                {
                    shooter.DisableShooting();
                }
            }
        }
        else
        {
            Debug.LogWarning("No projectile shooters assigned!");
        }
    }
}