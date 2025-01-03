using UnityEngine;
using System.Collections;
using UnityEngine.SceneManagement; // For scene loading
using UnityEngine.UI; // For button interaction

public class MiniGameLoader : MonoBehaviour
{
    public Transform trapGateObject; // Reference to the trapGate object
    public Collider miniGameStarter; // Public reference to the MiniGameStarter collider
    public float interactRange = 1f; // Interaction range (set to 1)

    // References for the cutscene
    public GameObject cutscenePage1;  // Parent object containing all the objects for the cutscene
    public Image cutsceneImage1;      // Slideshow image for the cutscene
    public Button endButton;          // End button to trigger the scene load

    private Vector3 initialTrapGatePosition; // Store the initial position of the trapGate

    private bool isCutsceneActive = false;  // Flag to track cutscene state
    public GameObject EscapeText;


    void Start()
    {
        // Store the initial position of the trapGate
        initialTrapGatePosition = trapGateObject.position;

        // Add the button listener for the "End Button"
        endButton.onClick.AddListener(LoadMiniGameScene);
    }

    void Update()
    {
        // Enable the MiniGameStarter collider if the trapGateObject has moved
        if (trapGateObject.position != initialTrapGatePosition)
        {
            miniGameStarter.enabled = true;
        }

        // If the MiniGameStarter collider is enabled, check for interaction range
        if (miniGameStarter.enabled && !isCutsceneActive)
        {
            // Check if the player is within the interact range using OverlapSphere
            Collider[] colliders = Physics.OverlapSphere(miniGameStarter.transform.position, interactRange);

            foreach (Collider collider in colliders)
            {
                // Check if the colliding object is tagged as "Player"
                if (collider.CompareTag("Player"))
                {
                    UnityEngine.Debug.Log("Player is within interact range, starting cutscene...");

                    // Trigger the cutscene
                    StartCutscene();
                }
            }
        }
    }

    // Starts the cutscene sequence
    private void StartCutscene()
    {
        isCutsceneActive = true;

        // Show the cutscene page
        cutscenePage1.SetActive(true);

        // Optionally, start a coroutine to display the slideshow images
        StartCoroutine(ShowCutsceneImages());

        // Disable EscapeText if it is active
        if (EscapeText.activeSelf)
        {
            EscapeText.SetActive(false);
        }
    }

    // Coroutine to handle the slideshow of images
    private IEnumerator ShowCutsceneImages()
    {
        // Show the first cutscene image
        cutsceneImage1.gameObject.SetActive(true);

        // Wait for some time before allowing interaction (e.g., 5 seconds)
        yield return new WaitForSeconds(5f);

        // After the cutscene, enable the End Button for interaction
        endButton.gameObject.SetActive(true);
    }

    // Loads the "minigame1" scene when the End Button is clicked
    private void LoadMiniGameScene()
    {
        UnityEngine.Debug.Log("End Button clicked, loading minigame1...");
        SceneManager.LoadScene("minigame1");
    }
}
