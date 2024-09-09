using UnityEngine;

public class CameraFollow : MonoBehaviour
{
    public Transform player; // Reference to the player's transform
    public float speedFactor = 3.0f; // Speed at which the camera follows the player
    public float deadZoneRadius = 4.0f; // Maximum distance the player can be from the camera before it starts moving
    public Vector3 offsetAdjustment = new Vector3(0, 0, 0); // Offset adjustment for fine-tuning the camera position

    private Vector3 offset; // Offset between the camera and the player

    void Start()
    {
        // Calculate the initial offset at the start
        offset = transform.position - player.position + offsetAdjustment;
    }

    void LateUpdate()
    {
        Vector3 desiredPosition = player.position + offset;
        float distance = Vector3.Distance(transform.position, desiredPosition);

        // Only move the camera if the player is outside the dead zone
        if (distance > deadZoneRadius)
        {
            transform.position = Vector3.Lerp(transform.position, desiredPosition, speedFactor * Time.deltaTime);
        }
    }

}