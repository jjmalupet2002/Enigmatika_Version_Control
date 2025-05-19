using System.Linq;
using UnityEngine;
using UnityEngine.UI;

public class PadLockPassword : MonoBehaviour
{
    MoveRuller _moveRull;

    public Button unlockButton;
    public Camera targetCamera;

    public int[] _numberPassword = { 0, 0, 0, 0 };

    private void Awake()
    {
        _moveRull = FindObjectOfType<MoveRuller>();

        if (_moveRull == null)
        {
            UnityEngine.Debug.LogError("MoveRuller component not found in the scene!");
        }

        if (unlockButton != null)
        {
            unlockButton.onClick.RemoveAllListeners();
            unlockButton.onClick.AddListener(Password);
            unlockButton.gameObject.SetActive(false);
        }
        else
        {
            UnityEngine.Debug.LogWarning("Unlock button not assigned in the Inspector!");
        }
    }

    private void Update()
    {
        if (targetCamera != null)
        {
            unlockButton.gameObject.SetActive(targetCamera.enabled);
        }
        else
        {
            UnityEngine.Debug.LogWarning("Target camera not assigned!");
        }
    }

    public void Password()
    {
        UnityEngine.Debug.Log("Checking password...");

        // Debugging: Print both arrays
        UnityEngine.Debug.Log("Entered: " + string.Join(",", _moveRull._numberArray));
        UnityEngine.Debug.Log("Correct: " + string.Join(",", _numberPassword));

        if (_moveRull._numberArray.SequenceEqual(_numberPassword))
        {
            UnityEngine.Debug.Log("Password correct!");

            for (int i = 0; i < _moveRull._rullers.Count; i++)
            {
                _moveRull._rullers[i].GetComponent<PadLockEmissionColor>()._isSelect = false;
                _moveRull._rullers[i].GetComponent<PadLockEmissionColor>().BlinkingMaterial();
            }

            PlaySuccessFeedback();
        }
        else
        {
            UnityEngine.Debug.Log("Incorrect password!");
            PlayFailureFeedback();
        }
    }

    void PlaySuccessFeedback()
    {
        UnityEngine.Debug.Log("Unlock animation or sound plays here!");
    }

    void PlayFailureFeedback()
    {
        UnityEngine.Debug.Log("Incorrect input feedback (e.g., shake animation)!");
    }
}
