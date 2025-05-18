using System.Diagnostics;
using UnityEngine;

public class WaterTrap : MonoBehaviour
{
    [Header("Water Settings")]
    public GameObject waterObject;
    public float targetYPosition = 5f;
    private float initialYPosition;

    [Header("Trigger Settings")]
    public GameObject triggerPoint;
    public float triggerRadius = 2f;

    [Header("Instruction Dependency")]
    public GameObject waterInstruction;
    private bool instructionShown = false;
    private bool triggerEnabled = false;

    [Header("Water Movement")]
    private bool isRising = false;
    private bool isResetting = false;
    private float riseSpeed = 1f;
    private float elapsedTime = 0f;
    private float totalRiseDuration = 180f; // 3 minutes

    private void Start()
    {
        if (waterObject == null)
        {
            return;
        }

        if (triggerPoint == null)
        {
            return;
        }

        if (waterInstruction == null)
        {
            return;
        }

        initialYPosition = waterObject.transform.position.y;
    }

    private void Update()
    {
        // Check if instruction was shown and now turned off
        if (!triggerEnabled)
        {
            if (waterInstruction.activeSelf)
            {
                instructionShown = true;
            }
            else if (instructionShown && !waterInstruction.activeSelf)
            {
                triggerEnabled = true;
            }
        }

        if (triggerEnabled)
        {
            CheckForTrigger();
        }

        if (isRising)
            RaiseWater();

        if (isResetting)
            ResetWater();
    }

    private void CheckForTrigger()
    {
        if (!isRising)
        {
            Collider[] hits = Physics.OverlapSphere(triggerPoint.transform.position, triggerRadius);
            foreach (var hit in hits)
            {
                if (hit.CompareTag("Player"))
                {
                    UnityEngine.Debug.Log("Water rising triggered by player.");
                    StartWaterRise();
                    break;
                }
            }
        }
    }

    public void StartWaterRise()
    {
        isRising = true;
        isResetting = false;
        elapsedTime = 0f;
        riseSpeed = 1f / totalRiseDuration;
    }

    public void SpeedUpWaterRise(float multiplier)
    {
        riseSpeed *= multiplier;
    }

    public void ResetWaterPosition()
    {
        isResetting = true;
        isRising = false;
        elapsedTime = 0f;
    }

    private void RaiseWater()
    {
        elapsedTime += Time.deltaTime;
        float t = Mathf.Clamp01(elapsedTime * riseSpeed);
        Vector3 currentPos = waterObject.transform.position;
        float newY = Mathf.Lerp(initialYPosition, targetYPosition, t);
        waterObject.transform.position = new Vector3(currentPos.x, newY, currentPos.z);

        if (Mathf.Approximately(newY, targetYPosition))
            isRising = false;
    }

    private void ResetWater()
    {
        elapsedTime += Time.deltaTime;
        float t = Mathf.Clamp01(elapsedTime / totalRiseDuration);
        Vector3 currentPos = waterObject.transform.position;
        float newY = Mathf.Lerp(currentPos.y, initialYPosition, t);
        waterObject.transform.position = new Vector3(currentPos.x, newY, currentPos.z);

        if (Mathf.Approximately(newY, initialYPosition))
            isResetting = false;
    }

    private void OnDrawGizmosSelected()
    {
        if (triggerPoint != null)
        {
            Gizmos.color = Color.cyan;
            Gizmos.DrawWireSphere(triggerPoint.transform.position, triggerRadius);
        }
    }
}
