using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Events;

public class SafeController : MonoBehaviour
{
    [Header("Safe Model Reference")]
    public GameObject safeModel;
    public Animator safeAnimator;

    [Header("Animation Timers (default: 1f)")]
    public float animationDelay = 1f;

    [Header("Safe UI")]
    public GameObject safeUI; // Ensure this is the parent object and is disabled in the inspector
    public Button safeInspectButton;
    public Button safeNumberConfirmationUI;
    public GameObject wrongCombinationUI; // Ensure this is disabled in the inspector
    public Button safeExitButton;

    [Header("Correct Safe Solution (0-10)")]
    public int safeSolutionNum1;
    public int safeSolutionNum2;
    public int safeSolutionNum3;

    [Header("UI Numbers (text)")]
    public Text firstNumberUI;
    public Text secondNumberUI;
    public Text thirdNumberUI;

    [Header("Unity Event - What happens when you open the safe?")]
    public UnityEvent onSafeOpened;

    [Header("Switch Camera Reference")]
    public SwitchCamera switchCamera; // Reference to the SwitchCamera script

    private int currentNum1;
    private int currentNum2;
    private int currentNum3;
    private bool isSafeOpened = false;

    private void Start()
    {
        safeInspectButton.onClick.AddListener(OpenSafeUI);
        safeNumberConfirmationUI.onClick.AddListener(CheckCombination);
        safeExitButton.onClick.AddListener(CloseSafeUI);

        // Initialize UI states
        wrongCombinationUI.SetActive(false);
        safeUI.SetActive(false);

        // Initialize the numbers and update UI
        currentNum1 = 0;
        currentNum2 = 0;
        currentNum3 = 0;
        firstNumberUI.text = currentNum1.ToString();
        secondNumberUI.text = currentNum2.ToString();
        thirdNumberUI.text = currentNum3.ToString();

        // Subscribe to camera state change event
        if (switchCamera != null)
        {
            switchCamera.onCameraStateChange.AddListener(OnCameraStateChange);
        }
        else
        {
            Debug.LogError("SwitchCamera reference is not assigned in SafeController.");
        }
    }

    public void IncreaseNumber(Text numberText, ref int number)
    {
        number = (number + 1) % 11;
        numberText.text = number.ToString();
    }

    public void DecreaseNumber(Text numberText, ref int number)
    {
        number = (number + 10) % 11; // Handles decrementing, ensuring it wraps around
        numberText.text = number.ToString();
    }

    // UI Button Methods
    public void OnFirstNumberUp() { IncreaseNumber(firstNumberUI, ref currentNum1); }
    public void OnFirstNumberDown() { DecreaseNumber(firstNumberUI, ref currentNum1); }
    public void OnSecondNumberUp() { IncreaseNumber(secondNumberUI, ref currentNum2); }
    public void OnSecondNumberDown() { DecreaseNumber(secondNumberUI, ref currentNum2); }
    public void OnThirdNumberUp() { IncreaseNumber(thirdNumberUI, ref currentNum3); }
    public void OnThirdNumberDown() { DecreaseNumber(thirdNumberUI, ref currentNum3); }

    public void OpenSafeUI()
    {
        if (!isSafeOpened)
        {
            safeUI.SetActive(true);
            safeInspectButton.gameObject.SetActive(false);

        }
    }


    public void CloseSafeUI()
    {
        safeUI.SetActive(false);
        safeInspectButton.gameObject.SetActive(true);

        // Check if the object calling this method has the tag "MainBackButton"
        if (gameObject.CompareTag("MainBackButton"))
        {
            safeInspectButton.gameObject.SetActive(false); // Deactivate the inspect button
        }
        else
        {
            // Ensure the inspect button is shown only if we are in close-up view
            if (switchCamera.currentCameraState != CameraState.CloseUp)
            {
                safeInspectButton.gameObject.SetActive(false); // deactivate the inspect button when we exit close up-view
            }
        }
    }



    public void CheckCombination()
    {
        if (currentNum1 == safeSolutionNum1 && currentNum2 == safeSolutionNum2 && currentNum3 == safeSolutionNum3)
        {
            StartCoroutine(OpenSafe());
        }
        else
        {
            wrongCombinationUI.SetActive(true);
            Invoke("HideWrongCombinationUI", 2f); // Hide after 2 seconds
        }
    }

    private void HideWrongCombinationUI()
    {
        wrongCombinationUI.SetActive(false);
    }

    private IEnumerator OpenSafe()
    {
        yield return new WaitForSeconds(animationDelay);
        safeAnimator.SetTrigger("Open");
        onSafeOpened.Invoke();
        isSafeOpened = true; // Mark the safe as opened
        CloseSafeUI();
    }

    private void OnCameraStateChange(CameraState state)
    {
        // Show the Safe Inspect button only when in close-up view
        if (state == CameraState.CloseUp)
        {
            safeInspectButton.gameObject.SetActive(true);
        }
        else
        {
            safeInspectButton.gameObject.SetActive(false);
        }
    }
}