using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class TipSystem : MonoBehaviour
{
    [Header("UI Elements")]
    public GameObject tipButton;
    public GameObject tipUI;
    public Text tipText;
    public Button exitTipButton;

    [Header("Tip Management")]
    public List<string> tips = new List<string>();
    private int currentTipIndex = -1;

    [Header("Timing")]
    public float tipCooldown = 15f;
    private float nextTipTime = 0f;

    [Header("Audio")]
    public AudioSource audioSource;
    public AudioClip tipOpenSound;
    public AudioClip tipCloseSound;
    public AudioClip tipButtonAppearSound;

    private bool tipButtonShown = false;

    [Header("Tip Button Animation")]
    public float minScale = 0.9f;
    public float maxScale = 1.1f;
    public float pulseDuration = 3f;
    public float pulseSpeed = 2f;

    void Start()
    {
        tipUI.SetActive(false);
        tipButton.SetActive(false);
        exitTipButton.onClick.AddListener(CloseTipUI);

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
        if (Time.time >= nextTipTime && !tipButtonShown)
        {
            tipButton.SetActive(true);
            tipButtonShown = true;

            // Play sound
            if (audioSource != null && tipButtonAppearSound != null)
                audioSource.PlayOneShot(tipButtonAppearSound);

            // Start the pulse animation
            StartCoroutine(PulseTipButton());
        }
    }

    public void ShowTipUI()
    {
        RandomizeTip();
        tipUI.SetActive(true);
        tipButton.SetActive(false);
        tipButtonShown = false;

        if (audioSource != null && tipOpenSound != null)
            audioSource.PlayOneShot(tipOpenSound);

        nextTipTime = Time.time + tipCooldown;
    }

    public void CloseTipUI()
    {
        tipUI.SetActive(false);

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

    private IEnumerator PulseTipButton()
    {
        float timer = 0f;

        while (timer < pulseDuration)
        {
            float scale = Mathf.Lerp(minScale, maxScale, (Mathf.Sin(Time.time * pulseSpeed) + 1f) / 2f);
            tipButton.transform.localScale = new Vector3(scale, scale, 1f);
            timer += Time.deltaTime;
            yield return null;
        }

        // Reset to default scale after animation
        tipButton.transform.localScale = Vector3.one;
    }
}
