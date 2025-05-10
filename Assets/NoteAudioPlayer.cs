using System.Collections.Generic;
using System.Diagnostics;
using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(NoteObjectHandler))]
public class NoteAudioPlayer : MonoBehaviour
{
    [Header("UI Buttons")]
    public GameObject playButtonObj;    // UI object containing the Play Button
    public GameObject pauseButtonObj;   // UI object containing the Pause Button

    private Button playButton;
    private Button pauseButton;

    public NoteObjectHandler noteHandler;
    public AudioSource audioSource;

    private void Awake()
    {
        noteHandler = GetComponent<NoteObjectHandler>();
        audioSource = gameObject.AddComponent<AudioSource>();
        audioSource.playOnAwake = false;

        if (noteHandler == null)
        {
            enabled = false;
            return;
        }

        if (noteHandler.noteAudioClip == null)
        {
            return;
        }
        else
        {
            audioSource.clip = noteHandler.noteAudioClip;
        }

        if (noteHandler.notePages == null || noteHandler.notePages.Count == 0)
        {
            return;
        }

        if (playButtonObj != null)
        {
            playButton = playButtonObj.GetComponent<Button>();
            playButtonObj.SetActive(false); // Hide by default
        }

        if (pauseButtonObj != null)
        {
            pauseButton = pauseButtonObj.GetComponent<Button>();
            pauseButtonObj.SetActive(false); // Hide by default
        }
    }

    private void OnEnable()
    {
        if (playButton != null)
            playButton.onClick.AddListener(PlayAudio);

        if (pauseButton != null)
            pauseButton.onClick.AddListener(PauseAudio);
    }

    private void OnDisable()
    {
        if (playButton != null)
            playButton.onClick.RemoveListener(PlayAudio);

        if (pauseButton != null)
            pauseButton.onClick.RemoveListener(PauseAudio);
    }

    private void Update()
    {
        if (noteHandler == null || noteHandler.notePages == null || noteHandler.notePages.Count == 0)
            return;

        bool anyPageActive = false;
        foreach (GameObject page in noteHandler.notePages)
        {
            if (page != null && page.activeInHierarchy)
            {
                anyPageActive = true;
                break;
            }
        }

        // Show/Hide buttons based on note activity and playback state
        if (playButtonObj != null)
            playButtonObj.SetActive(anyPageActive && !audioSource.isPlaying);

        if (pauseButtonObj != null)
            pauseButtonObj.SetActive(audioSource.isPlaying);

        // Stop audio if all note pages are inactive
        if (!anyPageActive && audioSource.isPlaying)
        {
            audioSource.Stop();
        }
    }

    private void PlayAudio()
    {
        if (noteHandler.noteAudioClip == null || !AnyPageActive())
        {
            return;
        }

        if (!audioSource.isPlaying)
        {
            audioSource.clip = noteHandler.noteAudioClip;
            audioSource.Play();
        }
    }

    private void PauseAudio()
    {
        if (audioSource.isPlaying)
        {
            audioSource.Pause();
        }
    }

    private bool AnyPageActive()
    {
        foreach (GameObject page in noteHandler.notePages)
        {
            if (page != null && page.activeInHierarchy)
                return true;
        }
        return false;
    }
}
