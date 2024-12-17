using UnityEngine;

public class MinimapCameraFollow : MonoBehaviour
{
    public Transform player; // Reference to the player's transform
    public Vector3 offset; // Offset from the player

    private void LateUpdate()
    {
        if (player != null)
        {
            // Update the minimap camera's position to follow the player with an offset
            Vector3 newPosition = player.position + offset;
            newPosition.y = transform.position.y; // Keep the same Y position for the minimap camera
            transform.position = newPosition;
        }
    }
}
