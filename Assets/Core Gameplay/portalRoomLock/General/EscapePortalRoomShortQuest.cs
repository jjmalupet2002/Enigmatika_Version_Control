using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class EscapePortalRoomShortQuest : MonoBehaviour
{
    public Text objectiveText; // Reference to the UI text component for displaying objectives
    public Transform player; // Reference to the player's transform

    public Collider portalCollider; // Collider for the portal objective
    public Collider exitPortalRoomCollider; // Collider for the final objective

    public GameObject hoodedFigureUI; // UI game object for the hooded figure objective
    public GameObject bookUI; // UI game object for the book objective

    public float portalInteractRange = 5f; // Interaction range for the portal objective
    public float exitPortalRoomInteractRange = 5f; // Interaction range for the final objective
    public bool IntroQuestFinished = false;

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

    void Start()
    {
        currentState = QuestState.EscapePortal;
        UpdateObjectiveText();
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
                if (IsPlayerInRange(exitPortalRoomCollider, exitPortalRoomInteractRange))
                {
                    currentState = QuestState.Complete;
                    UpdateObjectiveText();
                    isQuestComplete = true; // Mark the quest as complete
                    StartCoroutine(HideCompleteTextAfterDelay(2f)); // Hide the complete text after 2 seconds
                }
                break;

            case QuestState.Complete:
                // Quest complete, you can add any additional logic here if needed
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
        objectiveText.text = ""; // Clear the text after the delay
    }
}
