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

    private bool isPlayerNearLever = false;
    private bool isLoweringTrap = false;

    private void Start()
    {
        if (interactButton != null)
        {
            interactButton.onClick.AddListener(OnInteractPressed);
        }

        // Ensure the trap gate starts at the defined Y position
        if (trapGate != null)
        {
            Vector3 pos = trapGate.transform.position;
            pos.y = startYPosition;
            trapGate.transform.position = pos;
        }
    }

    private void Update()
    {
        // Check distance between player and lever
        if (player != null && leverObject != null)
        {
            float distance = Vector3.Distance(player.transform.position, leverObject.transform.position);
            isPlayerNearLever = distance <= interactionDistance;
        }

        // Smoothly lower trap gate
        if (isLoweringTrap && trapGate != null)
        {
            Vector3 currentPos = trapGate.transform.position;
            float newY = Mathf.Lerp(currentPos.y, targetYPosition, Time.deltaTime * loweringSpeed);
            trapGate.transform.position = new Vector3(currentPos.x, newY, currentPos.z);

            // Stop lowering if close enough to target Y
            if (Mathf.Abs(currentPos.y - targetYPosition) < 0.05f)
            {
                isLoweringTrap = false;
            }
        }
    }

    private void OnInteractPressed()
    {
        if (isPlayerNearLever)
        {
            if (leverAnimator != null)
            {
                leverAnimator.SetTrigger("LeverDown");
            }

            isLoweringTrap = true;
        }
    }
}
