using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Collections;

public class AnswerTheCallManager : MonoBehaviour
{
    [Header("References")]
    public AudioSource audioSource;
    public GameObject worldSpaceImage;
    public GameObject dialogueUI;
    public Text objectiveText;
    public Light flickeringLight; // TV light

    [Header("Flashing Settings")]
    public float flashInterval = 0.5f; // Image flash interval

    [Header("Light Flickering Settings")]
    public float minFlickerInterval = 0.05f;
    public float maxFlickerInterval = 0.3f;

    private bool isPlaying = false;
    private Coroutine flashingCoroutine;
    private Coroutine lightFlickerCoroutine;

    private void Start()
    {
        if (flickeringLight != null)
        {
            lightFlickerCoroutine = StartCoroutine(FlickerLight());
        }
    }

    void Update()
    {
        if (objectiveText != null && objectiveText.text == "Answer the call")
        {
            if (!isPlaying)
            {
                PlaySoundAndShowImage();
            }
        }
        else
        {
            if (isPlaying)
            {
                StopSoundAndHideImage();
            }
        }

        if (dialogueUI != null && dialogueUI.activeInHierarchy)
        {
            if (isPlaying)
            {
                StopSoundAndHideImage();
            }
        }
    }

    private void PlaySoundAndShowImage()
    {
        if (audioSource != null && !audioSource.isPlaying)
        {
            audioSource.Play();
        }
        if (worldSpaceImage != null)
        {
            flashingCoroutine = StartCoroutine(FlashImage());
        }
        isPlaying = true;
    }

    private void StopSoundAndHideImage()
    {
        if (audioSource != null && audioSource.isPlaying)
        {
            audioSource.Stop();
        }
        if (worldSpaceImage != null)
        {
            if (flashingCoroutine != null)
            {
                StopCoroutine(flashingCoroutine);
            }
            worldSpaceImage.SetActive(false);
        }
        isPlaying = false;
    }

    private IEnumerator FlashImage()
    {
        while (true)
        {
            worldSpaceImage.SetActive(!worldSpaceImage.activeSelf);
            yield return new WaitForSeconds(flashInterval);
        }
    }

    private IEnumerator FlickerLight()
    {
        while (true)
        {
            flickeringLight.enabled = !flickeringLight.enabled;
            float randomInterval = Random.Range(minFlickerInterval, maxFlickerInterval);
            yield return new WaitForSeconds(randomInterval);
        }
    }
}
