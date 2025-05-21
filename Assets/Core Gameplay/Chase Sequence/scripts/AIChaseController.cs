using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class AIChaseController : MonoBehaviour
{
    [System.Serializable]
    public class EnemyData
    {
        public GameObject enemyObject;
        public GameObject triggerZone;
        public float moveSpeed = 3f;
        public bool moveInZ = true;
        public float targetZ;
        public bool moveInX = false;
        public float targetX;
        public float collisionRange = 1f;
        public float triggerRange = 1f;    // Custom range for trigger zone detection

        [HideInInspector] public Vector3 startPosition;
        [HideInInspector] public bool hasBeenTriggered = false;
    }

    [Header("References")]
    public Transform player;
    public Transform playerStartPosition;
    public EnemyTrapScript trapScript;
    public CanvasGroup blackScreenCanvasGroup;
    public AudioSource chaseMusic;
    public AudioSource backgroundMusic;

    [Header("Enemies")]
    public List<EnemyData> enemies = new List<EnemyData>();

    [Header("Settings")]
    public float fadeDuration = 1.5f;

    private Rigidbody playerRb;

    void Start()
    {
        playerRb = player.GetComponent<Rigidbody>();

        foreach (var enemy in enemies)
        {
            enemy.startPosition = enemy.enemyObject.transform.position;
        }

        if (blackScreenCanvasGroup != null)
            blackScreenCanvasGroup.alpha = 0f;
    }

    void Update()
    {
        foreach (var enemy in enemies)
        {
            if (!enemy.hasBeenTriggered)
            {
                // Check if player overlaps trigger zone with customizable range
                if (enemy.triggerZone != null &&
                    Physics.OverlapSphere(enemy.triggerZone.transform.position, enemy.triggerRange).Length > 0)
                {
                    foreach (var hit in Physics.OverlapSphere(enemy.triggerZone.transform.position, enemy.triggerRange))
                    {
                        if (hit.transform == player)
                        {
                            enemy.hasBeenTriggered = true;
                            chaseMusic?.Play();
                        }
                    }
                }
            }
            else
            {
                MoveEnemy(enemy);
                CheckPlayerCollision(enemy);
            }
        }
    }

    private void MoveEnemy(EnemyData enemy)
    {
        Vector3 currentPos = enemy.enemyObject.transform.position;
        Vector3 targetPos = currentPos;

        if (enemy.moveInZ)
            targetPos.z = enemy.targetZ;

        if (enemy.moveInX)
            targetPos.x = enemy.targetX;

        Vector3 direction = (targetPos - currentPos).normalized;
        enemy.enemyObject.transform.position += direction * enemy.moveSpeed * Time.deltaTime;
    }

    private void CheckPlayerCollision(EnemyData enemy)
    {
        float dist = Vector3.Distance(enemy.enemyObject.transform.position, player.position);
        if (dist <= enemy.collisionRange)
        {
            StartCoroutine(HandlePlayerCaught());
        }
    }

    private IEnumerator HandlePlayerCaught()
    {
        // Disable player movement temporarily
        player.GetComponent<PlayerJoystickControl>().SetInputEnabled(false);
        chaseMusic?.Stop();
        backgroundMusic?.Stop();

        yield return StartCoroutine(FadeScreen(1));

        ResetPlayerPosition();
        ResetEnemies();

        yield return new WaitForSeconds(0.5f);
        yield return StartCoroutine(FadeScreen(0));

        player.GetComponent<PlayerJoystickControl>().SetInputEnabled(true);
        backgroundMusic?.Play();
    }

    private void ResetPlayerPosition()
    {
        if (playerRb != null)
        {
            playerRb.velocity = Vector3.zero;
            playerRb.angularVelocity = Vector3.zero;
            playerRb.MovePosition(playerStartPosition.position);
        }
        else
        {
            player.position = playerStartPosition.position;
        }

        if (trapScript != null)
            trapScript.resetTrapGate = true;
    }

    private void ResetEnemies()
    {
        foreach (var enemy in enemies)
        {
            enemy.enemyObject.transform.position = enemy.startPosition;
            enemy.hasBeenTriggered = false;
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

    // Optional: visualize trigger and collision areas
    private void OnDrawGizmosSelected()
    {
        foreach (var enemy in enemies)
        {
            if (enemy.triggerZone != null)
            {
                Gizmos.color = Color.yellow;
                Gizmos.DrawWireSphere(enemy.triggerZone.transform.position, enemy.triggerRange); // Use dynamic range
            }

            if (enemy.enemyObject != null)
            {
                Gizmos.color = Color.red;
                Gizmos.DrawWireSphere(enemy.enemyObject.transform.position, enemy.collisionRange);
            }
        }
    }
}
