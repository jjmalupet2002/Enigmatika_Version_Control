using System.Collections.Generic;
using UnityEngine;

public class MusicPlay : MonoBehaviour
{
    [Header("UI References")]
    public List<GameObject> uiElements; // List of UI elements to check

    [Header("Audio Settings")]
    public AudioSource audioSource;
    public AudioClip musicClip;

    private void Start()
    {
        if (audioSource == null)
        {
            audioSource = gameObject.AddComponent<AudioSource>();
        }
        audioSource.clip = musicClip;
        audioSource.loop = true; // Ensures continuous playback
    }

    private void Update()
    {
        if (IsAnyUIActive())
        {
            if (!audioSource.isPlaying)
            {
                audioSource.Play();
            }
        }
        else
        {
            if (audioSource.isPlaying)
            {
                audioSource.Stop();
            }
        }
    }

    private bool IsAnyUIActive()
    {
        foreach (GameObject uiElement in uiElements)
        {
            if (uiElement != null && uiElement.activeSelf)
            {
                return true;
            }
        }
        return false;
    }
}
