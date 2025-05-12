using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SmoothWaterFill : MonoBehaviour
{
    public float targetScaleY = 2.71227f;
    public float duration = 2.0f; // Time in seconds for full water rise
    private float elapsedTime = 0f;
    private Vector3 initialScale;

    void Start()
    {
        initialScale = transform.localScale;
        transform.localScale = new Vector3(initialScale.x, 0, initialScale.z); // Start with zero height
    }

    void Update()
    {
        if (elapsedTime < duration)
        {
            elapsedTime += Time.deltaTime;
            float progress = Mathf.Clamp01(elapsedTime / duration);
            float easedProgress = Mathf.SmoothStep(0, 1, progress);
            float newY = Mathf.Lerp(0, targetScaleY, easedProgress);
            transform.localScale = new Vector3(initialScale.x, newY, initialScale.z);
        }
    }
}
