using UnityEngine;
using UnityEngine.SceneManagement;

public class GameStateManager : MonoBehaviour
{
    public static GameStateManager Instance { get; private set; }

    private bool canPlayerMove = true;

    public GameObject interactUI;
    public CanvasGroup joystickCanvasGroup;

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject); // Persist the GameStateManager
            SceneManager.sceneLoaded += OnSceneLoaded; // Listen for scene changes
        }
        else if (Instance != this)
        {
            Destroy(gameObject);
        }
    }

    private void OnDestroy()
    {
        SceneManager.sceneLoaded -= OnSceneLoaded;
    }

    private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        TryFindUIReferences();
    }

    public void TryFindUIReferences()
    {
        interactUI = GameObject.FindWithTag("InteractUI");
        joystickCanvasGroup = GameObject.FindWithTag("JoystickUI")?.GetComponent<CanvasGroup>();
    }

    public bool CanPlayerMove() => canPlayerMove;

    public void SetPlayerMovementState(bool state) => canPlayerMove = state;

    public void DisableUIElements()
    {
        if (interactUI != null) interactUI.SetActive(false);
        if (joystickCanvasGroup != null)
        {
            joystickCanvasGroup.gameObject.SetActive(false);
        }
    }

    public void EnableUIElements()
    {
        if (interactUI != null) interactUI.SetActive(true);
        if (joystickCanvasGroup != null)
        {
            joystickCanvasGroup.gameObject.SetActive(true);
            joystickCanvasGroup.interactable = true;
            joystickCanvasGroup.blocksRaycasts = true;
            joystickCanvasGroup.alpha = 1f;
        }
    }
}
