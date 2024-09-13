using UnityEngine;

public class GameStateManager : MonoBehaviour
{
    public static GameStateManager Instance { get; private set; }

    private bool canPlayerMove = true;

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
            UnityEngine.Debug.Log("GameStateManager Instance initialized.");
        }
        else
        {
            Destroy(gameObject);
            UnityEngine.Debug.LogWarning("Duplicate GameStateManager instance detected and destroyed.");
        }
    }

    public bool CanPlayerMove()
    {
        UnityEngine.Debug.Log("CanPlayerMove called. Returning: " + canPlayerMove);
        return canPlayerMove;
    }

    public void SetPlayerMovementState(bool state)
    {
        canPlayerMove = state;
        UnityEngine.Debug.Log("SetPlayerMovementState called. New state: " + state);
    }
}
