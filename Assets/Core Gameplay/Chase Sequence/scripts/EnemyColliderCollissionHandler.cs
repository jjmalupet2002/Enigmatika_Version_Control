using System.Diagnostics;
using UnityEngine;

public class EnemyColliderHandler : MonoBehaviour
{
    private AIChaseController chaseController;

    void Start()
    {
        // Find the AIChaseController in the scene
        chaseController = FindObjectOfType<AIChaseController>();

        if (chaseController == null)
        {
            UnityEngine.Debug.LogError("AIChaseController not found in the scene.");
        }
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (collision.transform.CompareTag("Player"))
        {
            if (chaseController != null)
            {
                chaseController.OnPlayerCaught();
            }
        }
    }
}
