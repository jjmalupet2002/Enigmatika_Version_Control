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
    public AudioSource backgroundMusic;
    public CanvasGroup blackScreenCanvasGroup;

    private NavMeshAgent enemyAgent;
    private Rigidbody enemyRigidbody;
    private Rigidbody playerRigidbody;

    [Header("Chase Settings")]
    public float detectionRadius = 5f; // Radius for player detection
    public float chaseSpeed = 5f;
    public float fadeDuration = 1.5f;
    public float chaseCooldown = 2f;

    private bool isChasing = false;
    private bool isOnCooldown = false;

    // Reference to the ShootingToggle script
    private ShootingToggle shootingToggle;

    void Start()
    {
        enemyAgent = enemy.GetComponent<NavMeshAgent>();
        enemyRigidbody = enemy.GetComponent<Rigidbody>();
        playerRigidbody = player.GetComponent<Rigidbody>();

        shootingToggle = player.GetComponent<ShootingToggle>(); // Get the ShootingToggle component on the player

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
            EnableShooting();
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
        if (!isChasing && !isOnCooldown)
        {
            isChasing = true;

            if (backgroundMusic != null && backgroundMusic.isPlaying)
            {
                backgroundMusic.Stop();
            }

            if (chaseMusic != null && !chaseMusic.isPlaying)
            {
                chaseMusic.Play();
            }
        }
    }

    private void EnableShooting()
    {
        if (shootingToggle != null)
        {
            shootingToggle.GetType().GetField("enableShooting", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance).SetValue(shootingToggle, true); // Enable shooting
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

        player.GetComponent<PlayerJoystickControl>().SetInputEnabled(false);

        yield return StartCoroutine(FadeScreen(1)); // Fade to black

        ResetPlayerPosition();
        ResetEnemyPosition();

        yield return new WaitForSeconds(0.5f);
        yield return StartCoroutine(FadeScreen(0)); // Fade back to normal

        yield return new WaitForSeconds(chaseCooldown);
        isOnCooldown = false;
        player.GetComponent<PlayerJoystickControl>().SetInputEnabled(true);

        if (backgroundMusic != null)
        {
            backgroundMusic.Play();
        }
    }

    private void ResetPlayerPosition()
    {
        if (playerRigidbody != null)
        {
            playerRigidbody.velocity = Vector3.zero;
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
