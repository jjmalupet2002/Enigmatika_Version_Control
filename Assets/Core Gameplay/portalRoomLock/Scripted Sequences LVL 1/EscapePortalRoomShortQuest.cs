using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using Save;
using CarterGames.Assets.SaveManager;

public class EscapePortalRoomShortQuest : MonoBehaviour
{
    public Text objectiveText; // UI text component for displaying objectives
    public Transform player; // Player's transform

    public Collider portalCollider; // Portal objective collider
    public Collider exitPortalRoomCollider; // Final objective collider
    public Collider PortalRoomExit; // Portal room exit collider
    public GameObject EscapeFailedCutscene; // Cutscene game object
    public Button EndEscapeCutsceneButton; // Button to exit cutscene

    public GameObject hoodedFigureUI; // Hooded figure UI
    public GameObject bookUI; // Book UI

    public float portalInteractRange = 5f;
    public float exitPortalRoomInteractRange = 5f;

    [Header("Door Highlighting")]
    public GameObject doorObject; // The door GameObject to highlight
    public Material whiteBlinkMaterial; // The blinking white material
    public float blinkDuration = 2f;
    public float blinkInterval = 0.2f;
    private bool isBlinking = false;

    private Material originalDoorMaterial;
    private Renderer doorRenderer;


    [Header("Saving and Loading")]
    public bool IntroQuestFinished = false;
    public Tutorial_IntroQuestSaveObject tutorialSaveObject;

    public GameObject arrowUI; // Assign the ArrowPointer prefab

    private enum QuestState
    {
        EscapePortal,
        TalkToHoodedFigure,
        WaitForHoodedFigureUI,
        ReadTheBook,
        WaitForBookUI,
        ExitPortalRoom,
        Complete
    }

    private QuestState currentState;
    private bool isQuestComplete = false;

    private void OnEnable()
    {
        // Subscribe to the save and load events
        SaveEvents.OnSaveGame += SaveQuestState;
        SaveEvents.OnLoadGame += LoadQuestState;
    }

    private void OnDisable()
    {
        // Unsubscribe from the save and load events
        SaveEvents.OnSaveGame -= SaveQuestState;
        SaveEvents.OnLoadGame -= LoadQuestState;
    }

    private IEnumerator ShowArrowBlink()
    {
        arrowUI.SetActive(true);

        // Blink effect
        float duration = 2.5f;
        float blinkInterval = 0.3f;
        float elapsedTime = 0f;

        Image arrowImage = arrowUI.GetComponentInChildren<Image>();
        if (arrowImage == null)
        {
            UnityEngine.Debug.LogWarning("No Image found on arrow UI.");
            yield break;
        }

        while (elapsedTime < duration)
        {
            arrowImage.enabled = !arrowImage.enabled; // Toggle visibility
            yield return new WaitForSeconds(blinkInterval);
            elapsedTime += blinkInterval;
        }

        arrowImage.enabled = true; // Ensure it’s visible when done
        arrowUI.SetActive(false);
    }

    private IEnumerator BlinkDoorMaterial()
    {
        if (doorRenderer == null || whiteBlinkMaterial == null || originalDoorMaterial == null)
        {
            yield break; // Early exit if any of the materials or renderer are not assigned
        }

        float elapsedTime = 0f;
        bool useWhite = true;

        while (elapsedTime < blinkDuration)
        {
            doorRenderer.material = useWhite ? whiteBlinkMaterial : originalDoorMaterial;
            useWhite = !useWhite;

            yield return new WaitForSeconds(blinkInterval);
            elapsedTime += blinkInterval;
        }

        doorRenderer.material = originalDoorMaterial; // Ensure original material is restored
    }

    void Start()
    {
        if (doorObject != null)
        {
            doorRenderer = doorObject.GetComponent<Renderer>();
            if (doorRenderer != null)
            {
                originalDoorMaterial = doorRenderer.material;
            }
        }

        if (portalCollider == null)
        {
            UnityEngine.Debug.LogError("portalCollider is not assigned!", this);
        }

        if (EndEscapeCutsceneButton != null)
        {
            EndEscapeCutsceneButton.onClick.AddListener(DisableEscapeCutscene);
        }
        else
        {
            UnityEngine.Debug.LogError("EndEscapeCutsceneButton is not assigned!", this);
        }

        currentState = QuestState.EscapePortal;
        UpdateObjectiveText();
        PortalRoomExit.gameObject.SetActive(false);
    }

    void Update()
    {
        if (isQuestComplete) return;

        switch (currentState)
        {
            case QuestState.EscapePortal:
                if (IsPlayerInRange(portalCollider, portalInteractRange))
                {
                    currentState = QuestState.TalkToHoodedFigure;
                    UpdateObjectiveText();
                }
                break;

            case QuestState.TalkToHoodedFigure:
                if (hoodedFigureUI.activeSelf)
                {
                    currentState = QuestState.WaitForHoodedFigureUI;
                }
                break;

            case QuestState.WaitForHoodedFigureUI:
                if (!hoodedFigureUI.activeSelf)
                {
                    StartCoroutine(ShowArrowBlink()); // Show the arrow when book opens
                    currentState = QuestState.ReadTheBook;
                    UpdateObjectiveText();
                }
                break;

            case QuestState.ReadTheBook:
                if (bookUI.activeSelf)
                {
                    currentState = QuestState.WaitForBookUI;
                }
                break;

            case QuestState.WaitForBookUI:
                if (!bookUI.activeSelf)
                {
                    currentState = QuestState.ExitPortalRoom;
                    UpdateObjectiveText();
                }
                break;

            case QuestState.ExitPortalRoom:
                PortalRoomExit.gameObject.SetActive(true);

                if (IsPlayerInRange(exitPortalRoomCollider, exitPortalRoomInteractRange))
                {
                    currentState = QuestState.Complete;
                    UpdateObjectiveText();
                    isQuestComplete = true;
                    StartCoroutine(HideCompleteTextAfterDelay(2f));
                }
                break;

            case QuestState.Complete:
                PortalRoomExit.gameObject.SetActive(false);
                PortalRoomExit.gameObject.SetActive(true);
                break;
        }
    }

    bool IsPlayerInRange(Collider targetCollider, float interactRange)
    {
        Collider[] hitColliders = Physics.OverlapSphere(targetCollider.transform.position, interactRange);
        foreach (var hitCollider in hitColliders)
        {
            if (hitCollider.CompareTag("Player"))
            {
                // Only trigger the cutscene if the quest is NOT complete
                if (!isQuestComplete && currentState == QuestState.EscapePortal)
                {
                    EscapeFailedCutscene.SetActive(true);
                }
                return true;
            }
        }
        return false;
    }

    void UpdateObjectiveText()
    {
        switch (currentState)
        {
            case QuestState.EscapePortal:
                objectiveText.text = "Escape through the portal";
                break;
            case QuestState.TalkToHoodedFigure:
                objectiveText.text = "Talk to the Hooded figure";
                break;
            case QuestState.WaitForHoodedFigureUI:
                objectiveText.text = "Complete your interaction with the Hooded figure";
                break;
            case QuestState.ReadTheBook:
                objectiveText.text = "Read the book";
                break;
            case QuestState.WaitForBookUI:
                objectiveText.text = "Finish reading the book";
                break;
            case QuestState.ExitPortalRoom:
                objectiveText.text = "Go out the Portal Room";
                // Start blinking effect on the door
                if (doorRenderer != null && whiteBlinkMaterial != null)
                {
                    StartCoroutine(BlinkDoorMaterial());
                }
                break;
            case QuestState.Complete:
                objectiveText.text = "Talk to the hooded figure";
                IntroQuestFinished = true;
                break;
        }
    }

    IEnumerator HideCompleteTextAfterDelay(float delay)
    {
        yield return new WaitForSeconds(delay);
        objectiveText.text = "";
    }

    void DisableEscapeCutscene()
    {
        if (EscapeFailedCutscene != null)
        {
            EscapeFailedCutscene.SetActive(false);
        }
    }

    // Save the current quest state
    private void SaveQuestState()
    {
        // Save the state by setting the Value
        tutorialSaveObject.currentQuestState.Value = (int)currentState;
    }

    // Load the saved quest state
    private void LoadQuestState()
    {
        int savedState = tutorialSaveObject.currentQuestState.Value;
        currentState = (QuestState)savedState;

        if (currentState == QuestState.ExitPortalRoom || currentState == QuestState.Complete)
        {
            PortalRoomExit.gameObject.SetActive(true);
        }
        else
        {
            PortalRoomExit.gameObject.SetActive(false);
        }

        if (currentState == QuestState.Complete)
        {
            objectiveText.text = "";
            IntroQuestFinished = true;  // Ensure this is set after loading
        }
        else
        {
            UpdateObjectiveText();
        }
    }
}
