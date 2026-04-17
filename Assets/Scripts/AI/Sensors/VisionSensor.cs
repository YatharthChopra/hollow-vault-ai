using UnityEngine;
using UnityEditor;

/// <summary>
/// Vision sensor: checks whether the player is inside a forward-facing view cone
/// and not occluded by obstacles.
///
/// Attach to the same GameObject as the AI agent.
/// Usage: call CanSeePlayer() from any state.
/// </summary>
public class VisionSensor : MonoBehaviour
{
    [Header("Vision Settings")]
    [Tooltip("Maximum detection range in metres.")]
    public float viewRadius = 9f;

    [Tooltip("Total horizontal field of view in degrees.")]
    [Range(0f, 360f)]
    public float viewAngle = 60f;

    [Tooltip("Layer mask for obstacles that block line of sight.")]
    public LayerMask obstacleMask;

    [Tooltip("Layer mask that identifies the player.")]
    public LayerMask playerMask;

    // Cached result for Gizmo drawing
    [HideInInspector] public bool playerVisible;

    /// <summary>
    /// Returns true if the player is within the view cone and has clear line of sight.
    /// </summary>
    public bool CanSeePlayer(Transform player)
    {
        if (player == null) return false;

        Vector3 toPlayer = player.position - transform.position;
        float dist = toPlayer.magnitude;

        // 1. Distance check
        if (dist > viewRadius)
        {
            playerVisible = false;
            return false;
        }

        // 2. Angle check (half-angle on each side of forward)
        Vector3 dirToPlayer = toPlayer.normalized;
        float angle = Vector3.Angle(transform.forward, dirToPlayer);

        if (angle > viewAngle * 0.5f)
        {
            playerVisible = false;
            return false;
        }

        // 3. Raycast – check nothing blocks the line of sight
        if (Physics.Raycast(transform.position, dirToPlayer, out RaycastHit hit, viewRadius, obstacleMask | playerMask))
        {
            playerVisible = (hit.transform == player);
            return playerVisible;
        }

        playerVisible = false;
        return false;
    }

    // -------------------------------------------------------------------------
    // Editor Gizmos – visualise the cone in Scene view
    // -------------------------------------------------------------------------
    private void OnDrawGizmosSelected()
    {
#if UNITY_EDITOR
        Handles.color = playerVisible
            ? new Color(1f, 0f, 0f, 0.25f)
            : new Color(0f, 1f, 1f, 0.20f);

        Vector3 fwd = transform.forward;
        Handles.DrawSolidArc(transform.position, Vector3.up,  fwd,  viewAngle * 0.5f, viewRadius);
        Handles.DrawSolidArc(transform.position, Vector3.up,  fwd, -viewAngle * 0.5f, viewRadius);
#endif
    }
}
