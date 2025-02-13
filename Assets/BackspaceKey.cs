using System;
using UnityEngine;
using UnityEngine.UI;

public class BackspaceKey : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private Button backspaceButton;
    public static Action onBackspacePressed;

    private void Start()
    {
        backspaceButton.onClick.AddListener(HandleBackspace);
    }

    private void HandleBackspace()
    {
        onBackspacePressed?.Invoke();
    }
}
