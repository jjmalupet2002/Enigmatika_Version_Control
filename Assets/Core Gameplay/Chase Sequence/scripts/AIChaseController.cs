using System.Collections;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.UI;

public class AIChaseController : MonoBehaviour
{
    [Header("References")]
    public Transform player;
    public Transform enemy;
    public Transform playerStartPosition;
    public Transform enemyStartPosition;
    public AudioSource chaseMusic;
    public AudioSource backgroundMusic; // Reference to background music
    public CanvasGroup blackScreenCanvasGroup; // Using CanvasGroup instead of Image

    private NavMeshAgent enemyAgent;
    private Rigidbody enemyRigidbody;
    private Rigidbody playerRigidbody;

    [Header("Chase Settings")]
    public float detectionRadius = 5f; // Radius for player detection
    public float chaseSpeed = 5f;
    public float fadeDuration = 1.5f; // Time for fade effect
    public float chaseCooldown = 2f; // Cooldown before AI chases again

    private bool isChasing = false;
    private bool isOnCooldown = false; // Flag to track cooldown state

    void Start()
    {
        enemyAgent = enemy.GetComponent<NavMeshAgent>();
        enemyRigidbody = enemy.GetComponent<Rigidbody>();
        playerRigidbody = player.GetComponent<Rigidbody>();

        if (enemyAgent == null)
        {
            UnityEngine.Debug.LogError("NavMeshAgent component is missing on the enemy GameObject.");
            return;
        }

        if (enemyRigidbody == null)
        {
            UnityEngine.Debug.LogError("Rigidbody component is missing on the enemy GameObject.");
            return;
        }

        if (playerRigidbody == null)
        {
            UnityEngine.Debug.LogError("Rigidbody component is missing on the player GameObject.");
            return;
        }

        enemyAgent.speed = chaseSpeed;
        enemyAgent.isStopped = true;

        if (blackScreenCanvasGroup != null)
            blackScreenCanvasGroup.alpha = 0f;
    }

    void Update()
    {
        if (!isChasing && !isOnCooldown)
        {
            DetectPlayer();
        }
        else if (isChasing)
        {
            ChasePlayer();
        }
    }

    private void DetectPlayer()
    {
        if (Vector3.Distance(player.position, enemy.position) <= detectionRadius)
        {
            StartChase();
        }
    }

    private void ChasePlayer()
    {
        if (enemyAgent != null)
        {
            enemyAgent.SetDestination(player.position);
            enemyAgent.isStopped = false;
        }
    }

    private void StartChase()
    {
        if (!isChasing && !isOnCooldown) // Ensure chase does not start during cooldown
        {
            isChasing = true;

            // Stop background music if it's playing
            if (backgroundMusic != null && backgroundMusic.isPlaying)
            {
                backgroundMusic.Stop();
            }

            // Start chase music if it's not playing
            if (chaseMusic != null && !chaseMusic.isPlaying)
            {
                chaseMusic.Play();
            }
        }
    }

    public void OnPlayerCaught()
    {
        StartCoroutine(HandlePlayerCaught());
    }

    private IEnumerator HandlePlayerCaught()
    {
        isChasing = false;
        isOnCooldown = true;

        if (chaseMusic != null)
            chaseMusic.Stop();

        if (enemyAgent != null)
            enemyAgent.isStopped = true;

        // **Disable Player Input Before Resetting**
        player.GetComponent<PlayerJoystickControl>().SetInputEnabled(false);

        yield return StartCoroutine(FadeScreen(1)); // Fade to black

        // Reset positions
        ResetPlayerPosition();
        ResetEnemyPosition();

        yield return new WaitForSeconds(0.5f);
        yield return StartCoroutine(FadeScreen(0)); // Fade back to normal

        // **Enable Player Input After Cooldown**
        yield return new WaitForSeconds(chaseCooldown);
        isOnCooldown = false;
        player.GetComponent<PlayerJoystickControl>().SetInputEnabled(true);

        // Resume background music when chase ends
        if (backgroundMusic != null)
        {
            backgroundMusic.Play();
        }
    }

    private void ResetPlayerPosition()
    {
        if (playerRigidbody != null)
        {
            // **Use MovePosition() instead of isKinematic**
            playerRigidbody.velocity = Vector3.zero; // Stop movement
            playerRigidbody.angularVelocity = Vector3.zero;
            playerRigidbody.MovePosition(playerStartPosition.position);
        }
        else
        {
            player.position = playerStartPosition.position;
        }
    }

    private void ResetEnemyPosition()
    {
        if (enemyAgent != null)
        {
            enemyAgent.Warp(enemyStartPosition.position);
            enemyAgent.isStopped = true;
        }
    }

    private IEnumerator FadeScreen(float targetAlpha)
    {
        float elapsedTime = 0f;
        float startAlpha = blackScreenCanvasGroup.alpha;

        while (elapsedTime < fadeDuration)
        {
            elapsedTime += Time.deltaTime;
            blackScreenCanvasGroup.alpha = Mathf.Lerp(startAlpha, targetAlpha, elapsedTime / fadeDuration);
            yield return null;
        }

        blackScreenCanvasGroup.alpha = targetAlpha;
    }
}
