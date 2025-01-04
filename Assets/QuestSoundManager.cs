using System.Collections.Generic;
using UnityEngine;

public class QuestSoundManager : MonoBehaviour
{
    public AudioClip mainQuestStartedSound;
    public AudioClip criteriaCompleteSound;
    public AudioClip mainQuestCompleteSound;

    [Range(0f, 1f)] public float mainQuestStartedVolume = 1f; // Volume for main quest started sound
    [Range(0f, 1f)] public float criteriaCompleteVolume = 1f; // Volume for criteria complete sound
    [Range(0f, 1f)] public float mainQuestCompleteVolume = 1f; // Volume for main quest complete sound

    private AudioSource audioSource;

    void Start()
    {
        // Initialize the AudioSource
        audioSource = GetComponent<AudioSource>();
        if (audioSource == null)
        {
            audioSource = gameObject.AddComponent<AudioSource>();
        }

        // Subscribe to QuestManager events
        QuestManager questManager = FindObjectOfType<QuestManager>();
        if (questManager != null)
        {
            questManager.OnQuestAcceptedEvent += PlayMainQuestStartedSound;
            questManager.OnNextCriteriaStartedEvent += PlayCriteriaCompleteSound;
            questManager.OnQuestCompletedEvent += PlayMainQuestCompleteSound;
        }
        else
        {
            UnityEngine.Debug.LogError("QuestManager not found in the scene.");
        }
    }

    private void OnDestroy()
    {
        // Unsubscribe from events to avoid memory leaks
        QuestManager questManager = FindObjectOfType<QuestManager>();
        if (questManager != null)
        {
            questManager.OnQuestAcceptedEvent -= PlayMainQuestStartedSound;
            questManager.OnNextCriteriaStartedEvent -= PlayCriteriaCompleteSound;
            questManager.OnQuestCompletedEvent -= PlayMainQuestCompleteSound;
        }
    }

    private void PlayMainQuestStartedSound(MainQuest quest)
    {
        PlaySound(mainQuestStartedSound, mainQuestStartedVolume);
    }

    private void PlayCriteriaCompleteSound(MainQuest quest)
    {
        // Check if the last criteria is being completed
        if (quest.questCriteriaList.TrueForAll(c => c.CriteriaStatus == QuestEnums.QuestCriteriaStatus.Completed))
        {
            return; // Don't play the criteria sound if it's the last one
        }

        PlaySound(criteriaCompleteSound, criteriaCompleteVolume);
    }

    private void PlayMainQuestCompleteSound(MainQuest quest)
    {
        PlaySound(mainQuestCompleteSound, mainQuestCompleteVolume);
    }

    private void PlaySound(AudioClip clip, float volume)
    {
        if (clip != null && audioSource != null)
        {
            audioSource.PlayOneShot(clip, volume);
        }
    }
}
