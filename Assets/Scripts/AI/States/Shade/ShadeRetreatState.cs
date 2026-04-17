using UnityEngine;

/// <summary>
/// RETREAT state — Shade flees from the torch toward the nearest shadow (perimeter waypoint).
///
/// Transitions OUT:
///   → DRIFT : safe shadow reached AND torch extinguished
/// </summary>
public class ShadeRetreatState : State
{
    private ShadeBrain s;

    public ShadeRetreatState(ShadeBrain shade) : base(shade)
    {
        s = shade;
    }

    public override void Enter()
    {
        s.agent.speed = s.huntSpeed; // flee fast
        // Find furthest waypoint from player (shadow area)
        s.agent.SetDestination(FindShadowWaypoint());
        Debug.Log("[Shade] Entering RETREAT");
    }

    public override void Execute()
    {
        // Once torch is out AND we have reached shadow → resume drifting
        bool torchGone = !s.lightSense.DetectsTorch();
        float dist = s.agent.remainingDistance;
        bool arrived = dist < s.waypointReachedThreshold && !s.agent.pathPending;

        if (torchGone && arrived)
        {
            s.ChangeState(new ShadeDriftState(s));
        }
    }

    public override void Exit()
    {
        Debug.Log("[Shade] Exiting RETREAT");
    }

    // Pick the waypoint furthest from the player (dark corner)
    private Vector3 FindShadowWaypoint()
    {
        Vector3 best = s.transform.position;
        float maxDist = 0f;
        foreach (Transform wp in s.patrolWaypoints)
        {
            float d = Vector3.Distance(wp.position, s.player.position);
            if (d > maxDist)
            {
                maxDist = d;
                best = wp.position;
            }
        }
        return best;
    }
}
