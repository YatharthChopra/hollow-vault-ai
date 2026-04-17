using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

public class VisionSensor : MonoBehaviour
{
    public float viewRadius = 9f;
    [Range(0, 360)]
    public float viewAngle = 60f;
    public LayerMask obstacleMask;
    public LayerMask playerMask;

    [HideInInspector] public bool playerVisible;

    public bool CanSeePlayer(Transform player)
    {
        if (player == null) return false;

        Vector3 toPlayer = player.position - transform.position;
        float dist = toPlayer.magnitude;

        // 1. distance check
        if (dist > viewRadius)
        {
            playerVisible = false;
            return false;
        }

        // 2. angle check
        Vector3 dirToPlayer = toPlayer.normalized;
        float angle = Vector3.Angle(transform.forward, dirToPlayer);

        if (angle > viewAngle * 0.5f)
        {
            playerVisible = false;
            return false;
        }

        // 3. raycast to check if anything is blocking the view
        if (Physics.Raycast(transform.position, dirToPlayer, out RaycastHit hit, viewRadius, obstacleMask | playerMask))
        {
            playerVisible = (hit.transform == player);
            return playerVisible;
        }

        playerVisible = false;
        return false;
    }

    private void OnDrawGizmosSelected()
    {
#if UNITY_EDITOR
        Handles.color = playerVisible ? new Color(1f, 0f, 0f, 0.25f) : new Color(0f, 1f, 1f, 0.20f);

        Vector3 fwd = transform.forward;
        Handles.DrawSolidArc(transform.position, Vector3.up, fwd, viewAngle * 0.5f, viewRadius);
        Handles.DrawSolidArc(transform.position, Vector3.up, fwd, -viewAngle * 0.5f, viewRadius);
#endif
    }
}
