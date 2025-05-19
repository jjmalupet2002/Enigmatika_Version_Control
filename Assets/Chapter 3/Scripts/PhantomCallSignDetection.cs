using UnityEngine;
using System.Collections;
using System.Diagnostics;
using UnityEngine.UI;

public class PhantomCallSignDetection : MonoBehaviour
{
    public GameObject PhantomCallSignImage1;
    public GameObject PhantomCallSignImage2;
    public GameObject PhantomCallSignImage3;
    public GameObject PhantomCallSignImage4;

    public GameObject DetectiveDialogue1;
    public GameObject DetectiveDialogue2;
    public GameObject DetectiveDialogue3;
    public GameObject DetectiveDialogue4;

    private void Update()
    {
        // Check if any of the PhantomCallSignImages are enabled and enable the corresponding DetectiveDialogue
        if (PhantomCallSignImage1.activeInHierarchy)
        {
            EnableDialogue(DetectiveDialogue1);
        }
        else if (PhantomCallSignImage2.activeInHierarchy)
        {
            EnableDialogue(DetectiveDialogue2);
        }
        else if (PhantomCallSignImage3.activeInHierarchy)
        {
            EnableDialogue(DetectiveDialogue3);
        }
        else if (PhantomCallSignImage4.activeInHierarchy)
        {
            EnableDialogue(DetectiveDialogue4);
        }
    }

    // Method to enable the dialogue and start the fade-out timer
    private void EnableDialogue(GameObject dialogue)
    {
        dialogue.SetActive(true);

        // Start the fade-out and disable sequence after 10 seconds
        StartCoroutine(FadeOutAndDisable(dialogue));
    }

    // Coroutine to fade out and disable the dialogue after 10 seconds
    private IEnumerator FadeOutAndDisable(GameObject dialogue)
    {
        CanvasGroup canvasGroup = dialogue.GetComponent<CanvasGroup>();

        if (canvasGroup == null)
        {
            // If there's no CanvasGroup, add one for fading effect
            canvasGroup = dialogue.AddComponent<CanvasGroup>();
        }

        // Fade-in
        canvasGroup.alpha = 1f;

        // Wait for 10 seconds
        yield return new WaitForSeconds(10f);

        // Fade-out
        float fadeTime = 1f;
        float elapsedTime = 0f;

        while (elapsedTime < fadeTime)
        {
            canvasGroup.alpha = Mathf.Lerp(1f, 0f, elapsedTime / fadeTime);
            elapsedTime += Time.deltaTime;
            yield return null;
        }

        // Ensure it's fully faded out
        canvasGroup.alpha = 0f;

        // Disable the dialogue after fade-out
        dialogue.SetActive(false);
    }
}
