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
}
