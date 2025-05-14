using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class EnemyTrapScript : MonoBehaviour
{
    [Header("Trap Gate Settings")]
    public GameObject trapGate;
    public float startYPosition;
    public float targetYPosition;
    public float loweringSpeed = 2f;

    [Header("Lever Settings")]
    public GameObject leverObject;
    public Animator leverAnimator;

    [Header("Player and UI")]
    public GameObject player;
    public Button interactButton;

    [Header("Detection Settings")]
    public float interactionDistance = 2f;
    public float detectionRadius = 3f;

    [Header("Manual Testing")]
    public bool triggerLoweringManually = false;
    public bool resetTrapGate = false; // ✅ NEW reset trigger

    private bool isPlayerNearLever = false;
    private bool isLoweringTrap = false;
    private bool isRaisingTrap = false; // ✅ NEW state flag

    [Header("Projectile Control")]
    public List<ShootingToggle> shootingToggles; // Assign all relevant toggles in the inspector

    [Header("Sound Settings")]
    public AudioClip leverSound;
    private AudioSource audioSource;

    private void Start()
    {
        if (interactButton != null)
        {
            interactButton.onClick.AddListener(OnInteractPressed);
        }

        if (trapGate != null)
        {
            Vector3 pos = trapGate.transform.position;
            startYPosition = pos.y;
        }

        audioSource = GetComponent<AudioSource>();
        if (audioSource == null)
        {
            audioSource = gameObject.AddComponent<AudioSource>();
        }
    }

    private void Update()
    {
        // Manual trigger
        if (triggerLoweringManually)
        {
            isLoweringTrap = true;
            isRaisingTrap = false;
            triggerLoweringManually = false;
        }

        // Reset trigger
        if (resetTrapGate)
        {
            isRaisingTrap = true;
            isLoweringTrap = false;
            resetTrapGate = false;
        }

        // Proximity detection
        if (leverObject != null && player != null)
        {
            Collider[] hitColliders = Physics.OverlapSphere(leverObject.transform.position, detectionRadius);
            isPlayerNearLever = false;

            foreach (var hitCollider in hitColliders)
            {
                if (hitCollider.gameObject == player)
                {
                    isPlayerNearLever = true;
                    break;
                }
            }
        }

        // Lowering trap
        if (isLoweringTrap && trapGate != null)
        {
            Vector3 currentPos = trapGate.transform.position;
            float newY = Mathf.MoveTowards(currentPos.y, targetYPosition, loweringSpeed * Time.deltaTime);
            trapGate.transform.position = new Vector3(currentPos.x, newY, currentPos.z);

            if (Mathf.Approximately(newY, targetYPosition))
            {
                isLoweringTrap = false;
            }
        }

        // Raising trap (reset)
        if (isRaisingTrap && trapGate != null)
        {
            Vector3 currentPos = trapGate.transform.position;
            float newY = Mathf.MoveTowards(currentPos.y, startYPosition, loweringSpeed * Time.deltaTime);
            trapGate.transform.position = new Vector3(currentPos.x, newY, currentPos.z);

            if (Mathf.Approximately(newY, startYPosition))
            {
                isRaisingTrap = false;
            }
        }
    }

    private void OnInteractPressed()
    {
        if (isPlayerNearLever)
        {
            leverAnimator?.SetTrigger("LeverDown");
            isLoweringTrap = true;
            isRaisingTrap = false;

            // Play sound
            if (leverSound != null && audioSource != null)
            {
                audioSource.PlayOneShot(leverSound);
            }

            // Disable all shooting toggles
            foreach (var toggle in shootingToggles)
            {
                if (toggle != null)
                    toggle.SetShootingEnabled(false);
            }
        }
    }

    private void OnDrawGizmos()
    {
        if (leverObject != null)
        {
            Gizmos.color = Color.red;
            Gizmos.DrawWireSphere(leverObject.transform.position, detectionRadius);
        }
    }
}
