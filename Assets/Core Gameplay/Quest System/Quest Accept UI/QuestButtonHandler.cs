using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class QuestButtonHandler : MonoBehaviour
{
    [Header("Quest Button Settings")]
    public GameObject openQuestButton; // Reference to the Quest button
    public float interactRange = 5f;   // Interaction range for the NPC (visualization)

    [Header("Dialogue UI Reference")]
    public GameObject dialogueUI; // Reference to the Dialogue UI

    private Transform playerTransform;
    private Collider npcCollider;

    private bool dialogueWasActiveLastFrame = false;
    private bool hasTalkedToNPC = false;
    private bool blinkingInProgress = false;

    private Image questButtonImage;
    private Coroutine blinkCoroutine;

    void Start()
    {
        npcCollider = GetComponent<Collider>();

        if (openQuestButton != null)
        {
            questButtonImage = openQuestButton.GetComponent<Image>();
            openQuestButton.SetActive(false);
        }
    }

    void Update()
    {
        if (dialogueUI != null)
        {
            bool dialogueCurrentlyActive = dialogueUI.activeSelf;

            if (!hasTalkedToNPC && dialogueWasActiveLastFrame && !dialogueCurrentlyActive)
            {
                hasTalkedToNPC = true;
                TriggerQuestButtonBlink();
            }

            dialogueWasActiveLastFrame = dialogueCurrentlyActive;
        }

        if (playerTransform != null)
        {
            float distance = Vector3.Distance(transform.position, playerTransform.position);

            if (distance <= interactRange)
            {
                // After NPC is talked to once, allow normal quest button behavior
                if (hasTalkedToNPC && !blinkingInProgress)
                {
                    openQuestButton.SetActive(true);
                }
            }
            else
            {
                if (!blinkingInProgress)
                    openQuestButton.SetActive(false);
            }
        }
    }

    void TriggerQuestButtonBlink()
    {
        if (blinkCoroutine != null)
            StopCoroutine(blinkCoroutine);

        blinkCoroutine = StartCoroutine(BlinkQuestButton());
    }

    IEnumerator BlinkQuestButton()
    {
        blinkingInProgress = true;

        Color originalColor = questButtonImage.color;
        // Color #984325 with semi-transparency (Alpha = 0.5)
        Color blinkColor = new Color(152f / 255f, 67f / 255f, 37f / 255f, 0.5f);

        float duration = 2.6f;
        float blinkRate = 0.3f;
        float timer = 0f;

        openQuestButton.SetActive(true);

        while (timer < duration)
        {
            questButtonImage.color = blinkColor;
            yield return new WaitForSeconds(blinkRate);
            questButtonImage.color = originalColor;
            yield return new WaitForSeconds(blinkRate);
            timer += blinkRate * 2;
        }

        questButtonImage.color = originalColor;
        blinkingInProgress = false;
    }

    void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.CompareTag("Player"))
        {
            playerTransform = collision.transform;
        }
    }

    void OnCollisionExit(Collision collision)
    {
        if (collision.gameObject.CompareTag("Player"))
        {
            playerTransform = null;
            if (!blinkingInProgress)
                openQuestButton.SetActive(false);
        }
    }

    void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position, interactRange);
    }
}
