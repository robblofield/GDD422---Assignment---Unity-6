using UnityEngine;

/// <summary>
/// Very simple smooth follow camera for 2D games.
/// - Follows a target Transform (usually the player).
/// - Uses Vector3.SmoothDamp to ease the movement in and out.
/// </summary>
public class FollowCamera2D : MonoBehaviour
{
    [Tooltip("The object that the camera should follow (for example, the player).")]
    public Transform target;

    [Tooltip("Offset from the target position. Adjust Z to place the camera in front or behind.")]
    public Vector3 offset = new Vector3(0f, 1f, -10f);

    [Tooltip("How long it takes for the camera to catch up to the target. Lower values = snappier, higher = smoother.")]
    public float smoothTime = 0.15f;

    // Internal value used by SmoothDamp.
    private Vector3 currentVelocity = Vector3.zero;

    private void LateUpdate()
    {
        // Always check that we have a target before trying to follow it.
        if (target == null)
        {
            return;
        }

        // Where we want the camera to be.
        Vector3 targetPosition = target.position + offset;

        // SmoothDamp moves the camera towards the target position over time.
        // It uses smoothTime to control how quickly it reaches the target.
        Vector3 newPosition = Vector3.SmoothDamp(
            transform.position,
            targetPosition,
            ref currentVelocity,
            smoothTime
        );

        transform.position = newPosition;
    }
}
