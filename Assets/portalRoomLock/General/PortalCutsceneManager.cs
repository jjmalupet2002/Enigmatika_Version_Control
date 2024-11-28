using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class PortalCutsceneManager : MonoBehaviour
{
    public GameObject page1;
    public Image cutsceneImage1;
    public Button endButton;
    public float interactRange = 1f; // Public interact range
    public Transform player;
    public Collider interactCollider;

    private bool isPlayerInRange = false;
    private bool isCutsceneComplete = false;

    void Start()
    {
        // Ensure the cutscene page is inactive at the start
        page1.SetActive(false);

        // Add listener to the end button
        endButton.onClick.AddListener(EndCutscene);
    }

    void Update()
    {
        if (isCutsceneComplete) return;

        // Check if the player is within the interact range
        Collider[] hitColliders = Physics.OverlapSphere(interactCollider.transform.position, interactRange);
        foreach (var hitCollider in hitColliders)
        {
            if (hitCollider.CompareTag("Player"))
            {
                if (!isPlayerInRange)
                {
                    // Player entered the range
                    isPlayerInRange = true;
                    StartCutscene();
                }
                return;
            }
        }
        isPlayerInRange = false;
    }

    void StartCutscene()
    {
        // Enable the cutscene page and start the fade-in effect
        page1.SetActive(true);
        StartCoroutine(FadeInImage(cutsceneImage1));
    }

    void EndCutscene()
    {
        // Disable the cutscene page and mark the cutscene as complete
        page1.SetActive(false);
        isCutsceneComplete = true;
    }

    IEnumerator FadeInImage(Image image)
    {
        Color color = image.color;
        color.a = 0;
        image.color = color;

        while (color.a < 1)
        {
            color.a += Time.deltaTime / 1f; // Adjust the duration of the fade-in effect as needed
            image.color = color;
            yield return null;
        }
    }
}
