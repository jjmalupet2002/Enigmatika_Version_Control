using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TeleporterManager : MonoBehaviour
{
    [System.Serializable]
    public class TeleporterPair
    {
        public Transform teleporterA;
        public Transform teleporterB;
    }

    public List<TeleporterPair> teleporterPairs;
    public Transform player;
    public Transform mainCamera;
    public float teleportDelay = 0.5f;
    public CanvasGroup fadeCanvasGroup;

    private Dictionary<Transform, Transform> teleporterDict;
    private bool isTeleporting = false;

    private void Start()
    {
        teleporterDict = new Dictionary<Transform, Transform>();

        foreach (var pair in teleporterPairs)
        {
            teleporterDict[pair.teleporterA] = pair.teleporterB;
            teleporterDict[pair.teleporterB] = pair.teleporterA;
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            Teleport(other.transform);
        }
    }


    public void Teleport(Transform teleporter)
    {
        if (!isTeleporting && teleporterDict.ContainsKey(teleporter))
        {
            StartCoroutine(TeleportPlayer(teleporterDict[teleporter]));
        }
    }

    private IEnumerator TeleportPlayer(Transform destination)
    {
        isTeleporting = true;

        // Fade out
        yield return StartCoroutine(FadeOut());

        // Calculate the offset between the player and the camera
        Vector3 cameraOffset = mainCamera.position - player.position;

        // Teleport the player
        player.position = destination.position;

        // Maintain camera's offset relative to the player, keeping the y-axis constant
        mainCamera.position = new Vector3(player.position.x + cameraOffset.x, mainCamera.position.y, player.position.z + cameraOffset.z);

        // Fade in
        yield return StartCoroutine(FadeIn());

        isTeleporting = false;
    }

    private IEnumerator FadeOut()
    {
        float elapsedTime = 0f;
        while (fadeCanvasGroup.alpha < 1)
        {
            elapsedTime += Time.deltaTime;
            fadeCanvasGroup.alpha = Mathf.Clamp01(elapsedTime / teleportDelay);
            yield return null;
        }
    }

    private IEnumerator FadeIn()
    {
        float elapsedTime = 0f;
        while (fadeCanvasGroup.alpha > 0)
        {
            elapsedTime += Time.deltaTime;
            fadeCanvasGroup.alpha = Mathf.Clamp01(1 - (elapsedTime / teleportDelay));
            yield return null;
        }
    }
}


