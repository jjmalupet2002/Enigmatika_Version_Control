using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using UnityEngine;

public class GhostAI : MonoBehaviour
{
    [Header("References")]
    public GameObject player;
    public CanvasGroup blackBackground;

    [Header("Initial Positions")]
    public Transform playerResetPosition;
    public Transform ghostResetPosition;

    [Header("Waypoints for Wandering")]
    public List<Transform> wanderPoints;

    [Header("Ranges")]
    public float interactRange = 2f;       // Kill range
    public float playerProximity = 10f;    // Detection range
    public float musicTriggerRange = 8f;   // Range to trigger ghost music

    [Header("States")]
    public bool CloakIsOn = false;

    [Header("Speeds")]
    public float wanderSpeed = 2f;
    public float chaseSpeed = 5f;
    public float rotationSpeed = 5f;

    [Header("Timers")]
    public float wanderInterval = 3f;

    [Header("Cloak System")]
    public GameObject CloakEquipUI;
    public GameObject Cloak;

    [Header("Music")]
    public AudioSource backgroundMusic;
    public AudioSource ghostMusic;

    [Header("Triggers")]
    public GameObject ghostMusicTrigger;         // A GameObject with collider or just a position marker
    public GameObject chaseInstruction;          // Instruction object to be shown and then disabled

    private bool isChasing = false;
    private bool isFading = false;
    private float wanderTimer;
    private Transform currentWanderTarget;

    private bool hasTriggeredMusic = false;
    private bool chaseInstructionPreviouslyActive = false;
    private bool ghostActivated = false;

    void Start()
    {
        PickNewWanderPoint();
        wanderTimer = wanderInterval;

        if (backgroundMusic != null)
            backgroundMusic.Play();

        if (ghostMusic != null)
            ghostMusic.Stop();

        if (chaseInstruction != null)
            chaseInstructionPreviouslyActive = chaseInstruction.activeSelf;
    }

    void Update()
    {
        if (isFading) return;

        CloakIsEquipped();

        float distanceToPlayer = Vector3.Distance(transform.position, player.transform.position);

        // MUSIC LOGIC
        if (ghostMusicTrigger != null && !hasTriggeredMusic)
        {
            float distToTrigger = Vector3.Distance(player.transform.position, ghostMusicTrigger.transform.position);
            if (distToTrigger <= musicTriggerRange)
            {
                hasTriggeredMusic = true;
                if (ghostMusic != null && !ghostMusic.isPlaying)
                    ghostMusic.Play();

                if (backgroundMusic != null && backgroundMusic.isPlaying)
                    backgroundMusic.Stop();

            }
        }

        // CHASE INSTRUCTION LOGIC
        if (!ghostActivated && chaseInstruction != null)
        {
            bool currentState = chaseInstruction.activeSelf;

            if (chaseInstructionPreviouslyActive && !currentState)
            {
                ghostActivated = true;
            }

            chaseInstructionPreviouslyActive = currentState;
        }

        if (!ghostActivated) return; // Ghost is inactive until instruction triggers it

        // KILL CHECK
        if (!CloakIsOn && distanceToPlayer <= interactRange)
        {
            StartCoroutine(HandleKillSequence());
            return;
        }

        // CLOAKED BEHAVIOR
        if (CloakIsOn)
        {
            if (isChasing)
            {
                isChasing = false;
                PickNewWanderPoint();
                wanderTimer = wanderInterval;
            }

            HandleWandering();
            return;
        }

        // NORMAL BEHAVIOR
        if (distanceToPlayer <= playerProximity)
        {
            isChasing = true;
            ChasePlayer();
        }
        else
        {
            if (isChasing)
            {
                isChasing = false;
                PickNewWanderPoint();
                wanderTimer = wanderInterval;
            }

            HandleWandering();
        }
    }

    void ChasePlayer()
    {
        Vector3 direction = (player.transform.position - transform.position).normalized;
        transform.position = Vector3.MoveTowards(transform.position, player.transform.position, chaseSpeed * Time.deltaTime);
        RotateTowards(direction);
    }

    void HandleWandering()
    {
        if (currentWanderTarget == null) return;

        Vector3 direction = (currentWanderTarget.position - transform.position).normalized;
        transform.position = Vector3.MoveTowards(transform.position, currentWanderTarget.position, wanderSpeed * Time.deltaTime);
        RotateTowards(direction);

        wanderTimer -= Time.deltaTime;
        if (Vector3.Distance(transform.position, currentWanderTarget.position) < 0.5f || wanderTimer <= 0f)
        {
            PickNewWanderPoint();
            wanderTimer = wanderInterval;
        }
    }

    void PickNewWanderPoint()
    {
        if (wanderPoints.Count == 0) return;
        currentWanderTarget = wanderPoints[UnityEngine.Random.Range(0, wanderPoints.Count)];
    }

    void RotateTowards(Vector3 direction)
    {
        if (direction == Vector3.zero) return;
        Quaternion targetRotation = Quaternion.LookRotation(new Vector3(direction.x, 0, direction.z));
        transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, rotationSpeed * Time.deltaTime);
    }

    IEnumerator HandleKillSequence()
    {
        isFading = true;

        yield return StartCoroutine(FadeCanvas(blackBackground, 0f, 1f, 0.5f));

        player.transform.position = playerResetPosition.position;
        transform.position = ghostResetPosition.position;

        isChasing = false;
        PickNewWanderPoint();
        wanderTimer = wanderInterval;

        yield return StartCoroutine(FadeCanvas(blackBackground, 1f, 0f, 1.2f));
        isFading = false;
    }

    IEnumerator FadeCanvas(CanvasGroup canvas, float startAlpha, float endAlpha, float duration)
    {
        float elapsed = 0f;
        canvas.alpha = startAlpha;
        canvas.blocksRaycasts = true;

        while (elapsed < duration)
        {
            canvas.alpha = Mathf.Lerp(startAlpha, endAlpha, elapsed / duration);
            elapsed += Time.deltaTime;
            yield return null;
        }

        canvas.alpha = endAlpha;
        if (endAlpha == 0f)
            canvas.blocksRaycasts = false;
    }

    void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, interactRange);

        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position, playerProximity);

        Gizmos.color = Color.cyan;
        if (ghostMusicTrigger != null)
            Gizmos.DrawWireSphere(ghostMusicTrigger.transform.position, musicTriggerRange);
    }

    void CloakIsEquipped()
    {
        bool equipUIActive = CloakEquipUI != null && CloakEquipUI.activeSelf;
        bool cloakActive = Cloak != null && Cloak.activeSelf;

        if (equipUIActive)
        {
            if (Cloak != null && !cloakActive)
                Cloak.SetActive(true);
        }
        else
        {
            if (Cloak != null && cloakActive)
                Cloak.SetActive(false);
        }

        CloakIsOn = Cloak != null && Cloak.activeSelf;
    }
}
