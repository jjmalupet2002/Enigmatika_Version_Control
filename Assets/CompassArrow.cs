using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CompassArrow : MonoBehaviour
{
    public Transform player;
    public Transform compassPivot;
    public Transform target; // The object of interest
    public float minRotationSpeed = -120f;
    public float maxRotationSpeed = 120f;
    public float changeDirectionInterval = 0.5f;
    public bool testPointing = false; // Toggle to test pointing mode
    public float pointingSpeed = 180f; // Degrees per second when pointing
    public GameObject arrow3dModel;
    public GameObject questTextObject;
    public float disableDistance = 2f; // Distance at which arrow is hidden

    private float currentSpeed;
    private float timer;

    void Start()
    {
        PickNewRandomSpeed();
    }

    void Update()
    {
        // Keep compass pivot following player
        compassPivot.position = player.position;

        // Logic for showing/hiding arrow
        if (arrow3dModel != null && questTextObject != null && target != null)
        {
            float distanceToTarget = Vector3.Distance(player.position, target.position);

            if (questTextObject.activeInHierarchy && distanceToTarget > disableDistance)
            {
                arrow3dModel.SetActive(true);
            }
            else
            {
                arrow3dModel.SetActive(false);
            }
        }

        if (testPointing && target != null)
        {
            PointAtTarget();
        }
        else
        {
            timer += Time.deltaTime;
            if (timer >= changeDirectionInterval)
            {
                PickNewRandomSpeed();
                timer = 0f;
            }

            compassPivot.RotateAround(compassPivot.position, Vector3.up, currentSpeed * Time.deltaTime);
        }
    }

    public void SetTarget(Transform newTarget)
    {
        target = newTarget;
        testPointing = newTarget != null; // Enable pointing if a valid target is set
    }

    void PickNewRandomSpeed()
    {
        currentSpeed = UnityEngine.Random.Range(minRotationSpeed, maxRotationSpeed);
    }

    void PointAtTarget()
    {
        Vector3 direction = target.position - compassPivot.position;
        direction.y = 0f;

        if (direction.sqrMagnitude < 0.001f) return;

        Quaternion targetRotation = Quaternion.LookRotation(direction);

        compassPivot.rotation = Quaternion.RotateTowards(
            compassPivot.rotation,
            targetRotation,
            pointingSpeed * Time.deltaTime
        );
    }
}
