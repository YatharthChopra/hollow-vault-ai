using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

public class VisionCone : MonoBehaviour
{
    public float viewRadius = 9f;
    [Range(0, 360)]
    public float viewAngle = 60f;
    public LayerMask obstacleMask;
    public LayerMask playerMask;

    [HideInInspector] public bool playerVisible;

    public bool CanSee(Transform target)
    {
        if (target == null) return false;

        Vector3 toTarget = target.position - transform.position;
        float dist = toTarget.magnitude;

        // 1. distance check
        if (dist > viewRadius)
        {
            playerVisible = false;
            return false;
        }

        // 2. angle check
        Vector3 dir = toTarget.normalized;
        float angle = Vector3.Angle(transform.forward, dir);

        if (angle > viewAngle * 0.5f)
        {
            playerVisible = false;
            return false;
        }

        // 3. raycast — check nothing is blocking the line of sight
        if (Physics.Raycast(transform.position, dir, out RaycastHit hit, viewRadius, obstacleMask | playerMask))
        {
            playerVisible = (hit.transform == target);
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
