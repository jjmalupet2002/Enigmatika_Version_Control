using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
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

    [Header("Saving and Loading")]
    public bool IntroQuestFinished = false;
    public Tutorial_IntroQuestSaveObject tutorialSaveObject;

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

    void Start()
    {
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
                break;
            case QuestState.Complete:
                objectiveText.text = "Quest Complete!";
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
        UnityEngine.Debug.Log("Loaded state: " + savedState);
        currentState = (QuestState)savedState;

        // Debug log the current state
        UnityEngine.Debug.Log("Current state after load: " + currentState);

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