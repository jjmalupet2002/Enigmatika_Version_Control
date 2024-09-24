using UnityEngine;

public class GameStateManager : MonoBehaviour
{
    public static GameStateManager Instance { get; private set; }

    private bool canPlayerMove = true;

    // References to UI elements
    public GameObject interactUI; // Assign in Inspector
    public CanvasGroup joystickCanvasGroup; // Assign in Inspector

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
    }

    public bool CanPlayerMove()
    {
        return canPlayerMove;
    }

    public void SetPlayerMovementState(bool state)
    {
        canPlayerMove = state;
    }

    // Methods to enable/disable UI elements
    public void DisableUIElements()
    {
        if (interactUI != null) interactUI.SetActive(false);
     

        if (joystickCanvasGroup != null)
        {
            joystickCanvasGroup.gameObject.SetActive(false); // Disable the whole CanvasGroup
        }
    }

    public void EnableUIElements()
    {
        if (interactUI != null) interactUI.SetActive(true);
        

        if (joystickCanvasGroup != null)
        {
            joystickCanvasGroup.gameObject.SetActive(true); // Enable the whole CanvasGroup
            joystickCanvasGroup.interactable = true;
            joystickCanvasGroup.blocksRaycasts = true;
            joystickCanvasGroup.alpha = 1f; // Show joystick
        }
    }
}
