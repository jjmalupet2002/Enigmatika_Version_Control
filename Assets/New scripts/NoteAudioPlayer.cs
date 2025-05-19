using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class NoteAudioPlayer : MonoBehaviour
{
    [System.Serializable]
    public class NoteEntry
    {
        public GameObject noteUI;   // The UI object for the note
        public AudioClip audioClip; // The associated audio clip
    }

    public List<NoteEntry> noteEntries = new List<NoteEntry>(); // List to hold all entries
    public Button playButton;  // Play button
    public Button pauseButton; // Pause button
    private AudioSource audioSource; // AudioSource to play the clip

    private bool wasAnyNoteUIActive = false; // Flag to track if any note UI was active

    private void Start()
    {
        audioSource = GetComponent<AudioSource>();

        // Add listeners to the buttons
        playButton.onClick.AddListener(PlayAudio);
        pauseButton.onClick.AddListener(PauseAudio);

        // Initially hide the buttons (already done in inspector, so this may be redundant)
        playButton.gameObject.SetActive(false);
        pauseButton.gameObject.SetActive(false);
    }

    private void Update()
    {
        // Update button visibility based on active noteUI
        SetButtonVisibility();
    }

    private void SetButtonVisibility()
    {
        bool isAnyNoteUIActive = false;

        foreach (var entry in noteEntries)
        {
            // Check if any note UI is active
            if (entry.noteUI.activeSelf)
            {
                isAnyNoteUIActive = true;
                break;
            }
        }

        // If the state has changed, update the button visibility
        if (isAnyNoteUIActive != wasAnyNoteUIActive)
        {
            if (isAnyNoteUIActive)
            {
                playButton.gameObject.SetActive(true);  // Show play button
                pauseButton.gameObject.SetActive(false); // Hide pause button
            }
            else
            {
                playButton.gameObject.SetActive(false); // Hide play button
                pauseButton.gameObject.SetActive(false); // Hide pause button
            }

            // Update the flag to the new state
            wasAnyNoteUIActive = isAnyNoteUIActive;
        }
    }

    private void PlayAudio()
    {
        foreach (var entry in noteEntries)
        {
            // Play audio if the note UI is active
            if (entry.noteUI.activeSelf)
            {
                audioSource.clip = entry.audioClip;
                audioSource.Play();
                playButton.gameObject.SetActive(false); // Hide play button after playing
                pauseButton.gameObject.SetActive(true); // Show pause button
                break;
            }
        }
    }

    private void PauseAudio()
    {
        if (audioSource.isPlaying)
        {
            audioSource.Pause();
            pauseButton.gameObject.SetActive(false); // Hide pause button after pausing
            playButton.gameObject.SetActive(true);  // Show play button again
        }
    }
}
