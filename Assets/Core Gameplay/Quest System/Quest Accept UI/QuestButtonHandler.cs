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
        bool dialogueCurrentlyActive = false;

        if (dialogueUI != null)
        {
            dialogueCurrentlyActive = dialogueUI.activeSelf;

            if (!hasTalkedToNPC && dialogueWasActiveLastFrame && !dialogueCurrentlyActive)
            {
                if (playerTransform != null)
                {
                    float distance = Vector3.Distance(transform.position, playerTransform.position);
                    if (distance <= interactRange)
                    {
                        hasTalkedToNPC = true;
                        TriggerQuestButtonBlink();
                    }
                }
            }

            dialogueWasActiveLastFrame = dialogueCurrentlyActive;
        }

        if (playerTransform != null)
        {
            float distance = Vector3.Distance(transform.position, playerTransform.position);

            if (distance <= interactRange)
            {
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
        Color blinkColor = new Color(152f / 255f, 67f / 255f, 37f / 255f, 0.5f);

        float duration = 2.6f;
        float blinkRate = 0.3f;
        float timer = 0f;

        openQuestButton.SetActive(true);

        while (timer < duration)
        {
            if (playerTransform == null || Vector3.Distance(transform.position, playerTransform.position) > interactRange)
            {
                openQuestButton.SetActive(false);
                blinkingInProgress = false;
                yield break; // Stop blinking early if player is too far
            }

            questButtonImage.color = blinkColor;
            yield return new WaitForSeconds(blinkRate);

            if (playerTransform == null || Vector3.Distance(transform.position, playerTransform.position) > interactRange)
            {
                openQuestButton.SetActive(false);
                blinkingInProgress = false;
                yield break;
            }

            questButtonImage.color = originalColor;
            yield return new WaitForSeconds(blinkRate);
            timer += blinkRate * 2;
        }

        // Final check in case the player moved away at the end of blinking
        if (playerTransform == null || Vector3.Distance(transform.position, playerTransform.position) > interactRange)
        {
            openQuestButton.SetActive(false);
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
