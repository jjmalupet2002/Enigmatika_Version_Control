using UnityEngine;

public class CameraCollisionHandler : MonoBehaviour
{
    public LayerMask collisionLayers; // Layers that the camera can collide with
    public float cameraRadius = 0.5f; // Radius of the camera collider

    public Vector3 CheckCollision(Vector3 targetPosition)
    {
        // Check if the target position would result in a collision
        if (IsCollision(targetPosition))
        {
            // Handle collision response (e.g., stop movement or adjust position)
            return AdjustForCollision(transform.position, targetPosition);
        }

        // No collision detected, return the target position
        return targetPosition;
    }

    private bool IsCollision(Vector3 targetPosition)
    {
        // Check if the target position would result in a collision
        Collider[] hitColliders = Physics.OverlapSphere(targetPosition, cameraRadius, collisionLayers);
        return hitColliders.Length > 0;
    }

    private Vector3 AdjustForCollision(Vector3 currentPosition, Vector3 targetPosition)
    {
        // Simple collision response by stopping the camera at the current position
        // You can implement more complex logic (e.g., sliding along surfaces)
        return currentPosition;
    }

    void OnDrawGizmosSelected()
    {
        // Draw a sphere in the Scene view to visualize the camera collider
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, cameraRadius);
    }
}
