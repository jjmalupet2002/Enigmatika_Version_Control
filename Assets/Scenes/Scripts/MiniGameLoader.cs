using UnityEngine;
using System.Collections;
using UnityEngine.SceneManagement; // For scene loading
using UnityEngine.UI; // For button interaction
using CarterGames.Assets.SaveManager; // Import SaveManager for saving/loading
using Save;

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

    [SerializeField] private PortalRoomTrapLockSaveObject saveObject; // Reference to the save object

    private void OnEnable()
    {
        SaveEvents.OnSaveGame += SaveMiniGameState;
        SaveEvents.OnLoadGame += LoadMiniGameState;
    }

    private void OnDisable()
    {
        SaveEvents.OnSaveGame -= SaveMiniGameState;
        SaveEvents.OnLoadGame -= LoadMiniGameState;
    }

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
            Collider[] colliders = Physics.OverlapSphere(miniGameStarter.transform.position, interactRange);

            foreach (Collider collider in colliders)
            {
                if (collider.CompareTag("Player"))
                {
                    UnityEngine.Debug.Log("Player is within interact range, starting cutscene...");
                    StartCutscene();
                }
            }
        }
    }

    private void StartCutscene()
    {
        isCutsceneActive = true;

        // Show the cutscene page
        cutscenePage1.SetActive(true);

        // Start a coroutine to display the slideshow images
        StartCoroutine(ShowCutsceneImages());
        SaveEvents.SaveGame(); // Auto-save before switching to minigame scene

        // Disable EscapeText if it is active
        if (EscapeText.activeSelf)
        {
            EscapeText.SetActive(false);
        }
    }

    private IEnumerator ShowCutsceneImages()
    {
        cutsceneImage1.gameObject.SetActive(true);
        yield return new WaitForSeconds(5f);
        endButton.gameObject.SetActive(true);
    }

    private void LoadMiniGameScene()
    {
        UnityEngine.Debug.Log("End Button clicked, loading minigame1...");
        SceneManager.LoadScene("minigame1");
    }

    // Save the EscapeText and miniGameStarter states
    private void SaveMiniGameState()
    {
        if (saveObject != null)
        {
            saveObject.isEscapeTextActive.Value = EscapeText.activeSelf;
            saveObject.isMiniGameStarterEnabled.Value = miniGameStarter.enabled;
        }
    }

    // Load the EscapeText and miniGameStarter states
    private void LoadMiniGameState()
    {
        if (saveObject != null)
        {
            EscapeText.SetActive(saveObject.isEscapeTextActive.Value);
            miniGameStarter.enabled = saveObject.isMiniGameStarterEnabled.Value;
        }
    }
}
