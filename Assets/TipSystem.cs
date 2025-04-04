using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using UnityEngine;
using UnityEngine.UI;

public class TipSystem : MonoBehaviour
{
    [Header("UI Elements")]
    public GameObject tipButton;        // The button that triggers the Tip UI
    public GameObject tipUI;            // The UI that shows the tip
    public Text tipText;                // Text element to display the tip
    public Button exitTipButton;        // Button to close the tip UI

    [Header("Tip Management")]
    public List<string> tips = new List<string>(); // List of tips
    private int currentTipIndex = -1;

    [Header("Timing")]
    private float tipCooldown = 50f;  // 50 seconds for testing
    private float nextTipTime = 0f;

    [Header("Audio")]
    public AudioSource audioSource;
    public AudioClip tipOpenSound;
    public AudioClip tipCloseSound;
    public AudioClip tipButtonAppearSound;

    private bool tipButtonShown = false;

    void Start()
    {
        tipUI.SetActive(false);
        tipButton.SetActive(false);
        exitTipButton.onClick.AddListener(CloseTipUI);

        // Add the ShowTipUI listener to the tipButton OnClick
        Button tipButtonComponent = tipButton.GetComponent<Button>();
        if (tipButtonComponent != null)
        {
            tipButtonComponent.onClick.AddListener(ShowTipUI);
        }

        RandomizeTip();
        nextTipTime = Time.time + tipCooldown;
    }

    void Update()
    {
        // Debug log to confirm if the tip button should appear now
        if (Time.time >= nextTipTime && !tipButtonShown)
        {
            tipButton.SetActive(true);
            tipButtonShown = true;
            UnityEngine.Debug.Log("Tip button should appear now"); // Added debug log

            // Play the tip button appear sound
            if (audioSource != null && tipButtonAppearSound != null)
                audioSource.PlayOneShot(tipButtonAppearSound);
        }
    }

    public void ShowTipUI()
    {
        RandomizeTip();
        tipUI.SetActive(true);
        tipButton.SetActive(false);
        tipButtonShown = false;

        // Play open sound
        if (audioSource != null && tipOpenSound != null)
            audioSource.PlayOneShot(tipOpenSound);

        nextTipTime = Time.time + tipCooldown;
    }

    public void CloseTipUI()
    {
        tipUI.SetActive(false);

        // Play close sound
        if (audioSource != null && tipCloseSound != null)
            audioSource.PlayOneShot(tipCloseSound);
    }

    private void RandomizeTip()
    {
        if (tips != null && tips.Count > 0)
        {
            currentTipIndex = UnityEngine.Random.Range(0, tips.Count);
            tipText.text = tips[currentTipIndex];
        }
        else
        {
            tipText.text = "No tips available.";
        }
    }
}
