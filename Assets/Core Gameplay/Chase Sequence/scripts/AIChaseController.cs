using System.Collections;
using System.Diagnostics;
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
    public Transform enemyStopCheckpoint; // NEW

    public AudioSource chaseMusic;
    public AudioSource backgroundMusic;
    public CanvasGroup blackScreenCanvasGroup;

    private NavMeshAgent enemyAgent;
    private Rigidbody enemyRigidbody;
    private Rigidbody playerRigidbody;

    [Header("Chase Settings")]
    public float detectionRadius = 5f;
    public float chaseSpeed = 5f;
    public float fadeDuration = 1.5f;
    public float chaseCooldown = 2f;
    public float slowDownRadius = 2f;       // NEW
    public float stopSpeedThreshold = 0.1f; // NEW
    public float decelerationRate = 1.5f;   // NEW

    private float currentSpeed;

    private bool isChasing = false;
    private bool isOnCooldown = false;
    private bool isSlowingDown = false;

    private ShootingToggle shootingToggle;

    void Start()
    {
        enemyAgent = enemy.GetComponent<NavMeshAgent>();
        enemyRigidbody = enemy.GetComponent<Rigidbody>();
        playerRigidbody = player.GetComponent<Rigidbody>();

        // Assuming "ProjectileShoot" is a separate GameObject with ShootingToggle attached.
        // Find the GameObject and get the ShootingToggle component.
        GameObject projectileShootObject = GameObject.Find("ProjectileShoot"); // or reference directly if it's assigned
        if (projectileShootObject != null)
        {
            shootingToggle = projectileShootObject.GetComponent<ShootingToggle>();
        }

        if (enemyAgent == null || enemyRigidbody == null || playerRigidbody == null || shootingToggle == null)
        {
            UnityEngine.Debug.LogError("Missing required components.");
            return;
        }

        currentSpeed = chaseSpeed;
        enemyAgent.speed = currentSpeed;
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
        else if (isChasing && !isSlowingDown)
        {
            ChasePlayer();
            CheckForStopCheckpoint();
        }
        else if (isSlowingDown)
        {
            SlowDownAtCheckpoint();
        }
    }

    private void DetectPlayer()
    {
        if (Vector3.Distance(player.position, enemy.position) <= detectionRadius)
        {
            StartChase();
            EnableShooting(true);
            UnityEngine.Debug.Log("Shooting has been ENABLED.");
        }
    }

    private void ChasePlayer()
    {
        if (enemyAgent != null && player != null)
        {
            enemyAgent.SetDestination(player.position);
            enemyAgent.isStopped = false;
            enemyAgent.speed = chaseSpeed;
        }
    }

    private void StartChase()
    {
        if (!isChasing && !isOnCooldown)
        {
            isChasing = true;

            backgroundMusic?.Stop();
            if (chaseMusic != null && !chaseMusic.isPlaying)
                chaseMusic.Play();
        }
    }

    private void CheckForStopCheckpoint()
    {
        if (enemyStopCheckpoint != null &&
            Vector3.Distance(enemy.position, enemyStopCheckpoint.position) <= slowDownRadius)
        {
            isSlowingDown = true;
        }
    }

    private void SlowDownAtCheckpoint()
    {
        if (enemyAgent != null)
        {
            currentSpeed = Mathf.Lerp(currentSpeed, 0f, Time.deltaTime * decelerationRate);
            enemyAgent.speed = currentSpeed;

            if (currentSpeed <= stopSpeedThreshold)
            {
                enemyAgent.speed = 0f;
                enemyAgent.isStopped = true;

                EnableShooting(false);
                isChasing = false;
                isSlowingDown = false;
            }
        }
    }

    private void EnableShooting(bool value)
    {
        if (shootingToggle != null)
        {
            shootingToggle.SetShootingEnabled(value);
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

        chaseMusic?.Stop();
        enemyAgent.isStopped = true;

        player.GetComponent<PlayerJoystickControl>().SetInputEnabled(false);
        yield return StartCoroutine(FadeScreen(1));

        ResetPlayerPosition();
        ResetEnemyPosition();

        yield return new WaitForSeconds(0.5f);
        yield return StartCoroutine(FadeScreen(0));

        yield return new WaitForSeconds(chaseCooldown);
        isOnCooldown = false;
        player.GetComponent<PlayerJoystickControl>().SetInputEnabled(true);

        backgroundMusic?.Play();
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

        currentSpeed = chaseSpeed;
        isSlowingDown = false;
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

